namespace calidad_app.Models.Produccion;

/// <summary>
/// Una celda de la programación: una máquina, un turno y una fecha, con el
/// operador asignado si lo hay.
///
/// Las celdas VACÍAS también llegan, y son la mitad del valor de la pantalla:
/// son los huecos que el jefe de producción tiene que llenar. Por eso la
/// consulta devuelve la rejilla completa (máquinas activas × turnos) y no solo
/// lo ya programado.
/// </summary>
public class CeldaAsignacion
{
    public DateOnly Fecha { get; set; }

    public int MaquinaId { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public bool MaquinaActiva { get; set; }

    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int? LineaId { get; set; }
    public string? LineaCodigo { get; set; }
    public string? LineaNombre { get; set; }

    public int TurnoId { get; set; }
    public string TurnoNombre { get; set; } = string.Empty;

    public int? AsignacionId { get; set; }
    public DateTime? AsignadaEl { get; set; }

    public int? OperadorId { get; set; }
    public string? OperadorCodigo { get; set; }
    public string? OperadorNombre { get; set; }
    public bool? OperadorActivo { get; set; }

    /// <summary>Registros de inspección capturados en esa máquina, turno y fecha.</summary>
    public int Registros { get; set; }

    /// <summary>
    /// De esos registros, los que capturó alguien distinto del operador
    /// programado. Es la diferencia entre lo que se planificó y lo que
    /// realmente pasó en la planta; sin este número la pantalla sería solo una
    /// agenda.
    /// </summary>
    public int RegistrosDeOtroOperador { get; set; }

    public bool Asignada { get; set; }

    /// <summary>
    /// Falso en cuanto hay producción capturada bajo la celda: esa asignación
    /// ya es historia y se consulta, pero no se borra.
    /// </summary>
    public bool PuedeEliminarse { get; set; }

    public string Linea => LineaCodigo ?? "(sin línea)";
    public bool HayDesvio => RegistrosDeOtroOperador > 0;
}

/// <summary>Lo que la pantalla envía al programar o mover una celda.</summary>
public class AsignacionEdicion
{
    public int AsignacionId { get; set; }
    public int OperadorId { get; set; }
    public int MaquinaId { get; set; }
    public int TurnoId { get; set; }
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public static AsignacionEdicion Desde(CeldaAsignacion celda) => new()
    {
        AsignacionId = celda.AsignacionId ?? 0,
        OperadorId = celda.OperadorId ?? 0,
        MaquinaId = celda.MaquinaId,
        TurnoId = celda.TurnoId,
        Fecha = celda.Fecha
    };
}

/// <summary>
/// Candidato para una celda. Incluye a los que YA están ocupados, marcados con
/// <see cref="Ocupado"/> y con la máquina donde están: ocultarlos sería peor,
/// porque quien programa busca a alguien por su nombre, no lo encuentra y no
/// sabe si es que no existe, si está inactivo o si él mismo acaba de asignarlo.
/// </summary>
public class OperadorDisponible
{
    public int OperadorId { get; set; }
    public string OperadorCodigo { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }
    public string RolNombre { get; set; } = string.Empty;

    public int? AsignacionId { get; set; }
    public int? MaquinaId { get; set; }
    public string? MaquinaCodigo { get; set; }
    public string? MaquinaNombre { get; set; }
    public bool Ocupado { get; set; }

    /// <summary>
    /// En cuántos turnos de esa fecha ya está programado. La base no lo
    /// prohíbe (un doble turno es legítimo y ocurre), pero cambia la decisión
    /// de quien programa.
    /// </summary>
    public int TurnosEnElDia { get; set; }

    public string Etiqueta => $"{OperadorCodigo} - {OperadorNombre}";

    public string? Advertencia => Ocupado
        ? $"Ya está en {MaquinaCodigo} en este turno"
        : TurnosEnElDia > 0
            ? $"Ya cubre {TurnosEnElDia} turno(s) este día"
            : null;
}

/// <summary>
/// Resultado de copiar la programación de un día a otro. <c>Omitidas</c> son
/// las celdas que no se copiaron porque ya no son válidas (máquina retirada,
/// operador dado de baja) o porque el destino ya tenía producción capturada;
/// la pantalla las reporta para que quien programa sepa qué revisar.
/// </summary>
public class ResultadoCopiaAsignaciones
{
    public DateOnly FechaDestino { get; set; }
    public int EnOrigen { get; set; }
    public int Copiadas { get; set; }
    public int Omitidas { get; set; }
}
