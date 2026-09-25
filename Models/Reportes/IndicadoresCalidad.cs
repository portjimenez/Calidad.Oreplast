namespace calidad_app.Models.Reportes;

/// <summary>
/// El detalle de calidad del periodo: cómo se comportó la detección, cómo se
/// están cerrando las no conformidades y hasta dónde llegó el flujo de
/// liberación.
///
/// Llega en una sola llamada (cal.usp_Kpi_Calidad) porque las cuatro partes se
/// leen juntas: un número alto de alertas solo se puede juzgar sabiendo si se
/// atendieron, y las no conformidades abiertas solo se entienden viendo en qué
/// estado están atoradas.
/// </summary>
public class ResumenCalidadKpi
{
    public AlertasKpi Alertas { get; set; } = new();
    public List<NoConformidadPorSeveridad> PorSeveridad { get; set; } = [];
    public List<NoConformidadPorEstado> PorEstado { get; set; } = [];
    public EmbudoLiberacion Embudo { get; set; } = new();
}

/// <summary>
/// Volumen y atención de las desviaciones detectadas contra la ficha técnica.
/// </summary>
public class AlertasKpi
{
    public int Total { get; set; }
    public int Pendientes { get; set; }
    public int Atendidas { get; set; }
    public int Criticas { get; set; }
    public int CriticasPendientes { get; set; }

    /// <summary>Alertas que terminaron en una no conformidad formal.</summary>
    public int ConNoConformidad { get; set; }

    public decimal? HorasAtencionPromedio { get; set; }
    public decimal? HorasAtencionMaxima { get; set; }

    /// <summary>Lo que lleva esperando la alerta sin atender más antigua.</summary>
    public decimal? HorasPendienteMasAntigua { get; set; }

    public decimal? PorcentajeAtendidas => Total > 0
        ? Math.Round(100m * Atendidas / Total, 1)
        : null;
}

/// <summary>No conformidades del periodo agrupadas por severidad, con su tiempo de cierre.</summary>
public class NoConformidadPorSeveridad
{
    public int SeveridadId { get; set; }
    public string SeveridadNombre { get; set; } = string.Empty;

    public int Total { get; set; }
    public int Abiertas { get; set; }
    public int Cerradas { get; set; }

    public decimal? DiasCierrePromedio { get; set; }

    /// <summary>Lo que llevan abiertas las que todavía no cierran.</summary>
    public decimal? DiasAbiertaPromedio { get; set; }
}

/// <summary>
/// Dónde se están quedando las no conformidades. Muchas en "En análisis" es un
/// problema distinto a muchas en "Registrada".
/// </summary>
public class NoConformidadPorEstado
{
    public int EstadoId { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool EsFinal { get; set; }
    public int Total { get; set; }
}

/// <summary>
/// El recorrido de liberación en números: de las corridas del periodo, cuántas
/// pasaron el despeje, cuántas llegaron al cierre, y de sus lotes cuántos
/// están liberados y cuántos certificados.
/// </summary>
public class EmbudoLiberacion
{
    public int Registros { get; set; }
    public int RegistrosConDespeje { get; set; }
    public int RegistrosConCierre { get; set; }

    public int Lotes { get; set; }
    public int LotesLiberados { get; set; }
    public int LotesCertificados { get; set; }

    public decimal? PorcentajeLiberados { get; set; }
    public decimal? PorcentajeCertificados { get; set; }

    /// <summary>Lotes ya liberados que siguen esperando su certificado.</summary>
    public int LotesPorCertificar => Math.Max(LotesLiberados - LotesCertificados, 0);
}
