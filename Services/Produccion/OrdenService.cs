using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Produccion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Produccion;

public class OrdenService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IOrdenService
{
    public Task<List<OrdenResumen>> ListarAsync(
        FiltroOrdenes filtro, int maxFilas = 200, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Orden_Listar",
            cmd => cmd
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@Estado", filtro.Estado)
                .Con("@ClienteId", filtro.ClienteId)
                .Con("@ProductoId", filtro.ProductoId)
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@SoloConPendientes", filtro.SoloConPendientes)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosProduccion.OrdenResumen, token),
            ct);

    public Task<ResumenOrdenes> ResumenAsync(
        FiltroOrdenes filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Orden_Resumen",
            cmd => cmd
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@ClienteId", filtro.ClienteId)
                .Con("@ProductoId", filtro.ProductoId),
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosProduccion.ResumenOrdenes, token) ?? new(),
            ct);

    public Task<OrdenDetalle?> ObtenerAsync(int ordenId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Orden_Obtener",
            cmd => cmd.Con("@OrdenId", ordenId),
            LeerExpedienteAsync,
            ct);

    public async Task<int> GuardarAsync(OrdenEdicion orden, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Orden_Guardar",
            cmd => cmd
                .Con("@NumeroOP", orden.NumeroOP)
                .Con("@ClienteId", orden.ClienteId)
                .Con("@ProductoId", orden.ProductoId)
                .Con("@UsuarioId", usuarioId)
                .Con("@KgProgramados", orden.KgProgramados)
                .Con("@OrdenId", orden.OrdenId == 0 ? null : orden.OrdenId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosProduccion.LeerIdAsync,
            ct);
    }

    public async Task CambiarEstadoAsync(
        int ordenId, string estado, string? motivo = null, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "prod.usp_Orden_CambiarEstado",
            cmd => cmd
                .Con("@OrdenId", ordenId)
                .Con("@Estado", estado)
                .Con("@UsuarioId", usuarioId)
                .Con("@Motivo", motivo)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /// <summary>
    /// Los cinco conjuntos del expediente: encabezado, registros de
    /// inspección, lotes, bobinas y no conformidades. EF Core solo sabe leer
    /// el primero, por eso esta consulta baja a ADO.NET.
    /// </summary>
    private static async Task<OrdenDetalle?> LeerExpedienteAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var orden = await lector.LeerUnoAsync(MapeosProduccion.OrdenDetalle, ct);

        if (orden is null)
        {
            return null;
        }

        if (await lector.NextResultAsync(ct))
        {
            orden.RegistrosDeInspeccion =
                await lector.LeerListaAsync(MapeosProduccion.RegistroDeOrden, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            orden.LotesDeLaOrden = await lector.LeerListaAsync(MapeosProduccion.LoteDeOrden, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            orden.BobinasDeLaOrden =
                await lector.LeerListaAsync(MapeosProduccion.BobinaDeOrden, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            orden.NoConformidades =
                await lector.LeerListaAsync(MapeosProduccion.NoConformidadDeOrden, ct);
        }

        return orden;
    }
}
