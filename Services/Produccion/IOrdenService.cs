using calidad_app.Models.Produccion;

namespace calidad_app.Services.Produccion;

/// <summary>
/// Órdenes de producción: la pantalla de seguimiento del jefe de producción.
///
/// Es distinta del buscador de órdenes del módulo 2: aquel es un
/// autocompletado para que el operador encuentre SU orden; esta responde
/// cuánto lleva avanzada cada orden y permite abrirlas y corregirlas.
///
/// La escritura exige el permiso GESTIONAR_ORDENES, que la base comprueba en
/// cada procedimiento: la pantalla nunca envía el usuario, lo resuelve el
/// servicio a partir de la identidad autenticada.
///
/// El ESTADO de la orden no se administra desde aquí en el caso normal: lo
/// mueve solo el sistema (a EnProceso al abrir el primer registro de
/// inspección, a Cerrada cuando Calidad firma el cierre del último). Solo
/// <see cref="CambiarEstadoAsync"/> cubre las dos excepciones: el cierre
/// anticipado y la reapertura.
/// </summary>
public interface IOrdenService
{
    Task<List<OrdenResumen>> ListarAsync(
        FiltroOrdenes filtro, int maxFilas = 200, CancellationToken ct = default);

    /// <summary>
    /// Indicadores del encabezado. Se piden aparte de la lista porque cuentan
    /// TODAS las órdenes del periodo, mientras que la lista viene recortada
    /// por <c>maxFilas</c>.
    /// </summary>
    Task<ResumenOrdenes> ResumenAsync(FiltroOrdenes filtro, CancellationToken ct = default);

    /// <summary>
    /// Expediente completo: la cadena orden → registros → bobinas → lotes →
    /// certificado, más las no conformidades. Null si la orden no existe.
    /// </summary>
    Task<OrdenDetalle?> ObtenerAsync(int ordenId, CancellationToken ct = default);

    /// <summary>
    /// Alta o modificación. Devuelve el id de la orden (el que ya tenía o el
    /// recién generado).
    /// </summary>
    Task<int> GuardarAsync(OrdenEdicion orden, CancellationToken ct = default);

    /// <summary>
    /// Cierre anticipado o reapertura. El motivo es obligatorio al reabrir una
    /// orden cerrada: es un movimiento excepcional y queda en la bitácora para
    /// poder explicarse en una auditoría.
    /// </summary>
    Task CambiarEstadoAsync(
        int ordenId, string estado, string? motivo = null, CancellationToken ct = default);
}
