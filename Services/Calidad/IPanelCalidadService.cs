using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Pantalla de entrada del módulo: qué tiene Calidad pendiente ahora.
/// </summary>
public interface IPanelCalidadService
{
    /// <summary>
    /// Tarjetas y colas de trabajo en una sola llamada.
    ///
    /// El rango de fechas acota alertas y no conformidades, no las colas de
    /// liberación y certificado: un lote liberado la semana pasada sigue
    /// esperando su certificado hoy y no debe desaparecer del panel porque el
    /// filtro apunte a esta semana.
    /// </summary>
    Task<ResumenPanelCalidad> ObtenerAsync(
        DateOnly? desde = null,
        DateOnly? hasta = null,
        int? areaId = null,
        int maxFilas = 10,
        CancellationToken ct = default);
}
