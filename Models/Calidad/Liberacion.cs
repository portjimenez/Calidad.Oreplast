namespace calidad_app.Models.Calidad;

/// <summary>
/// Los dos tipos de liberación que admite un registro. El despeje autoriza a
/// arrancar; el cierre libera lo producido. Cada registro admite uno de cada
/// tipo como máximo (lo garantiza una restricción única en la base).
/// </summary>
public static class TipoLiberacion
{
    public const string DespejeLinea = "DespejeLinea";
    public const string CierreOrden = "CierreOrden";
}

/// <summary>Estado de liberación de un registro de inspección.</summary>
public class EstadoLiberacion
{
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Bloqueado { get; set; }
    public DateOnly Fecha { get; set; }

    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public string EstadoOrden { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string TurnoNombre { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;

    public int? LoteId { get; set; }
    public string? CodigoLote { get; set; }
    public string? EstadoLote { get; set; }

    public int TotalBobinas { get; set; }
    public decimal? PesoTotal { get; set; }
    public int BobinasNoConformes { get; set; }

    /// <summary>Siempre dos filas: despeje y cierre, firmadas o no.</summary>
    public List<FirmaLiberacion> Firmas { get; set; } = [];

    public CierreOrden? Cierre { get; set; }

    public FirmaLiberacion? Despeje =>
        Firmas.FirstOrDefault(f => f.Tipo == TipoLiberacion.DespejeLinea);

    public FirmaLiberacion? CierreFirma =>
        Firmas.FirstOrDefault(f => f.Tipo == TipoLiberacion.CierreOrden);

    /// <summary>
    /// El cierre no se firma sin el despeje: en el proceso real el despeje
    /// autoriza a arrancar y el cierre libera lo que se produjo.
    /// </summary>
    public bool PuedeFirmarCierre => Despeje?.Firmada == true && CierreFirma?.Firmada != true;

    public bool PuedeFirmarDespeje => Despeje?.Firmada != true;
}

/// <summary>Una de las dos firmas del registro.</summary>
public class FirmaLiberacion
{
    public string Tipo { get; set; } = string.Empty;
    public int? LiberacionId { get; set; }
    public bool Firmada { get; set; }
    public bool? CalidadVerificada { get; set; }
    public bool? InocuidadVerificada { get; set; }
    public int? LiberadoPorId { get; set; }

    /// <summary>
    /// Código de colaborador de quien firma. No es una columna de la
    /// liberación: sale de seg.Usuario a través de LiberadoPorId, que ya lo
    /// tiene normalizado.
    /// </summary>
    public string? CodigoQuienLibera { get; set; }

    public string? LiberadoPorNombre { get; set; }
    public DateTime? FechaLiberacion { get; set; }
    public int? LoteId { get; set; }
    public string? CodigoLote { get; set; }
    public bool TieneFirma { get; set; }

    public string TipoTexto => Tipo == TipoLiberacion.DespejeLinea
        ? "Despeje de línea"
        : "Cierre de orden";

    public string DescripcionTexto => Tipo == TipoLiberacion.DespejeLinea
        ? "Autoriza el arranque de la corrida."
        : "Libera el producto y cierra el registro.";
}

/// <summary>Sección Cierre de orden del formato.</summary>
public class CierreOrden
{
    public int CierreId { get; set; }
    public int RegistroId { get; set; }
    public string? Comentarios { get; set; }
    public decimal? KgProductoNoConforme { get; set; }
    public string? RazonNoConforme { get; set; }
}

/// <summary>Resultado de firmar una liberación.</summary>
public class ResultadoLiberacion
{
    public int LiberacionId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int? LoteId { get; set; }

    /// <summary>La orden se cerró porque ya no le quedaban registros abiertos.</summary>
    public bool OrdenCerrada { get; set; }

    /// <summary>
    /// Advertencias que el validador encontró pero que no impiden liberar.
    /// Se muestran al firmar: quien libera debe saber qué está firmando.
    /// </summary>
    public int AdvertenciasAlLiberar { get; set; }
}
