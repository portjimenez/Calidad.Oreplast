using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// No conformidades: el registro formal de un hallazgo de calidad y su
/// seguimiento hasta el cierre.
///
/// Todas las reglas duras viven en la base: el correlativo del código, las
/// transiciones de estado permitidas, la exigencia de causa raíz y acción
/// correctiva antes de cerrar, y que una NC en estado final ya no se toca.
/// Este servicio no las repite; solo traduce entre la pantalla y los
/// procedimientos.
/// </summary>
public interface INoConformidadService
{
    /// <summary>Tabla maestra. Los filtros en null no filtran.</summary>
    Task<List<NoConformidadResumen>> ListarAsync(
        FiltroNoConformidades filtro, int maxFilas = 200, CancellationToken ct = default);

    /// <summary>
    /// Expediente completo: encabezado, historial, evidencias, bobinas
    /// afectadas, alertas vinculadas y los estados a los que puede pasar.
    /// </summary>
    Task<NoConformidadDetalle?> ObtenerAsync(int noConformidadId, CancellationToken ct = default);

    /// <summary>Los cinco catálogos del formulario en una sola llamada.</summary>
    Task<CatalogosNoConformidad> ObtenerCatalogosAsync(
        int? areaId = null, CancellationToken ct = default);

    /// <summary>
    /// Levanta la no conformidad. El código (NC-año-correlativo) y el estado
    /// inicial los pone la base, no la pantalla.
    /// </summary>
    Task<NoConformidadCreada> CrearAsync(NuevaNoConformidad nueva, CancellationToken ct = default);

    /// <summary>
    /// Edita el contenido de una NC abierta. Los parámetros en null significan
    /// "no cambiar"; una cadena vacía en causa raíz o acción correctiva las
    /// vacía. El estado no se cambia aquí.
    /// </summary>
    Task<NoConformidadDetalle?> ActualizarAsync(
        int noConformidadId,
        string? descripcion = null,
        string? causaRaiz = null,
        string? accionCorrectiva = null,
        int? responsableId = null,
        int? severidadId = null,
        int? tipoDefectoId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Mueve la NC de estado y deja el movimiento en el historial. La base
    /// rechaza las transiciones no permitidas y los cierres sin análisis.
    /// </summary>
    Task<NoConformidadDetalle?> CambiarEstadoAsync(
        int noConformidadId,
        int estadoId,
        string? observacion = null,
        CancellationToken ct = default);

    /// <summary>
    /// Marca bobinas como afectadas (o las libera). Es aquí donde se decide la
    /// conformidad del producto, no en la confirmación de la bobina.
    /// </summary>
    Task<NoConformidadDetalle?> VincularBobinasAsync(
        int noConformidadId,
        IReadOnlyCollection<int> bobinaIds,
        bool vincular = true,
        CancellationToken ct = default);

    /// <summary>Adjunta una evidencia (normalmente la foto del defecto).</summary>
    Task<EvidenciaNc?> GuardarEvidenciaAsync(
        int noConformidadId,
        string nombreArchivo,
        byte[] contenido,
        CancellationToken ct = default);

    /// <summary>Contenido de una evidencia, para descargarla. Queda en bitácora.</summary>
    Task<ArchivoEvidencia?> ObtenerArchivoEvidenciaAsync(
        int evidenciaId, CancellationToken ct = default);

    /// <summary>Quita una evidencia adjuntada por error, si la NC sigue abierta.</summary>
    Task EliminarEvidenciaAsync(int evidenciaId, CancellationToken ct = default);
}
