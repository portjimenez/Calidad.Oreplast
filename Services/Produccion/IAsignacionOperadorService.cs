using calidad_app.Models.Produccion;

namespace calidad_app.Services.Produccion;

/// <summary>
/// Programación de operadores por máquina y turno.
///
/// Toda la escritura exige el permiso ASIGNAR_OPERADORES, que la base
/// comprueba en cada procedimiento; la pantalla nunca envía el usuario, lo
/// resuelve el servicio a partir de la identidad autenticada.
///
/// Las reglas que impiden un choque de programación (una máquina con dos
/// operadores en el mismo turno, un operador en dos máquinas a la vez) viven
/// en la base y no aquí: son las mismas para cualquiera que escriba en
/// <c>prod.AsignacionOperador</c>, y repetirlas en C# solo abriría la puerta a
/// que las dos versiones dejen de coincidir.
/// </summary>
public interface IAsignacionOperadorService
{
    /// <summary>
    /// La rejilla de un día: todas las máquinas activas por todos los turnos,
    /// estén asignadas o no. Las celdas vacías son los huecos que hay que
    /// llenar, así que se devuelven a propósito.
    /// </summary>
    Task<List<CeldaAsignacion>> ListarRejillaAsync(
        DateOnly fecha, int? areaId = null, int? lineaId = null,
        int? maquinaId = null, int? turnoId = null, CancellationToken ct = default);

    /// <summary>
    /// Solo lo ya programado de un rango, para revisar la semana o buscar
    /// dónde estuvo un operador. No devuelve celdas vacías.
    /// </summary>
    Task<List<CeldaAsignacion>> ListarHistorialAsync(
        DateOnly fechaDesde, DateOnly fechaHasta, int? areaId = null, int? lineaId = null,
        int? maquinaId = null, int? turnoId = null, int? operadorId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Candidatos para una celda, incluidos los que ya están ocupados (vienen
    /// marcados). Ocultarlos dejaría a quien programa sin saber por qué no
    /// encuentra a alguien.
    /// </summary>
    Task<List<OperadorDisponible>> ListarOperadoresAsync(
        DateOnly fecha, int turnoId, int? areaId = null, string? busqueda = null,
        CancellationToken ct = default);

    /// <summary>Programa o mueve una celda. Devuelve el id de la asignación.</summary>
    Task<int> GuardarAsync(AsignacionEdicion asignacion, CancellationToken ct = default);

    /// <summary>
    /// Quita una celda. La base lo rechaza si ya hay producción capturada bajo
    /// ella: esa asignación es historia y se consulta, pero no se borra.
    /// </summary>
    Task EliminarAsync(int asignacionId, CancellationToken ct = default);

    /// <summary>
    /// Copia la programación de un día a otro. Con
    /// <paramref name="sobrescribir"/> en falso el destino tiene que estar
    /// limpio; en cierto se rehace, pero nunca se pisa una celda que ya tenga
    /// registros de inspección capturados.
    /// </summary>
    Task<ResultadoCopiaAsignaciones> CopiarAsync(
        DateOnly fechaOrigen, DateOnly fechaDestino, int? areaId = null,
        int? lineaId = null, bool sobrescribir = false, CancellationToken ct = default);
}
