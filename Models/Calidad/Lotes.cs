namespace calidad_app.Models.Calidad;

/// <summary>
/// Estados por los que pasa un lote. Se declaran como constantes porque la
/// pantalla decide con ellos qué acciones ofrecer, y una cadena suelta repartida
/// por varios componentes se desincroniza con el CHECK de la tabla sin que nadie
/// lo note.
/// </summary>
public static class EstadoLote
{
    /// <summary>Admite bobinas.</summary>
    public const string EnProceso = "EnProceso";

    /// <summary>Lo pone la liberación de cierre de orden. Listo para certificar.</summary>
    public const string Liberado = "Liberado";

    /// <summary>Lo pone la emisión del certificado. El lote ya no cambia.</summary>
    public const string Cerrado = "Cerrado";
}

/// <summary>Lote en la tabla maestra.</summary>
public class LoteResumen
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public string EstadoOrden { get; set; } = string.Empty;

    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;

    public DateOnly? FechaProduccion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }

    public int TotalBobinas { get; set; }
    public decimal? PesoTotal { get; set; }
    public int BobinasNoConformes { get; set; }
    public int NoConformidadesAbiertas { get; set; }

    public int? CertificadoId { get; set; }
    public string? CertificadoCodigo { get; set; }
    public DateTime? CertificadoFecha { get; set; }
    public bool TieneCertificado { get; set; }

    public bool AdmiteBobinas => Estado == EstadoLote.EnProceso;

    /// <summary>
    /// Un lote se puede certificar cuando está liberado, no tiene certificado y
    /// no le quedan no conformidades abiertas. La base vuelve a comprobarlo al
    /// emitir; esto solo evita ofrecer un botón que va a fallar.
    /// </summary>
    public bool PuedeCertificarse =>
        Estado == EstadoLote.Liberado && !TieneCertificado && NoConformidadesAbiertas == 0;
}

/// <summary>Expediente del lote: lo que sustenta la decisión de liberar y certificar.</summary>
public class LoteDetalle
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
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
    public string? Estructura { get; set; }

    public DateOnly? FechaProduccion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }

    public int TotalBobinas { get; set; }
    public decimal? PesoTotal { get; set; }
    public decimal? MetrosTotal { get; set; }
    public int BobinasNoConformes { get; set; }

    public int? CertificadoId { get; set; }
    public string? CertificadoCodigo { get; set; }
    public DateTime? CertificadoFecha { get; set; }
    public bool TieneCertificado { get; set; }

    public List<BobinaDeLote> Bobinas { get; set; } = [];
    public List<RegistroDeLote> Registros { get; set; } = [];
    public List<NoConformidadDeLote> NoConformidades { get; set; } = [];
    public List<AlertaDeNoConformidad> Alertas { get; set; } = [];

    public bool AdmiteBobinas => Estado == EstadoLote.EnProceso;

    public int NoConformidadesAbiertas => NoConformidades.Count(n => !n.EsFinal);

    public bool PuedeCertificarse =>
        Estado == EstadoLote.Liberado && !TieneCertificado && NoConformidadesAbiertas == 0;

    /// <summary>
    /// Por qué no se puede certificar todavía, en el orden en que hay que
    /// resolverlo. Se redacta aquí para que la pantalla explique en vez de
    /// limitarse a deshabilitar un botón.
    /// </summary>
    public string? MotivoNoCertificable =>
        TieneCertificado ? "El lote ya tiene certificado emitido."
        : Estado == EstadoLote.EnProceso ? "El lote todavía no ha sido liberado por Ingeniería de Calidad."
        : NoConformidadesAbiertas > 0 ? $"Hay {NoConformidadesAbiertas} no conformidad(es) abierta(s) sobre este lote."
        : TotalBobinas == 0 ? "El lote no tiene bobinas asignadas."
        : null;
}

/// <summary>Bobina agrupada en el lote.</summary>
public class BobinaDeLote
{
    public int BobinaId { get; set; }
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int IdBobi { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Metros { get; set; }
    public decimal? Ancho { get; set; }
    public decimal? Fuelle { get; set; }
    public decimal? Calibre { get; set; }

    /// <summary>Dato técnico del módulo 2: sus mediciones dieron dentro de rango.</summary>
    public bool Ok { get; set; }

    public bool Confirmada { get; set; }
    public DateTime? FechaConfirmacion { get; set; }

    /// <summary>Decisión de Calidad, distinta de <see cref="Ok"/>.</summary>
    public bool EsConforme { get; set; }

    public int? NoConformidadId { get; set; }
    public string? NoConformidadCodigo { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string TurnoNombre { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;
}

/// <summary>Bobina candidata a entrar en un lote (confirmada y todavía sin lote).</summary>
public class BobinaDisponible
{
    public int BobinaId { get; set; }
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int IdBobi { get; set; }
    public decimal? Peso { get; set; }
    public bool Ok { get; set; }
    public bool EsConforme { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
}

/// <summary>Registro de inspección del que salieron bobinas del lote.</summary>
public class RegistroDeLote
{
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool Bloqueado { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public string TurnoNombre { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;

    public int? LiberacionId { get; set; }
    public DateTime? FechaLiberacion { get; set; }
    public string? LiberadoPorNombre { get; set; }

    public bool Liberado => LiberacionId is not null;
}

/// <summary>No conformidad que toca al lote.</summary>
public class NoConformidadDeLote
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string SeveridadNombre { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = string.Empty;
    public bool EsFinal { get; set; }
    public DateTime FechaRegistro { get; set; }
}

/// <summary>Lote recién creado.</summary>
public class LoteCreado
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
}

/// <summary>Filtros de la tabla maestra de lotes.</summary>
public class FiltroLotes
{
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public int? OrdenId { get; set; }
    public int? ClienteId { get; set; }
    public int? ProductoId { get; set; }
    public string? Estado { get; set; }

    /// <summary>Los liberados que todavía esperan certificado: la cola de trabajo.</summary>
    public bool SoloSinCertificado { get; set; }

    public string? Busqueda { get; set; }
}
