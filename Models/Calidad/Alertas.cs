namespace calidad_app.Models.Calidad;

/// <summary>
/// Alerta de proceso: un parámetro que quedó fuera de la tolerancia de la
/// ficha técnica.
///
/// El valor y los límites son los del momento en que se detectó, y se leen de
/// la propia alerta y no de la ficha vigente: si mañana cambia la ficha, la
/// alerta debe seguir mostrando contra qué se comparó cuando ocurrió. Eso es
/// lo que la hace evidencia.
///
/// Casi todos los identificadores son nulos porque la alerta puede existir sin
/// bobina (medición a nivel de registro) y sin lote (bobinas todavía sin
/// agrupar).
/// </summary>
public class AlertaResumen
{
    public int AlertaId { get; set; }

    public int? RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int? OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;

    public int? ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;

    public int? BobinaId { get; set; }
    public int? IdBobi { get; set; }
    public int? LoteId { get; set; }
    public string? CodigoLote { get; set; }

    public int ParametroId { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }

    public decimal? ValorRegistrado { get; set; }
    public decimal? LimiteInferior { get; set; }
    public decimal? LimiteSuperior { get; set; }

    /// <summary>"Bajo" o "Alto": de qué lado se salió el valor.</summary>
    public string Desviacion { get; set; } = string.Empty;

    public int? MaquinaId { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public int? AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int? TurnoId { get; set; }
    public string TurnoNombre { get; set; } = string.Empty;
    public int? OperadorId { get; set; }
    public string OperadorNombre { get; set; } = string.Empty;

    public DateTime FechaDeteccion { get; set; }
    public bool Atendida { get; set; }
    public DateTime? FechaAtencion { get; set; }
    public int? AtendidaPorId { get; set; }
    public string? AtendidaPorNombre { get; set; }
    public string? Observacion { get; set; }

    public int? NoConformidadId { get; set; }
    public string? NoConformidadCodigo { get; set; }

    public int HorasSinAtender { get; set; }

    /// <summary>La medición se tomó sobre una bobina y no sobre la corrida completa.</summary>
    public bool EsDeBobina => BobinaId is not null;

    public string OrigenTexto => EsDeBobina ? $"Bobina {IdBobi}" : "Corrida";

    /// <summary>Rango de la ficha, con el lado abierto cuando no hay límite por ese lado.</summary>
    public string RangoTexto => (LimiteInferior, LimiteSuperior) switch
    {
        (null, null) => "sin límites",
        (not null, null) => $"≥ {LimiteInferior:0.###}",
        (null, not null) => $"≤ {LimiteSuperior:0.###}",
        _ => $"{LimiteInferior:0.###} a {LimiteSuperior:0.###}"
    };
}

/// <summary>
/// Alerta con el contexto necesario para decidir qué hacer con ella: además de
/// lo que ya trae el resumen, el producto, la ficha aplicada y el valor
/// objetivo.
/// </summary>
public class AlertaDetalle : AlertaResumen
{
    public DateOnly? FechaRegistroInspeccion { get; set; }
    public string EstadoRegistro { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;

    public decimal? BobinaPeso { get; set; }
    public bool? BobinaEsConforme { get; set; }

    public int? FichaId { get; set; }
    public string? FichaVersion { get; set; }
    public decimal? ValorObjetivo { get; set; }

    public string? NoConformidadEstado { get; set; }

    /// <summary>
    /// Todas las mediciones de ese parámetro en el registro, en orden. Sirve
    /// para distinguir un pico aislado de una tendencia: una desviación que se
    /// repite no se cierra con una observación, se escala a no conformidad.
    /// </summary>
    public List<MedicionParametro> Historial { get; set; } = [];

    /// <summary>Otras alertas sin atender del mismo registro.</summary>
    public List<AlertaRelacionada> Relacionadas { get; set; } = [];
}

/// <summary>Una medición del parámetro dentro del registro.</summary>
public class MedicionParametro
{
    public int RegParamId { get; set; }
    public int? BobinaId { get; set; }
    public int? IdBobi { get; set; }
    public decimal? ValorRegistrado { get; set; }

    /// <summary>Tres estados: dentro, fuera, o null cuando no hubo contra qué comparar.</summary>
    public bool? DentroDeRango { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string OrigenTexto => BobinaId is null ? "Corrida" : $"Bobina {IdBobi}";
}

/// <summary>Otra alerta abierta del mismo registro, para verlas juntas.</summary>
public class AlertaRelacionada
{
    public int AlertaId { get; set; }
    public int? BobinaId { get; set; }
    public int? IdBobi { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public bool EsCritico { get; set; }
    public decimal? ValorRegistrado { get; set; }
    public DateTime FechaDeteccion { get; set; }
}

/// <summary>
/// Filtros del monitor de alertas. Todos opcionales: en null el procedimiento
/// no filtra por ese campo.
/// </summary>
public class FiltroAlertas
{
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public int? AreaId { get; set; }
    public int? MaquinaId { get; set; }
    public int? TurnoId { get; set; }
    public int? OperadorId { get; set; }
    public int? ParametroId { get; set; }
    public int? RegistroId { get; set; }
    public int? LoteId { get; set; }

    /// <summary>null trae atendidas y pendientes. La pantalla arranca en false.</summary>
    public bool? Atendida { get; set; } = false;

    public bool SoloCriticos { get; set; }
}

/// <summary>Totales de la cola de alertas del periodo consultado.</summary>
public class TotalesAlertas
{
    public int TotalAlertas { get; set; }
    public int Pendientes { get; set; }
    public int Atendidas { get; set; }
    public int Criticas { get; set; }
    public int CriticasPendientes { get; set; }
    public DateTime? PendienteMasAntigua { get; set; }
}

/// <summary>Alertas agrupadas por parámetro: qué variable se desvía más.</summary>
public class AlertasPorParametro
{
    public int ParametroId { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }
    public int Total { get; set; }
    public int Pendientes { get; set; }
}

/// <summary>Alertas agrupadas por máquina: dónde ocurre.</summary>
public class AlertasPorMaquina
{
    public int MaquinaId { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public string AreaNombre { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Pendientes { get; set; }
}

/// <summary>Alertas agrupadas por turno: cuándo ocurre.</summary>
public class AlertasPorTurno
{
    public int TurnoId { get; set; }
    public string TurnoNombre { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Pendientes { get; set; }
}

/// <summary>
/// Los cuatro cortes que devuelve cal.usp_Alerta_Resumen en una sola llamada.
/// </summary>
public class ResumenAlertas
{
    public TotalesAlertas Totales { get; set; } = new();
    public List<AlertasPorParametro> PorParametro { get; set; } = [];
    public List<AlertasPorMaquina> PorMaquina { get; set; } = [];
    public List<AlertasPorTurno> PorTurno { get; set; } = [];
}

/// <summary>
/// Resultado de atender alertas. AlertasYaAtendidas &gt; 0 significa que otro
/// usuario se adelantó: no es un error, pero la pantalla debe avisarlo.
/// </summary>
public class ResultadoAtencion
{
    public int AlertasAtendidas { get; set; }
    public int AlertasYaAtendidas { get; set; }
}
