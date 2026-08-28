using calidad_app.Models.Inspeccion;

namespace calidad_app.Services.Inspeccion;

/// <summary>
/// Producción por bobina: alta, corrección, confirmación y checklist de calidad.
///
/// Confirmar y retirar la confirmación son operaciones distintas y devuelven
/// cosas distintas —una el resultado de la evaluación de esa bobina, la otra el
/// listado actualizado—, por eso son dos métodos y no un parámetro booleano.
/// </summary>
public interface IBobinaService
{
    /// <summary>Bobinas del registro con sus acumulados (total, mínimo, máximo, promedio, desviación).</summary>
    Task<ProduccionPorBobina> ListarAsync(int registroId, CancellationToken ct = default);

    /// <summary>Da de alta o corrige una bobina. Sin BobinaId inserta y asigna el correlativo.</summary>
    Task<(int BobinaId, int IdBobi)> GuardarAsync(
        int registroId, BobinaEntrada bobina, CancellationToken ct = default);

    /// <summary>
    /// Cierra la captura de la bobina: evalúa sus mediciones contra la ficha,
    /// levanta las alertas que correspondan y calcula el indicador Ok.
    /// La bobina se confirma aunque Ok quede en false: el sistema registra lo
    /// que se produjo y deja la evidencia para que Calidad decida.
    /// </summary>
    Task<ConfirmacionBobina> ConfirmarAsync(int bobinaId, CancellationToken ct = default);

    /// <summary>Reabre una bobina confirmada para poder corregirla. Queda en bitácora.</summary>
    Task<ProduccionPorBobina> RetirarConfirmacionAsync(int bobinaId, CancellationToken ct = default);

    /// <summary>Solo procede si la bobina no está confirmada ni bloqueada.</summary>
    Task<ProduccionPorBobina> EliminarAsync(int bobinaId, CancellationToken ct = default);

    Task<List<ItemChecklist>> ObtenerChecklistAsync(int bobinaId, CancellationToken ct = default);

    Task<List<ItemChecklist>> GuardarChecklistAsync(
        int bobinaId, IEnumerable<RespuestaChecklist> respuestas, CancellationToken ct = default);
}
