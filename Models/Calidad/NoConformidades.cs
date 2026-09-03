namespace calidad_app.Models.Calidad;

/// <summary>
/// No conformidad en la tabla maestra.
///
/// El listado y el expediente tienen forma distinta a propósito: el listado
/// trae contadores calculados (evidencias, bobinas, alertas) que el expediente
/// no necesita porque devuelve las listas completas, y el expediente trae los
/// textos largos que la tabla no muestra. Por eso son dos clases y no una con
/// herencia: atarlas obligaría a que los dos procedimientos devolvieran las
/// mismas columnas para siempre.
/// </summary>
public class NoConformidadResumen
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;

    public int? RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int? OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;

    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int TipoDefectoId { get; set; }
    public string TipoDefectoNombre { get; set; } = string.Empty;
    public int SeveridadId { get; set; }
    public string SeveridadNombre { get; set; } = string.Empty;

    public int EstadoActualId { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public int EstadoOrden { get; set; }
    public bool EsFinal { get; set; }

    public string Descripcion { get; set; } = string.Empty;
    public int? ResponsableId { get; set; }
    public string? ResponsableNombre { get; set; }
    public int RegistradaPorId { get; set; }
    public string RegistradaPorNombre { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public int DiasAbierta { get; set; }

    /// <summary>
    /// Sin causa raíz o sin acción correctiva la NC no se puede cerrar. Se
    /// devuelven como banderas para que la tabla marque las que están
    /// incompletas sin arrastrar los dos textos largos.
    /// </summary>
    public bool TieneCausaRaiz { get; set; }

    public bool TieneAccionCorrectiva { get; set; }

    public int TotalEvidencias { get; set; }
    public int BobinasAfectadas { get; set; }
    public int AlertasVinculadas { get; set; }

    /// <summary>Lo que le falta para poder cerrarse; vacío si ya está completa.</summary>
    public bool ListaParaCerrar => TieneCausaRaiz && TieneAccionCorrectiva;
}

/// <summary>Expediente completo de una no conformidad.</summary>
public class NoConformidadDetalle
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;

    public int? RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public DateOnly? FechaRegistroInspeccion { get; set; }
    public int? OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public int? ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int? ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;

    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int TipoDefectoId { get; set; }
    public string TipoDefectoNombre { get; set; } = string.Empty;
    public int SeveridadId { get; set; }
    public string SeveridadNombre { get; set; } = string.Empty;

    public int EstadoActualId { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public int EstadoOrden { get; set; }
    public bool EsFinal { get; set; }

    public string Descripcion { get; set; } = string.Empty;
    public string? CausaRaiz { get; set; }
    public string? AccionCorrectiva { get; set; }

    public int? ResponsableId { get; set; }
    public string? ResponsableNombre { get; set; }
    public int RegistradaPorId { get; set; }
    public string RegistradaPorNombre { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public int DiasAbierta { get; set; }

    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public string TurnoNombre { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;

    public List<MovimientoEstadoNc> Historial { get; set; } = [];
    public List<EvidenciaNc> Evidencias { get; set; } = [];
    public List<BobinaAfectada> Bobinas { get; set; } = [];
    public List<AlertaDeNoConformidad> Alertas { get; set; } = [];

    /// <summary>
    /// Estados a los que puede moverse desde donde está. Los calcula el mismo
    /// procedimiento que valida el cambio, así que la pantalla no reimplementa
    /// la regla de transiciones: solo ofrece lo que la base va a aceptar.
    /// </summary>
    public List<EstadoNcDisponible> EstadosDisponibles { get; set; } = [];

    /// <summary>Una NC cerrada o anulada es evidencia: ya no se edita.</summary>
    public bool EsEditable => !EsFinal;

    public bool PuedeCerrarse =>
        !string.IsNullOrWhiteSpace(CausaRaiz) && !string.IsNullOrWhiteSpace(AccionCorrectiva);
}

/// <summary>Un paso del historial de estados: quién la movió, cuándo y por qué.</summary>
public class MovimientoEstadoNc
{
    public int HistorialId { get; set; }
    public int EstadoId { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public bool EsFinal { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaCambio { get; set; }
    public string? Observacion { get; set; }
}

/// <summary>
/// Evidencia adjunta. El binario no viaja en el listado: solo sus metadatos,
/// para que abrir el expediente no arrastre fotos que quizá nadie abra.
/// </summary>
public class EvidenciaNc
{
    public int EvidenciaId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string? Ruta { get; set; }
    public DateTime FechaCarga { get; set; }
    public int? TamanoBytes { get; set; }
    public bool TieneArchivo { get; set; }

    public string TamanoTexto => TamanoBytes switch
    {
        null => "referencia externa",
        < 1024 => $"{TamanoBytes} B",
        < 1024 * 1024 => $"{TamanoBytes / 1024d:0.#} KB",
        _ => $"{TamanoBytes / (1024d * 1024d):0.#} MB"
    };
}

/// <summary>Bobina marcada como no conforme por esta NC.</summary>
public class BobinaAfectada
{
    public int BobinaId { get; set; }
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int IdBobi { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Metros { get; set; }
    public bool EsConforme { get; set; }
    public int? LoteId { get; set; }
    public string? CodigoLote { get; set; }
}

/// <summary>Alerta de proceso escalada a esta no conformidad.</summary>
public class AlertaDeNoConformidad
{
    public int AlertaId { get; set; }
    public int? RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public int? BobinaId { get; set; }
    public int? IdBobi { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }
    public decimal? ValorRegistrado { get; set; }
    public decimal? LimiteInferior { get; set; }
    public decimal? LimiteSuperior { get; set; }
    public DateTime FechaDeteccion { get; set; }
    public bool Atendida { get; set; }

    public string OrigenTexto => BobinaId is null ? "Corrida" : $"Bobina {IdBobi}";
}

/// <summary>Estado al que la NC puede pasar desde donde está.</summary>
public class EstadoNcDisponible
{
    public int EstadoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool EsFinal { get; set; }
}

/// <summary>Filtros de la tabla maestra. Todos opcionales.</summary>
public class FiltroNoConformidades
{
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public int? AreaId { get; set; }
    public int? SeveridadId { get; set; }
    public int? EstadoId { get; set; }
    public int? TipoDefectoId { get; set; }
    public int? ResponsableId { get; set; }
    public int? RegistroId { get; set; }
    public int? OrdenId { get; set; }

    /// <summary>Excluye los estados finales. La pantalla arranca en true.</summary>
    public bool SoloAbiertas { get; set; } = true;

    public string? Busqueda { get; set; }
}

/// <summary>Datos para levantar una no conformidad.</summary>
public class NuevaNoConformidad
{
    public int TipoDefectoId { get; set; }
    public int SeveridadId { get; set; }
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>En null se toma del registro de inspección, si se indicó uno.</summary>
    public int? AreaId { get; set; }

    public int? RegistroId { get; set; }
    public int? OrdenId { get; set; }
    public int? ResponsableId { get; set; }
    public string? CausaRaiz { get; set; }
    public string? AccionCorrectiva { get; set; }

    /// <summary>Alertas que se escalan a esta NC en el mismo acto.</summary>
    public List<int> AlertaIds { get; set; } = [];

    /// <summary>Bobinas que quedan marcadas como no conformes.</summary>
    public List<int> BobinaIds { get; set; } = [];
}

/// <summary>Identificación de la NC recién creada.</summary>
public class NoConformidadCreada
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;
}

/// <summary>Archivo de evidencia con su contenido, para descargarlo.</summary>
public class ArchivoEvidencia
{
    public int EvidenciaId { get; set; }
    public int NoConformidadId { get; set; }
    public string NoConformidadCodigo { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string? Ruta { get; set; }
    public byte[]? Contenido { get; set; }
    public DateTime FechaCarga { get; set; }
    public int? TamanoBytes { get; set; }
}

/// <summary>Severidad del hallazgo (Crítica, Mayor, Menor).</summary>
public class SeveridadCatalogo
{
    public int SeveridadId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>Estado del ciclo de vida de una NC.</summary>
public class EstadoNcCatalogo
{
    public int EstadoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool EsFinal { get; set; }
}

/// <summary>Tipo de defecto. Con AreaId en null aplica a cualquier área.</summary>
public class TipoDefectoCatalogo
{
    public int TipoDefectoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }

    public string Grupo => AreaNombre ?? "Todas las áreas";
}

/// <summary>Área de la planta.</summary>
public class AreaCatalogo
{
    public int AreaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>Usuario que puede figurar como responsable de la acción correctiva.</summary>
public class ResponsableCatalogo
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string RolNombre { get; set; } = string.Empty;
    public int? AreaId { get; set; }

    public string Etiqueta => $"{NombreCompleto} ({RolNombre})";
}

/// <summary>
/// Los cinco catálogos del formulario, en una sola llamada: abrirlo dispara
/// una consulta, no cinco.
/// </summary>
public class CatalogosNoConformidad
{
    public List<SeveridadCatalogo> Severidades { get; set; } = [];
    public List<EstadoNcCatalogo> Estados { get; set; } = [];
    public List<TipoDefectoCatalogo> TiposDefecto { get; set; } = [];
    public List<AreaCatalogo> Areas { get; set; } = [];
    public List<ResponsableCatalogo> Responsables { get; set; } = [];

    /// <summary>Tipos de defecto aplicables al área: los suyos y los transversales.</summary>
    public IEnumerable<TipoDefectoCatalogo> TiposDe(int? areaId) =>
        TiposDefecto.Where(t => t.AreaId is null || areaId is null || t.AreaId == areaId);
}
