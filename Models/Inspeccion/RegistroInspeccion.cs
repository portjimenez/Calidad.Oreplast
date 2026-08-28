namespace calidad_app.Models.Inspeccion;

/// <summary>
/// Fila del listado maestro de inspección (prod.usp_RegistroInspeccion_Listar).
/// Incluye los contadores que la base calcula al vuelo para pintar el semáforo
/// de cada fila sin una segunda consulta por registro.
/// </summary>
public class RegistroResumen
{
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public string EstadoOrden { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public DateTime? FechaHoraInicio { get; set; }
    public int TurnoId { get; set; }
    public string TurnoNombre { get; set; } = string.Empty;
    public int MaquinaId { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int? LineaId { get; set; }
    public string? LineaNombre { get; set; }
    public int OperadorId { get; set; }
    public string OperadorNombre { get; set; } = string.Empty;
    public int TipoRegistroId { get; set; }
    public string TipoRegistroCodigo { get; set; } = string.Empty;
    public string TipoRegistroNombre { get; set; } = string.Empty;
    public string FormatoCodigo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Bloqueado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int TotalBobinas { get; set; }
    public int BobinasConfirmadas { get; set; }
    public decimal PesoTotal { get; set; }
    public int ParametrosFueraDeRango { get; set; }
    public int AlertasPendientes { get; set; }

    /// <summary>Tipo de corrida según la letra del IdRegistro (…-P-001 = producción).</summary>
    public string Clasificacion =>
        IdRegistro.Split('-') is { Length: >= 3 } partes ? partes[^2] : string.Empty;
}

/// <summary>
/// Encabezado completo de un registro (prod.usp_RegistroInspeccion_Obtener,
/// primer resultado).
/// </summary>
public class RegistroDetalle
{
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public string EstadoOrden { get; set; } = string.Empty;
    public decimal? KgProgramados { get; set; }
    public int ClienteId { get; set; }
    public string ClienteCodigo { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public string? ProductoEstructura { get; set; }
    public DateOnly Fecha { get; set; }
    public DateTime? FechaHoraInicio { get; set; }
    public int TurnoId { get; set; }
    public string TurnoNombre { get; set; } = string.Empty;
    public int MaquinaId { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int? LineaId { get; set; }
    public string? LineaCodigo { get; set; }
    public string? LineaNombre { get; set; }
    public int OperadorId { get; set; }
    public string OperadorCodigo { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;
    public int TipoRegistroId { get; set; }
    public string TipoRegistroCodigo { get; set; } = string.Empty;
    public string TipoRegistroNombre { get; set; } = string.Empty;
    public int FormatoVersionId { get; set; }
    public string FormatoCodigo { get; set; } = string.Empty;
    public string FormatoVersion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Bloqueado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int? FichaId { get; set; }
    public string? FichaVersion { get; set; }
    public DateOnly? FichaVigenteDesde { get; set; }
    public bool DespejeLiberado { get; set; }
    public DateTime? DespejeFechaLiberacion { get; set; }
    public string? DespejeLiberadoPor { get; set; }

    /// <summary>Lotes a los que pertenecen las bobinas del registro (segundo resultado).</summary>
    public List<LoteResumen> Lotes { get; set; } = [];

    /// <summary>Un registro liberado, cerrado o bloqueado ya no admite cambios.</summary>
    public bool EsEditable =>
        !Bloqueado && Estado is not ("Liberado" or "Cerrado");

    public bool TieneFicha => FichaId is not null;
}

public class LoteResumen
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public DateOnly? FechaProduccion { get; set; }
    public decimal? PesoTotal { get; set; }
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// Filtros del listado maestro. Todos opcionales: null significa "no filtrar",
/// tal como los interpreta el procedimiento.
/// </summary>
public class FiltroRegistros
{
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public int? TurnoId { get; set; }
    public int? MaquinaId { get; set; }
    public int? AreaId { get; set; }
    public int? OperadorId { get; set; }
    public int? OrdenId { get; set; }
    public string? Estado { get; set; }
    public bool SoloAbiertos { get; set; }
    public string? Busqueda { get; set; }
    public int MaxFilas { get; set; } = 200;
}

/// <summary>Estados posibles de un registro de inspección (CK_Reg_Estado).</summary>
public static class EstadoRegistro
{
    public const string Pendiente = "Pendiente";
    public const string EnProceso = "EnProceso";
    public const string Liberado = "Liberado";
    public const string Cerrado = "Cerrado";
}
