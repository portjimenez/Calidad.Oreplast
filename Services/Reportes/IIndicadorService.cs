using calidad_app.Models.Reportes;

namespace calidad_app.Services.Reportes;

/// <summary>
/// El tablero de indicadores: cómo se comportó la planta en un periodo.
///
/// Es un servicio de SOLO LECTURA. Aquí no se captura nada: los datos los
/// levantan el módulo 2 (inspección, bobinas y mediciones contra la ficha), el
/// módulo 3 (alertas, no conformidades, liberación y certificados) y el módulo
/// 5 (desperdicio y paros); este módulo únicamente los mide y los pone uno
/// frente a otro.
///
/// Las cinco consultas se apoyan en la misma función de base
/// (prod.ufn_KpiRegistros), de modo que todas hablan del mismo conjunto de
/// corridas y los números cuadran entre pantallas. Si cada vista trajera su
/// propio cálculo, el mismo indicador podría dar distinto según dónde se mire,
/// que es lo que un tablero no se puede permitir.
/// </summary>
public interface IIndicadorService
{
    /// <summary>
    /// Las tarjetas del tablero: cada indicador con su valor, el del periodo
    /// anterior de igual longitud y la meta cuando existe; más los totales
    /// crudos del periodo como contexto.
    /// </summary>
    Task<TableroIndicadores> TableroAsync(
        FiltroIndicadores filtro, CancellationToken ct = default);

    /// <summary>
    /// La serie de tiempo del periodo, por día, semana o mes (ver
    /// <see cref="GranularidadTendencia"/>). Devuelve todos los periodos del
    /// rango, incluidos los que no tuvieron producción.
    /// </summary>
    Task<List<PuntoTendencia>> TendenciaAsync(
        FiltroIndicadores filtro, string granularidad, CancellationToken ct = default);

    /// <summary>
    /// El mismo periodo comparado entre líneas, máquinas, turnos, productos,
    /// clientes u operadores (ver <see cref="AgrupacionIndicadores"/>). La
    /// forma de la salida es la misma para las siete.
    /// </summary>
    Task<List<GrupoIndicador>> ComparativoAsync(
        FiltroIndicadores filtro, string agrupar, CancellationToken ct = default);

    /// <summary>
    /// El detalle de calidad: alertas y su atención, no conformidades por
    /// severidad y por estado, y el embudo de liberación y certificados.
    /// </summary>
    Task<ResumenCalidadKpi> CalidadAsync(
        FiltroIndicadores filtro, CancellationToken ct = default);

    /// <summary>
    /// Análisis de causas en forma de Pareto: qué pocas causas explican la
    /// mayor parte del problema (ver <see cref="OrigenPareto"/>).
    /// </summary>
    Task<List<CausaPareto>> ParetoAsync(
        FiltroIndicadores filtro, string origen, int maxFilas = 15,
        CancellationToken ct = default);
}
