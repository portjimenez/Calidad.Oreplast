using calidad_app.Models.Produccion;

namespace calidad_app.Services.Produccion;

/// <summary>
/// Control de desperdicio contra las metas, y el tiempo muerto que casi
/// siempre lo explica.
///
/// Es un servicio de solo lectura: el desperdicio no se captura aquí, se
/// captura en la inspección en proceso (módulo 2) junto con el setup y la
/// producción, y las metas se administran en catálogos (módulo 4). Esta
/// pantalla solo compara lo uno contra lo otro.
///
/// El porcentaje se mide sobre el MATERIAL PROCESADO (producido más
/// desperdiciado) y no sobre lo producido, para que el indicador siempre vaya
/// de 0 a 100 y se pueda comparar entre máquinas.
/// </summary>
public interface IDesperdicioService
{
    /// <summary>
    /// Totales del periodo y las cuatro metas del ámbito consultado, con su
    /// valor real y si se cumplen.
    /// </summary>
    Task<ResumenDesperdicio> ResumenAsync(
        FiltroDesperdicio filtro, CancellationToken ct = default);

    /// <summary>
    /// Detalle corrida por corrida, cada una con SU meta: la resuelve la base
    /// según la línea, la máquina y el producto de ese registro y la fecha en
    /// que se corrió, de modo que una corrida vieja se juzga con la meta que
    /// estaba vigente entonces.
    /// </summary>
    Task<List<RegistroDesperdicio>> ListarAsync(
        FiltroDesperdicio filtro, bool soloIncumplen = false, int maxFilas = 500,
        CancellationToken ct = default);

    /// <summary>
    /// El mismo periodo sumado por una dimensión (ver
    /// <see cref="AgrupacionDesperdicio"/>). La forma de la salida es la misma
    /// para las seis, así que la tabla y la gráfica no cambian con el corte.
    /// </summary>
    Task<List<GrupoDesperdicio>> AgruparAsync(
        FiltroDesperdicio filtro, string agrupar, CancellationToken ct = default);

    /// <summary>Minutos de paro por razón, ordenados y con su acumulado (Pareto).</summary>
    Task<List<ParoPorRazon>> TiempoMuertoAsync(
        FiltroDesperdicio filtro, CancellationToken ct = default);
}
