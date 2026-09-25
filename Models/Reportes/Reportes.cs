namespace calidad_app.Models.Reportes;

/// <summary>
/// Los cuatro reportes que se pueden generar. Cada uno responde a una
/// pregunta distinta y por eso tiene sus propias columnas y sus propios
/// filtros; lo que comparten es el periodo, el permiso (GENERAR_REPORTES) y la
/// forma de exportarse.
/// </summary>
public static class ClavesReporte
{
    public const string Produccion = "Produccion";
    public const string NoConformidades = "NoConformidades";
    public const string Alertas = "Alertas";
    public const string Lotes = "Lotes";

    public static readonly DefinicionReporte[] Catalogo =
    [
        new(Produccion, "Producción e inspección",
            "Una fila por corrida: kilos, desperdicio, cumplimiento de ficha, paros y estado de liberación."),

        new(Alertas, "Desviaciones contra la ficha técnica",
            "Cada desviación detectada en proceso, con el valor medido, el límite que se pasó y quién la atendió."),

        new(NoConformidades, "No conformidades",
            "Las no conformidades del periodo con su severidad, estado, causa raíz y días de atención."),

        new(Lotes, "Lotes, liberación y certificados",
            "Los lotes del periodo con sus bobinas, quién los liberó y si ya tienen certificado de calidad.")
    ];

    public static DefinicionReporte Definicion(string clave) =>
        Catalogo.FirstOrDefault(r => r.Clave == clave) ?? Catalogo[0];
}

/// <summary>Un reporte del catálogo: qué es y para qué sirve.</summary>
public record DefinicionReporte(string Clave, string Nombre, string Descripcion);

/// <summary>
/// Filtros del centro de reportes. Parte del mismo ámbito del tablero y le
/// agrega lo que solo tiene sentido en un reporte concreto (una severidad, un
/// parámetro, el estado de un lote).
///
/// Cada reporte usa lo que le aplica y pasa null en lo demás: los
/// procedimientos ignoran los filtros en null.
/// </summary>
public class FiltroReporte : FiltroIndicadores
{
    public int? SeveridadId { get; set; }
    public int? ParametroId { get; set; }
    public string? EstadoLote { get; set; }

    public bool SoloAbiertas { get; set; }
    public bool SoloPendientes { get; set; }
    public bool SoloCriticas { get; set; }
    public bool SoloSinCertificado { get; set; }

    /// <summary>
    /// Tope de filas. Existe para que un rango largo no intente traer decenas
    /// de miles de corridas al servidor web; cuando el reporte llega completo
    /// hasta el tope, la pantalla avisa que quedó recortado.
    /// </summary>
    public int MaxFilas { get; set; } = 5000;
}

/// <summary>Formato del archivo que se descarga.</summary>
public enum FormatoArchivo
{
    Excel,
    Pdf
}

/// <summary>
/// El archivo ya generado, listo para bajar por el circuito con
/// wwwroot/js/descargas.js.
/// </summary>
public record ArchivoGenerado(string NombreArchivo, string TipoContenido, byte[] Contenido)
{
    public int Bytes => Contenido.Length;
}

/// <summary>
/// Una fila del reporte de producción e inspección: una corrida con todo lo
/// que se midió en ella.
/// </summary>
public class FilaProduccion
{
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;

    public string Turno { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Linea { get; set; } = string.Empty;
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public string OperadorCodigo { get; set; } = string.Empty;
    public string Operador { get; set; } = string.Empty;

    public string NumeroOP { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;

    public int Bobinas { get; set; }
    public int BobinasConformes { get; set; }
    public int BobinasNoConformes { get; set; }

    public decimal KgProducidos { get; set; }
    public decimal KgDesperdicioSetup { get; set; }
    public decimal KgDesperdicioProduccion { get; set; }
    public decimal KgDesperdicio { get; set; }
    public decimal KgDuro { get; set; }
    public decimal KgRefill { get; set; }
    public decimal KgProcesado { get; set; }
    public decimal? PorcentajeDesperdicio { get; set; }

    public decimal HorasSetup { get; set; }
    public decimal HorasProduccion { get; set; }
    public int TiempoMuertoMin { get; set; }

    public int Mediciones { get; set; }
    public int MedicionesEnRango { get; set; }
    public int MedicionesFueraRango { get; set; }
    public decimal? PorcentajeFichaTecnica { get; set; }

    public int Alertas { get; set; }
    public int AlertasPendientes { get; set; }
    public int NoConformidades { get; set; }

    public bool ConDespeje { get; set; }
    public bool ConCierre { get; set; }
}

/// <summary>Una fila del reporte de no conformidades.</summary>
public class FilaNoConformidad
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }

    public string Severidad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool EsFinal { get; set; }
    public string TipoDefecto { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
    public string? CausaRaiz { get; set; }
    public string? AccionCorrectiva { get; set; }

    public string? IdRegistro { get; set; }
    public string? NumeroOP { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string Turno { get; set; } = string.Empty;
    public string Linea { get; set; } = string.Empty;

    public string RegistradaPor { get; set; } = string.Empty;
    public string? Responsable { get; set; }

    public DateTime? FechaCierre { get; set; }

    /// <summary>Hasta el cierre si ya cerró; hasta hoy si sigue abierta.</summary>
    public decimal DiasDeAtencion { get; set; }

    public int BobinasVinculadas { get; set; }
    public decimal KgVinculados { get; set; }
}

/// <summary>
/// Una fila del reporte de desviaciones: la evidencia de que el sistema
/// detecta la variación mientras la corrida está andando.
/// </summary>
public class FilaAlerta
{
    public int AlertaId { get; set; }
    public DateTime FechaDeteccion { get; set; }

    public string ParametroCodigo { get; set; } = string.Empty;
    public string Parametro { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }

    public decimal? ValorRegistrado { get; set; }
    public decimal? LimiteInferior { get; set; }
    public decimal? LimiteSuperior { get; set; }

    /// <summary>Cuánto se pasó del límite que violó. Positiva por arriba, negativa por abajo.</summary>
    public decimal? Desviacion { get; set; }

    public string? IdRegistro { get; set; }
    public string? NumeroOP { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public int? Bobina { get; set; }

    public string MaquinaCodigo { get; set; } = string.Empty;
    public string Linea { get; set; } = string.Empty;
    public string Turno { get; set; } = string.Empty;
    public string Operador { get; set; } = string.Empty;

    public bool Atendida { get; set; }
    public DateTime? FechaAtencion { get; set; }
    public string AtendidaPor { get; set; } = string.Empty;
    public decimal HorasHastaAtencion { get; set; }
    public string? Observacion { get; set; }
    public string? NoConformidad { get; set; }
}

/// <summary>Una fila del reporte de lotes, liberación y certificados.</summary>
public class FilaLote
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;

    public string NumeroOP { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;

    public int Bobinas { get; set; }
    public int BobinasConformes { get; set; }
    public decimal KgProducidos { get; set; }
    public int Maquinas { get; set; }

    public DateTime? FechaLiberacion { get; set; }
    public string LiberadoPor { get; set; } = string.Empty;

    public string? CodigoCertificado { get; set; }
    public DateTime? FechaCertificado { get; set; }
    public string CertificadoPor { get; set; } = string.Empty;
    public bool TieneCertificado { get; set; }

    public int NoConformidades { get; set; }
}

/// <summary>
/// Un reporte ya convertido en tabla, listo para exportarse.
///
/// Los cuatro reportes pasan por aquí antes de volverse Excel o PDF: así el
/// exportador se escribe una sola vez y no hay cuatro versiones del mismo
/// código de formato. El servicio de cada reporte decide las columnas y sus
/// tipos; el exportador solo sabe de tipos, no de reportes.
/// </summary>
public class TablaReporte
{
    public string Titulo { get; set; } = string.Empty;

    /// <summary>El periodo y los filtros aplicados, para que el archivo se explique solo.</summary>
    public string Subtitulo { get; set; } = string.Empty;

    public List<ColumnaReporte> Columnas { get; set; } = [];
    public List<object?[]> Filas { get; set; } = [];

    /// <summary>Aviso al pie, por ejemplo cuando el reporte se recortó por el tope de filas.</summary>
    public string? Nota { get; set; }
}

/// <summary>
/// Una columna del reporte. El tipo no es decorativo: decide el formato de
/// número en Excel (para que las cifras sigan siendo números y se puedan
/// sumar) y la alineación en el PDF.
/// </summary>
public record ColumnaReporte(string Titulo, TipoColumna Tipo = TipoColumna.Texto, double AnchoCm = 2.5);

public enum TipoColumna
{
    Texto,
    Entero,
    Decimal,
    Porcentaje,
    Fecha,
    FechaHora,
    Booleano
}

/// <summary>
/// Cómo se escribe una celda según el tipo de su columna.
///
/// Vive aquí y no en cada pantalla para que el mismo dato se lea igual en la
/// vista previa, en el PDF y en el Excel. En el Excel, además, los valores
/// viajan como números de verdad (no como texto) y este formato solo decide
/// cómo se muestran, de modo que quien recibe el archivo puede sumarlos y
/// graficarlos.
/// </summary>
public static class FormatoCelda
{
    public static string Texto(object? valor, TipoColumna tipo) => valor switch
    {
        null => "—",
        bool booleano => booleano ? "Sí" : "No",
        DateOnly fecha => fecha.ToString("dd/MM/yyyy"),
        DateTime fechaHora when tipo == TipoColumna.Fecha => fechaHora.ToString("dd/MM/yyyy"),
        DateTime fechaHora => fechaHora.ToString("dd/MM/yyyy HH:mm"),
        decimal numero when tipo == TipoColumna.Porcentaje => $"{numero:0.#} %",
        decimal numero when tipo == TipoColumna.Entero => numero.ToString("N0"),
        decimal numero => numero.ToString("N2"),
        int entero => entero.ToString("N0"),
        _ => valor.ToString() ?? string.Empty
    };

    /// <summary>Las columnas numéricas y de fecha se alinean a la derecha.</summary>
    public static bool AlineadaDerecha(TipoColumna tipo) => tipo is
        TipoColumna.Entero or TipoColumna.Decimal or TipoColumna.Porcentaje
        or TipoColumna.Fecha or TipoColumna.FechaHora;

    /// <summary>Formato de número de Excel para cada tipo de columna.</summary>
    public static string? FormatoExcel(TipoColumna tipo) => tipo switch
    {
        TipoColumna.Entero => "#,##0",
        TipoColumna.Decimal => "#,##0.00",
        TipoColumna.Porcentaje => "#,##0.0",
        TipoColumna.Fecha => "dd/mm/yyyy",
        TipoColumna.FechaHora => "dd/mm/yyyy hh:mm",
        _ => null
    };
}
