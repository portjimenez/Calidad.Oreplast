using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

public class PanelCalidadService(EjecutorSp sp) : IPanelCalidadService
{
    public Task<ResumenPanelCalidad> ObtenerAsync(
        DateOnly? desde = null,
        DateOnly? hasta = null,
        int? areaId = null,
        int maxFilas = 10,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_PanelCalidad_Resumen",
            cmd => cmd
                .Con("@FechaDesde", desde)
                .Con("@FechaHasta", hasta)
                .Con("@AreaId", areaId)
                .Con("@MaxFilas", maxFilas),
            async (lector, token) =>
            {
                var panel = new ResumenPanelCalidad
                {
                    Tarjetas = await lector.LeerUnoAsync(MapeosCalidad.TarjetasPanel, token) ?? new()
                };

                if (await lector.NextResultAsync(token))
                {
                    panel.AlertasUrgentes =
                        await lector.LeerListaAsync(MapeosCalidad.AlertaUrgente, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    panel.NoConformidades =
                        await lector.LeerListaAsync(MapeosCalidad.NoConformidadPendiente, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    panel.LotesPorCertificar =
                        await lector.LeerListaAsync(MapeosCalidad.LotePorCertificar, token);
                }

                return panel;
            },
            ct);
}
