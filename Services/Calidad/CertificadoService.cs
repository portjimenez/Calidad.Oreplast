using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Calidad;

public class CertificadoService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : ICertificadoService
{
    public Task<List<CertificadoResumen>> ListarAsync(
        FiltroCertificados filtro, int maxFilas = 200, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Certificado_Listar",
            cmd => cmd
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@ClienteId", filtro.ClienteId)
                .Con("@ProductoId", filtro.ProductoId)
                .Con("@OrdenId", filtro.OrdenId)
                .Con("@LoteId", filtro.LoteId)
                .Con("@EmitidoPorId", filtro.EmitidoPorId)
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosLotes.CertificadoResumen, token),
            ct);

    public Task<CertificadoDetalle?> ObtenerAsync(
        int? certificadoId = null, int? loteId = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Certificado_Obtener",
            cmd => cmd
                .Con("@CertificadoId", certificadoId)
                .Con("@LoteId", loteId),
            async (lector, token) =>
            {
                var certificado = await lector.LeerUnoAsync(MapeosLotes.CertificadoDetalle, token);

                if (certificado is null)
                {
                    return null;
                }

                // Los resultados contra la ficha: es la tabla que sustenta el documento.
                if (await lector.NextResultAsync(token))
                {
                    certificado.Resultados =
                        await lector.LeerListaAsync(MapeosLotes.ResultadoParametro, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    certificado.BobinasEntregadas =
                        await lector.LeerListaAsync(MapeosLotes.BobinaCertificada, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    certificado.BobinasSegregadasDetalle =
                        await lector.LeerListaAsync(MapeosLotes.BobinaSegregada, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    certificado.NoConformidades =
                        await lector.LeerListaAsync(MapeosLotes.NoConformidadCertificada, token);
                }

                return certificado;
            },
            ct);

    public async Task<CertificadoEmitido> EmitirAsync(
        int loteId, string? observaciones = null, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_Certificado_Emitir",
            cmd => cmd
                .Con("@LoteId", loteId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Observaciones", observaciones)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosLotes.CertificadoEmitido, token) ?? new(),
            ct);
    }
}
