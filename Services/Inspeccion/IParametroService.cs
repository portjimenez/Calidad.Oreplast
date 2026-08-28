using calidad_app.Models.Inspeccion;

namespace calidad_app.Services.Inspeccion;

/// <summary>
/// Mediciones de parámetros y su comparación contra la ficha técnica: es el
/// núcleo del módulo.
///
/// La evaluación no se hace aquí sino en la base, dentro de la misma llamada
/// que guarda el valor. Así la marca "fuera de rango" y su alerta se escriben
/// junto con la medición, y no puede quedar un valor desviado sin alerta.
/// </summary>
public interface IParametroService
{
    /// <summary>
    /// Ficha vigente del producto y los parámetros aplicables al área, con sus
    /// límites. TieneFicha en false no es un error: significa que el producto no
    /// tiene ficha activa y que los valores se capturarán sin comparación.
    /// </summary>
    Task<FichaAplicable> ObtenerFichaAplicableAsync(int registroId, CancellationToken ct = default);

    /// <summary>
    /// Formulario de parámetros del ámbito indicado: incluye los ya medidos y
    /// los pendientes, cada uno con sus límites y su semáforo.
    /// </summary>
    Task<List<ParametroMedicion>> ListarAsync(
        int registroId,
        string ambito = AmbitoParametro.Registro,
        int? bobinaId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Guarda las mediciones y devuelve el resultado de evaluarlas contra la
    /// ficha. Solo se tocan los parámetros enviados. Con bobinaId, la medición
    /// se atribuye a esa bobina; sin él, a la corrida completa.
    /// </summary>
    Task<EvaluacionResumen> GuardarAsync(
        int registroId,
        IEnumerable<ValorParametro> valores,
        int? bobinaId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Vuelve a evaluar lo ya capturado contra la ficha vigente, sin escribir
    /// mediciones nuevas. Útil si la ficha cambió después de la captura.
    /// </summary>
    Task<EvaluacionResumen> ReevaluarAsync(
        int registroId, int? bobinaId = null, CancellationToken ct = default);
}
