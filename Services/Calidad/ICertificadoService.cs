using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Certificados de calidad por lote: el documento que sale de la planta con el
/// producto.
///
/// La base solo lo emite cuando el lote está liberado, tiene bobinas y no le
/// quedan no conformidades abiertas, y exige el permiso GENERAR_CERTIFICADO.
/// Certificar mientras se investiga un defecto sería afirmar por escrito algo
/// que todavía no se sabe.
/// </summary>
public interface ICertificadoService
{
    Task<List<CertificadoResumen>> ListarAsync(
        FiltroCertificados filtro, int maxFilas = 200, CancellationToken ct = default);

    /// <summary>
    /// El certificado completo, tal como se imprime. Se puede pedir por su id o
    /// por el lote.
    /// </summary>
    Task<CertificadoDetalle?> ObtenerAsync(
        int? certificadoId = null, int? loteId = null, CancellationToken ct = default);

    /// <summary>Emite el certificado del lote. El código lo genera la base.</summary>
    Task<CertificadoEmitido> EmitirAsync(
        int loteId, string? observaciones = null, CancellationToken ct = default);
}
