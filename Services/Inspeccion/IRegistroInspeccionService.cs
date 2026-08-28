using calidad_app.Models.Inspeccion;

namespace calidad_app.Services.Inspeccion;

/// <summary>
/// Registro de inspección: encabezado y las secciones que dependen solo de él
/// (especificaciones, mezcla, setup, producción, despeje y validación previa a
/// liberar). Las bobinas y las mediciones de parámetros tienen sus propios
/// servicios porque tienen ciclo de vida propio.
///
/// Ninguna operación recibe quién la ejecuta: el UsuarioId sale del claim de la
/// sesión, dentro del servicio.
/// </summary>
public interface IRegistroInspeccionService
{
    Task<List<RegistroResumen>> ListarAsync(FiltroRegistros filtro, CancellationToken ct = default);

    Task<RegistroDetalle?> ObtenerAsync(int registroId, CancellationToken ct = default);

    /// <summary>
    /// Busca la orden sobre la que abrir un registro. No es un catálogo
    /// completo: hay cientos de órdenes y el operador llega con el número de
    /// OP. Por defecto excluye las cerradas, sobre las que no se puede abrir.
    /// </summary>
    Task<List<OrdenBusqueda>> BuscarOrdenesAsync(
        string? busqueda, bool incluirCerradas = false, CancellationToken ct = default);

    /// <summary>Abre un registro. Devuelve el id y el IdRegistro generado.</summary>
    Task<(int RegistroId, string IdRegistro)> CrearAsync(NuevoRegistro nuevo, CancellationToken ct = default);

    /// <summary>Corrige la cabecera. Los valores en null se dejan como están.</summary>
    Task ActualizarEncabezadoAsync(
        int registroId,
        DateOnly? fecha = null,
        int? turnoId = null,
        int? operadorId = null,
        int? maquinaId = null,
        DateTime? fechaHoraInicio = null,
        CancellationToken ct = default);

    Task<EspecificacionProceso?> ObtenerEspecificacionAsync(int registroId, CancellationToken ct = default);

    /// <summary>
    /// Guarda la sección completa: aquí un null sí borra el campo, así que la
    /// pantalla debe enviar siempre el formulario entero.
    /// </summary>
    Task GuardarEspecificacionAsync(EspecificacionProceso especificacion, CancellationToken ct = default);

    Task<MezclaMateriales> ObtenerMezclaAsync(int registroId, CancellationToken ct = default);

    /// <summary>
    /// Reemplaza la mezcla completa. Con validarSuma en false se admite una
    /// mezcla incompleta mientras el operador la captura; la comprobación
    /// definitiva la hace igualmente la validación previa a liberar.
    /// </summary>
    Task<MezclaMateriales> GuardarMezclaAsync(
        int registroId,
        IEnumerable<MaterialEntrada> materiales,
        bool validarSuma = true,
        CancellationToken ct = default);

    Task<SetupRegistro?> ObtenerSetupAsync(int registroId, CancellationToken ct = default);

    Task<SetupRegistro?> GuardarSetupAsync(SetupRegistro setup, CancellationToken ct = default);

    Task<ProduccionRegistro?> ObtenerProduccionAsync(int registroId, CancellationToken ct = default);

    Task<ProduccionRegistro?> GuardarProduccionAsync(ProduccionRegistro produccion, CancellationToken ct = default);

    Task<Checklist> ObtenerChecklistAsync(
        int registroId, string seccion = SeccionChecklist.DespejeLinea, CancellationToken ct = default);

    Task<Checklist> GuardarChecklistAsync(
        int registroId,
        IEnumerable<RespuestaChecklist> respuestas,
        string seccion = SeccionChecklist.DespejeLinea,
        CancellationToken ct = default);

    /// <summary>Diagnóstico de solo lectura: qué falta y qué impide liberar.</summary>
    Task<ValidacionCompletitud> ValidarCompletitudAsync(int registroId, CancellationToken ct = default);
}
