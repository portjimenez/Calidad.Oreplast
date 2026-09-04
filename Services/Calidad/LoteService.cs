using System.Data.Common;
using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Calidad;

public class LoteService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : ILoteService
{
    public Task<List<LoteResumen>> ListarAsync(
        FiltroLotes filtro, int maxFilas = 200, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Lote_Listar",
            cmd => cmd
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@OrdenId", filtro.OrdenId)
                .Con("@ClienteId", filtro.ClienteId)
                .Con("@ProductoId", filtro.ProductoId)
                .Con("@Estado", filtro.Estado)
                .Con("@SoloSinCertificado", filtro.SoloSinCertificado)
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosLotes.LoteResumen, token),
            ct);

    public Task<LoteDetalle?> ObtenerAsync(int loteId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Lote_Obtener",
            cmd => cmd.Con("@LoteId", loteId),
            LeerExpedienteAsync,
            ct);

    public async Task<LoteCreado> CrearAsync(
        int ordenId,
        DateOnly? fechaProduccion = null,
        string? codigoLote = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Lote_Crear",
            cmd => cmd
                .Con("@OrdenId", ordenId)
                .Con("@UsuarioId", usuarioId)
                .Con("@FechaProduccion", fechaProduccion)
                .Con("@CodigoLote", codigoLote)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosLotes.LoteCreado, token) ?? new(),
            ct);
    }

    public Task<List<BobinaDisponible>> ListarBobinasDisponiblesAsync(
        int? loteId = null, int? ordenId = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Lote_BobinasDisponibles",
            cmd => cmd
                .Con("@LoteId", loteId)
                .Con("@OrdenId", ordenId),
            (lector, token) => lector.LeerListaAsync(MapeosLotes.BobinaDisponible, token),
            ct);

    public async Task<LoteDetalle?> AsignarBobinasAsync(
        int loteId,
        IReadOnlyCollection<int> bobinaIds,
        bool asignar = true,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();
        var json = JsonSerializer.Serialize(bobinaIds.Select(id => new { BobinaId = id }));

        return await sp.ConsultarAsync(
            "prod.usp_Lote_AsignarBobinas",
            cmd => cmd
                .Con("@LoteId", loteId)
                .Con("@BobinasJson", json)
                .Con("@UsuarioId", usuarioId)
                .Con("@Asignar", asignar)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerExpedienteAsync,
            ct);
    }

    /// <summary>
    /// Los cinco conjuntos del expediente. Lo comparten obtener y asignar
    /// bobinas: el guardado devuelve el lote completo para que la pantalla se
    /// refresque con lo que la base aceptó.
    /// </summary>
    private static async Task<LoteDetalle?> LeerExpedienteAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var lote = await lector.LeerUnoAsync(MapeosLotes.LoteDetalle, ct);

        if (lote is null)
        {
            return null;
        }

        if (await lector.NextResultAsync(ct))
        {
            lote.Bobinas = await lector.LeerListaAsync(MapeosLotes.BobinaDeLote, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            lote.Registros = await lector.LeerListaAsync(MapeosLotes.RegistroDeLote, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            lote.NoConformidades = await lector.LeerListaAsync(MapeosLotes.NoConformidadDeLote, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            lote.Alertas = await lector.LeerListaAsync(MapeosCalidad.AlertaDeNoConformidad, ct);
        }

        return lote;
    }
}
