using calidad_app.Models.Catalogos;

namespace calidad_app.Services.Catalogos;

/// <summary>
/// Metas de desperdicio, material duro, refill y producción: los valores
/// contra los que se compara lo que la planta realmente gastó y produjo.
///
/// Va en su propio servicio y no con el resto de catálogos porque no se
/// comporta como ellos: una meta no se corrige, se sucede. Guardar una nueva
/// para un concepto y ámbito que ya tienen meta activa exige declararlo con
/// <c>reemplazarVigente</c>, y entonces la anterior queda inactiva pero se
/// conserva, porque el desperdicio del mes pasado se sigue midiendo contra la
/// meta que estaba vigente entonces.
/// </summary>
public interface IMetaProduccionService
{
    /// <summary>
    /// Metas registradas, de la más específica a la más general dentro de cada
    /// concepto. <c>soloVigentes</c> deja fuera las que todavía no entran en
    /// vigor, que sirven para dejar programado el cambio del próximo mes.
    /// </summary>
    Task<List<MetaProduccion>> ListarAsync(
        string? concepto = null, int? lineaId = null, int? maquinaId = null,
        int? productoId = null, bool soloActivos = false, bool soloVigentes = false,
        CancellationToken ct = default);

    /// <summary>
    /// Guarda la meta. Si ya hay una activa para el mismo concepto y ámbito, la
    /// base la rechaza salvo que <paramref name="reemplazarVigente"/> sea
    /// cierto, en cuyo caso desactiva la anterior en la misma operación.
    /// </summary>
    Task<int> GuardarAsync(
        MetaProduccion meta, bool reemplazarVigente = false, CancellationToken ct = default);

    Task CambiarEstadoAsync(int metaId, bool activo, CancellationToken ct = default);
}
