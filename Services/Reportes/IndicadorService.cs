using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Reportes;

namespace calidad_app.Services.Reportes;

public class IndicadorService(EjecutorSp sp) : IIndicadorService
{
    public Task<TableroIndicadores> TableroAsync(
        FiltroIndicadores filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Kpi_Resumen",
            cmd => Ambito(cmd, filtro),
            LeerTableroAsync,
            ct);

    public Task<List<PuntoTendencia>> TendenciaAsync(
        FiltroIndicadores filtro, string granularidad, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Kpi_Tendencia",
            cmd => Ambito(cmd, filtro).Con("@Granularidad", granularidad),
            (lector, token) => lector.LeerListaAsync(MapeosReportes.PuntoTendencia, token),
            ct);

    public Task<List<GrupoIndicador>> ComparativoAsync(
        FiltroIndicadores filtro, string agrupar, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Kpi_PorDimension",
            cmd => Ambito(cmd, filtro).Con("@Agrupar", agrupar),
            (lector, token) => lector.LeerListaAsync(MapeosReportes.GrupoIndicador, token),
            ct);

    public Task<ResumenCalidadKpi> CalidadAsync(
        FiltroIndicadores filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Kpi_Calidad",
            cmd => Ambito(cmd, filtro),
            LeerCalidadAsync,
            ct);

    public Task<List<CausaPareto>> ParetoAsync(
        FiltroIndicadores filtro, string origen, int maxFilas = 15,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Kpi_Pareto",
            cmd => Ambito(cmd, filtro)
                .Con("@Origen", origen)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosReportes.CausaPareto, token),
            ct);

    /// <summary>
    /// Los cinco procedimientos aceptan el mismo ámbito, así que los
    /// parámetros se arman una sola vez: si mañana se agrega una dimensión al
    /// filtro, se agrega aquí y las cinco consultas la reciben.
    /// </summary>
    private static DbCommand Ambito(DbCommand cmd, FiltroIndicadores filtro) => cmd
        .Con("@FechaDesde", filtro.FechaDesde)
        .Con("@FechaHasta", filtro.FechaHasta)
        .Con("@AreaId", filtro.AreaId)
        .Con("@LineaId", filtro.LineaId)
        .Con("@MaquinaId", filtro.MaquinaId)
        .Con("@TurnoId", filtro.TurnoId)
        .Con("@ProductoId", filtro.ProductoId)
        .Con("@ClienteId", filtro.ClienteId)
        .Con("@OperadorId", filtro.OperadorId);

    /// <summary>
    /// El tablero llega en dos conjuntos: los totales del periodo y una fila
    /// por indicador. Se juntan en un solo objeto porque la pantalla los
    /// muestra como una sola cabecera.
    /// </summary>
    private static async Task<TableroIndicadores> LeerTableroAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var tablero = new TableroIndicadores
        {
            Totales = await lector.LeerUnoAsync(MapeosReportes.TotalesPeriodo, ct) ?? new TotalesPeriodo()
        };

        if (await lector.NextResultAsync(ct))
        {
            tablero.Indicadores = await lector.LeerListaAsync(MapeosReportes.IndicadorKpi, ct);
        }

        return tablero;
    }

    /// <summary>
    /// El foco de calidad llega en cuatro conjuntos (alertas, no conformidades
    /// por severidad, por estado y embudo de liberación) porque las cuatro
    /// partes se leen juntas: un número alto de alertas solo se puede juzgar
    /// sabiendo si se atendieron.
    /// </summary>
    private static async Task<ResumenCalidadKpi> LeerCalidadAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var resumen = new ResumenCalidadKpi
        {
            Alertas = await lector.LeerUnoAsync(MapeosReportes.AlertasKpi, ct) ?? new AlertasKpi()
        };

        if (await lector.NextResultAsync(ct))
        {
            resumen.PorSeveridad = await lector.LeerListaAsync(MapeosReportes.NcPorSeveridad, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            resumen.PorEstado = await lector.LeerListaAsync(MapeosReportes.NcPorEstado, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            resumen.Embudo = await lector.LeerUnoAsync(MapeosReportes.EmbudoLiberacion, ct)
                             ?? new EmbudoLiberacion();
        }

        return resumen;
    }
}
