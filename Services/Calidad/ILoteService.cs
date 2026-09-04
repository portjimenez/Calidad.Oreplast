using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Lotes de producción: la unidad de liberación, trazabilidad y certificado.
///
/// El cliente no recibe "la orden", recibe un lote identificado del que se
/// puede decir con qué parámetros se produjo. Por eso el certificado cuelga del
/// lote, y por eso una bobina no puede estar en dos lotes: el reclamo llega con
/// el código impreso en la bobina y de ahí hay que llegar a mediciones,
/// máquina y operador sin ambigüedad.
/// </summary>
public interface ILoteService
{
    Task<List<LoteResumen>> ListarAsync(
        FiltroLotes filtro, int maxFilas = 200, CancellationToken ct = default);

    /// <summary>
    /// Expediente del lote: bobinas, registros de origen con su liberación, no
    /// conformidades relacionadas y alertas de proceso.
    /// </summary>
    Task<LoteDetalle?> ObtenerAsync(int loteId, CancellationToken ct = default);

    /// <summary>
    /// Abre un lote sobre una orden. El código lo genera la base con el formato
    /// que ya usa la planta; la pantalla no lo inventa.
    /// </summary>
    Task<LoteCreado> CrearAsync(
        int ordenId,
        DateOnly? fechaProduccion = null,
        string? codigoLote = null,
        CancellationToken ct = default);

    /// <summary>
    /// Bobinas que pueden entrar al lote: de la misma orden, confirmadas y sin
    /// lote. Las mismas condiciones que valida el guardado.
    /// </summary>
    Task<List<BobinaDisponible>> ListarBobinasDisponiblesAsync(
        int? loteId = null, int? ordenId = null, CancellationToken ct = default);

    /// <summary>Agrega bobinas al lote o las saca.</summary>
    Task<LoteDetalle?> AsignarBobinasAsync(
        int loteId,
        IReadOnlyCollection<int> bobinaIds,
        bool asignar = true,
        CancellationToken ct = default);
}
