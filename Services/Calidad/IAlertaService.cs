using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Alertas de proceso: los parámetros que quedaron fuera de la tolerancia de la
/// ficha técnica.
///
/// Las alertas no se crean desde aquí. Las genera la base al evaluar cada
/// medición (módulo 2), junto con el valor que las provocó, de modo que no
/// pueda existir una desviación sin su alerta. Este servicio es el lado de
/// Calidad: verlas, entenderlas y cerrarlas dejando constancia.
/// </summary>
public interface IAlertaService
{
    /// <summary>Monitor de alertas. Los filtros en null no filtran.</summary>
    Task<List<AlertaResumen>> ListarAsync(
        FiltroAlertas filtro, int maxFilas = 200, CancellationToken ct = default);

    /// <summary>
    /// Alerta con su contexto: el historial del mismo parámetro en el registro
    /// y las demás alertas abiertas de ese registro. Con eso se decide si se
    /// atiende con una observación o si amerita una no conformidad.
    /// </summary>
    Task<AlertaDetalle?> ObtenerAsync(int alertaId, CancellationToken ct = default);

    /// <summary>Totales y cortes por parámetro, máquina y turno.</summary>
    Task<ResumenAlertas> ObtenerResumenAsync(
        FiltroAlertas filtro, CancellationToken ct = default);

    /// <summary>
    /// Cierra las alertas indicadas con una observación obligatoria y, si se
    /// indica, las vincula a la no conformidad que se levantó por ellas.
    ///
    /// Atender no borra la desviación: la alerta queda como evidencia de que
    /// ocurrió. Lo que registra es que Calidad ya la revisó y qué dispuso.
    /// </summary>
    Task<ResultadoAtencion> AtenderAsync(
        IReadOnlyCollection<int> alertaIds,
        string observacion,
        int? noConformidadId = null,
        CancellationToken ct = default);
}
