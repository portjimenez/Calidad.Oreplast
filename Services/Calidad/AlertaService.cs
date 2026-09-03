using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Calidad;

public class AlertaService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IAlertaService
{
    public Task<List<AlertaResumen>> ListarAsync(
        FiltroAlertas filtro, int maxFilas = 200, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Alerta_Listar",
            cmd => cmd
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@AreaId", filtro.AreaId)
                .Con("@MaquinaId", filtro.MaquinaId)
                .Con("@TurnoId", filtro.TurnoId)
                .Con("@OperadorId", filtro.OperadorId)
                .Con("@ParametroId", filtro.ParametroId)
                .Con("@RegistroId", filtro.RegistroId)
                .Con("@LoteId", filtro.LoteId)
                .Con("@Atendida", filtro.Atendida)
                .Con("@SoloCriticos", filtro.SoloCriticos)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosCalidad.AlertaResumen, token),
            ct);

    public Task<AlertaDetalle?> ObtenerAsync(int alertaId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Alerta_Obtener",
            cmd => cmd.Con("@AlertaId", alertaId),
            async (lector, token) =>
            {
                var alerta = await lector.LeerUnoAsync(MapeosCalidad.AlertaDetalle, token);

                if (alerta is null)
                {
                    return null;
                }

                // Segundo resultado: cómo se comportó el parámetro a lo largo del registro.
                if (await lector.NextResultAsync(token))
                {
                    alerta.Historial = await lector.LeerListaAsync(MapeosCalidad.Medicion, token);
                }

                // Tercero: las demás alertas abiertas del mismo registro.
                if (await lector.NextResultAsync(token))
                {
                    alerta.Relacionadas =
                        await lector.LeerListaAsync(MapeosCalidad.AlertaRelacionada, token);
                }

                return alerta;
            },
            ct);

    public Task<ResumenAlertas> ObtenerResumenAsync(
        FiltroAlertas filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Alerta_Resumen",
            cmd => cmd
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@AreaId", filtro.AreaId)
                .Con("@MaquinaId", filtro.MaquinaId)
                .Con("@Atendida", filtro.Atendida),
            async (lector, token) =>
            {
                var resumen = new ResumenAlertas
                {
                    Totales = await lector.LeerUnoAsync(MapeosCalidad.TotalesAlertas, token) ?? new()
                };

                if (await lector.NextResultAsync(token))
                {
                    resumen.PorParametro =
                        await lector.LeerListaAsync(MapeosCalidad.AlertasPorParametro, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    resumen.PorMaquina =
                        await lector.LeerListaAsync(MapeosCalidad.AlertasPorMaquina, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    resumen.PorTurno =
                        await lector.LeerListaAsync(MapeosCalidad.AlertasPorTurno, token);
                }

                return resumen;
            },
            ct);

    public async Task<ResultadoAtencion> AtenderAsync(
        IReadOnlyCollection<int> alertaIds,
        string observacion,
        int? noConformidadId = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        // El procedimiento acepta una alerta suelta o una lista; se usa siempre
        // la lista para no tener dos caminos que mantener.
        var json = JsonSerializer.Serialize(alertaIds.Select(id => new { AlertaId = id }));

        return await sp.ConsultarAsync(
            "cal.usp_Alerta_Atender",
            cmd => cmd
                .Con("@UsuarioId", usuarioId)
                .Con("@Observacion", observacion)
                .Con("@AlertasJson", json)
                .Con("@NoConformidadId", noConformidadId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosCalidad.ResultadoAtencion, token) ?? new(),
            ct);
    }
}
