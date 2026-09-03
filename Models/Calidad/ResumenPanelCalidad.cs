namespace calidad_app.Models.Calidad;

/// <summary>
/// Lo que Ingeniería de Calidad tiene pendiente ahora mismo. Es la pantalla de
/// entrada del módulo y se arma con una sola llamada
/// (cal.usp_PanelCalidad_Resumen), que devuelve las tarjetas y las tres colas
/// de trabajo en el orden en que se atienden.
/// </summary>
public class ResumenPanelCalidad
{
    public TarjetasPanel Tarjetas { get; set; } = new();
    public List<AlertaResumen> AlertasUrgentes { get; set; } = [];
    public List<NoConformidadPendiente> NoConformidades { get; set; } = [];
    public List<LotePorCertificar> LotesPorCertificar { get; set; } = [];
}

/// <summary>Contadores de las tarjetas del panel.</summary>
public class TarjetasPanel
{
    public int AlertasPendientes { get; set; }
    public int AlertasCriticasPendientes { get; set; }
    public int NoConformidadesAbiertas { get; set; }
    public int NoConformidadesCriticas { get; set; }
    public int LotesPorCertificar { get; set; }
    public int RegistrosEnProceso { get; set; }
}

/// <summary>No conformidad abierta, tal como aparece en la cola del panel.</summary>
public class NoConformidadPendiente
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string SeveridadNombre { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = string.Empty;
    public string AreaNombre { get; set; } = string.Empty;
    public string TipoDefectoNombre { get; set; } = string.Empty;
    public string? ResponsableNombre { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int DiasAbierta { get; set; }
}

/// <summary>Lote ya liberado que todavía espera su certificado de calidad.</summary>
public class LotePorCertificar
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public string NumeroOP { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public DateOnly? FechaProduccion { get; set; }
    public int TotalBobinas { get; set; }
    public decimal? PesoTotal { get; set; }
}
