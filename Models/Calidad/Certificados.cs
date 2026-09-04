namespace calidad_app.Models.Calidad;

/// <summary>Certificado en el listado.</summary>
public class CertificadoResumen
{
    public int CertificadoId { get; set; }
    public string CertificadoCodigo { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string? Observaciones { get; set; }
    public int? EmitidoPorId { get; set; }
    public string? EmitidoPorNombre { get; set; }

    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public DateOnly? FechaProduccion { get; set; }

    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;

    public int TotalBobinas { get; set; }
    public decimal? PesoConforme { get; set; }
}

/// <summary>
/// El certificado completo, tal como se imprime.
///
/// El documento no afirma "el lote cumple": muestra, parámetro por parámetro,
/// contra qué se comparó y con qué resultado. La conclusión se sostiene sobre
/// los datos, y por eso todo lo que aparece aquí sale de la base y no se
/// calcula en la pantalla: dos impresiones del mismo certificado tienen que
/// decir exactamente lo mismo.
/// </summary>
public class CertificadoDetalle
{
    public int CertificadoId { get; set; }
    public string CertificadoCodigo { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string? Observaciones { get; set; }
    public int? EmitidoPorId { get; set; }
    public string? EmitidoPorNombre { get; set; }
    public string? EmitidoPorCodigo { get; set; }

    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public DateOnly? FechaProduccion { get; set; }
    public string EstadoLote { get; set; } = string.Empty;

    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public decimal? KgProgramados { get; set; }

    public int ClienteId { get; set; }
    public string ClienteCodigo { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public string? Estructura { get; set; }

    public int? FichaId { get; set; }
    public string? FichaVersion { get; set; }
    public DateOnly? FichaVigenteDesde { get; set; }

    public int TotalBobinas { get; set; }
    public int BobinasConformes { get; set; }
    public int BobinasSegregadas { get; set; }
    public decimal? PesoTotal { get; set; }
    public decimal? PesoConforme { get; set; }
    public decimal? MetrosTotal { get; set; }

    public List<ResultadoParametro> Resultados { get; set; } = [];
    public List<BobinaCertificada> BobinasEntregadas { get; set; } = [];
    public List<BobinaSegregada> BobinasSegregadasDetalle { get; set; } = [];
    public List<NoConformidadCertificada> NoConformidades { get; set; } = [];

    /// <summary>
    /// Un parámetro medido y fuera de tolerancia deja el lote como "cumple con
    /// observaciones": el documento no puede decir que todo salió bien si la
    /// tabla de resultados muestra lo contrario.
    /// </summary>
    public bool CumpleTodo => Resultados.All(r => r.Cumple != false);

    public int ParametrosEvaluados => Resultados.Count(r => r.Mediciones > 0);

    public int ParametrosFueraDeTolerancia => Resultados.Count(r => r.Cumple == false);
}

/// <summary>
/// Un parámetro de la ficha con el resumen estadístico de lo que se midió en el
/// lote.
/// </summary>
public class ResultadoParametro
{
    public int ParametroId { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }

    public decimal? ValorObjetivo { get; set; }
    public decimal? LimiteInferior { get; set; }
    public decimal? LimiteSuperior { get; set; }

    public int Mediciones { get; set; }
    public decimal? ValorMinimo { get; set; }
    public decimal? ValorMaximo { get; set; }
    public decimal? ValorPromedio { get; set; }
    public int FueraDeRango { get; set; }

    /// <summary>null cuando el parámetro no se midió: no hay contra qué concluir.</summary>
    public bool? Cumple { get; set; }

    public string RangoTexto => (LimiteInferior, LimiteSuperior) switch
    {
        (null, null) => "sin límites",
        (not null, null) => $"≥ {LimiteInferior:0.###}",
        (null, not null) => $"≤ {LimiteSuperior:0.###}",
        _ => $"{LimiteInferior:0.###} - {LimiteSuperior:0.###}"
    };

    public string ResultadoTexto => Cumple switch
    {
        true => "Conforme",
        false => $"{FueraDeRango} fuera",
        _ => "Sin medir"
    };
}

/// <summary>Bobina que se entrega al cliente.</summary>
public class BobinaCertificada
{
    public int BobinaId { get; set; }
    public int IdBobi { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public decimal? Peso { get; set; }
    public decimal? Metros { get; set; }
    public decimal? Ancho { get; set; }
    public decimal? Fuelle { get; set; }
    public decimal? Calibre { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
}

/// <summary>
/// Bobina separada del lote por una no conformidad. Aparece en el certificado a
/// propósito: un documento que omite lo que se segregó no es trazable.
/// </summary>
public class BobinaSegregada
{
    public int BobinaId { get; set; }
    public int IdBobi { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public decimal? Peso { get; set; }
    public decimal? Metros { get; set; }
    public string? NoConformidadCodigo { get; set; }
    public string? NoConformidadDescripcion { get; set; }
    public string? NoConformidadEstado { get; set; }
}

/// <summary>No conformidad del lote, ya resuelta, con su análisis.</summary>
public class NoConformidadCertificada
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? CausaRaiz { get; set; }
    public string? AccionCorrectiva { get; set; }
    public string SeveridadNombre { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}

/// <summary>Certificado recién emitido.</summary>
public class CertificadoEmitido
{
    public int CertificadoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int LoteId { get; set; }
}

/// <summary>Filtros del listado de certificados.</summary>
public class FiltroCertificados
{
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public int? ClienteId { get; set; }
    public int? ProductoId { get; set; }
    public int? OrdenId { get; set; }
    public int? LoteId { get; set; }
    public int? EmitidoPorId { get; set; }

    /// <summary>
    /// Busca por código de certificado, de lote o número de orden: el reclamo
    /// del cliente casi nunca llega con el número del certificado, llega con el
    /// código de lote impreso en la bobina.
    /// </summary>
    public string? Busqueda { get; set; }
}
