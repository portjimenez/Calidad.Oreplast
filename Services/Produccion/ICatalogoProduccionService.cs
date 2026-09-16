using calidad_app.Models.Produccion;

namespace calidad_app.Services.Produccion;

/// <summary>
/// Listas que alimentan los filtros y formularios de las tres pantallas del
/// módulo. Una sola llamada al abrir la pantalla, igual que
/// <c>ICatalogoInspeccionService</c> en el módulo 2.
///
/// Solo devuelve elementos activos: estas pantallas programan y miden
/// producción, y ofrecer una máquina retirada o un operador dado de baja solo
/// lleva a un guardado rechazado.
/// </summary>
public interface ICatalogoProduccionService
{
    /// <summary>
    /// <paramref name="busquedaProducto"/> es obligatorio para recibir
    /// productos: son más de trescientos y crecen con el ERP, así que se
    /// buscan en lugar de listarse.
    /// </summary>
    Task<SelectoresProduccion> ObtenerSelectoresAsync(
        string? busquedaProducto = null, bool incluirInactivos = false,
        CancellationToken ct = default);
}
