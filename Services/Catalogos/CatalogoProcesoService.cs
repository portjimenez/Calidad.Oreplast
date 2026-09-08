using calidad_app.Data.Sp;
using calidad_app.Models.Catalogos;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Catalogos;

public class CatalogoProcesoService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : ICatalogoProcesoService
{
    /* ---- Materiales ---- */

    public Task<List<MaterialCatalogo>> ListarMaterialesAsync(
        string? busqueda = null, bool soloActivos = false, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Material_Listar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@SoloActivos", soloActivos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Material, token),
            ct);

    public async Task<int> GuardarMaterialAsync(
        MaterialCatalogo material, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_Material_Guardar",
            cmd => cmd
                .Con("@Codigo", material.Codigo)
                .Con("@Nombre", material.Nombre)
                .Con("@UsuarioId", usuarioId)
                .Con("@MaterialId", material.MaterialId == 0 ? null : material.MaterialId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public Task CambiarEstadoMaterialAsync(
        int materialId, bool activo, CancellationToken ct = default) =>
        CambiarEstadoAsync("cat.usp_Material_CambiarEstado", "@MaterialId", materialId, activo, ct);

    /* ---- Parámetros medibles ---- */

    public Task<List<ParametroCatalogo>> ListarParametrosAsync(
        string? busqueda = null, int? areaId = null, bool soloActivos = false,
        bool soloCriticos = false, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Parametro_Listar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@AreaId", areaId)
                .Con("@SoloActivos", soloActivos)
                .Con("@SoloCriticos", soloCriticos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Parametro, token),
            ct);

    public async Task<int> GuardarParametroAsync(
        ParametroCatalogo parametro, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_Parametro_Guardar",
            cmd => cmd
                .Con("@Codigo", parametro.Codigo)
                .Con("@Nombre", parametro.Nombre)
                .Con("@UsuarioId", usuarioId)
                .Con("@Unidad", parametro.Unidad)
                .Con("@AreaId", parametro.AreaId)
                .Con("@EsCritico", parametro.EsCritico)
                .Con("@Orden", parametro.Orden)
                .Con("@ParametroId", parametro.ParametroId == 0 ? null : parametro.ParametroId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public Task CambiarEstadoParametroAsync(
        int parametroId, bool activo, CancellationToken ct = default) =>
        CambiarEstadoAsync("cat.usp_Parametro_CambiarEstado", "@ParametroId", parametroId, activo, ct);

    /* ---- Ítems de verificación ---- */

    public Task<List<ItemChecklistCatalogo>> ListarItemsChecklistAsync(
        string? tipo = null, int? areaId = null, string? busqueda = null,
        bool soloActivos = false, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_ItemChecklist_Listar",
            cmd => cmd
                .Con("@Tipo", tipo)
                .Con("@AreaId", areaId)
                .Con("@Busqueda", busqueda)
                .Con("@SoloActivos", soloActivos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.ItemChecklist, token),
            ct);

    public async Task<int> GuardarItemChecklistAsync(
        ItemChecklistCatalogo item, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_ItemChecklist_Guardar",
            cmd => cmd
                .Con("@Codigo", item.Codigo)
                .Con("@Texto", item.Texto)
                .Con("@Tipo", item.Tipo)
                .Con("@UsuarioId", usuarioId)
                .Con("@AreaId", item.AreaId)
                .Con("@Orden", item.Orden)
                .Con("@ItemId", item.ItemId == 0 ? null : item.ItemId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public Task CambiarEstadoItemChecklistAsync(
        int itemId, bool activo, CancellationToken ct = default) =>
        CambiarEstadoAsync("cat.usp_ItemChecklist_CambiarEstado", "@ItemId", itemId, activo, ct);

    /* ---- Tipos de defecto ---- */

    public Task<List<TipoDefectoCatalogo>> ListarTiposDefectoAsync(
        string? busqueda = null, int? areaId = null, bool soloActivos = false,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_TipoDefecto_Listar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@AreaId", areaId)
                .Con("@SoloActivos", soloActivos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.TipoDefecto, token),
            ct);

    public async Task<int> GuardarTipoDefectoAsync(
        TipoDefectoCatalogo tipo, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_TipoDefecto_Guardar",
            cmd => cmd
                .Con("@Nombre", tipo.Nombre)
                .Con("@UsuarioId", usuarioId)
                .Con("@AreaId", tipo.AreaId)
                .Con("@TipoDefectoId", tipo.TipoDefectoId == 0 ? null : tipo.TipoDefectoId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public Task CambiarEstadoTipoDefectoAsync(
        int tipoDefectoId, bool activo, CancellationToken ct = default) =>
        CambiarEstadoAsync(
            "cat.usp_TipoDefecto_CambiarEstado", "@TipoDefectoId", tipoDefectoId, activo, ct);

    /* ---- Razones de tiempo muerto ---- */

    public Task<List<RazonTiempoMuertoCatalogo>> ListarRazonesTiempoMuertoAsync(
        string? busqueda = null, bool soloActivos = false, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_RazonTiempoMuerto_Listar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@SoloActivos", soloActivos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.RazonTiempoMuerto, token),
            ct);

    public async Task<int> GuardarRazonTiempoMuertoAsync(
        RazonTiempoMuertoCatalogo razon, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_RazonTiempoMuerto_Guardar",
            cmd => cmd
                .Con("@Nombre", razon.Nombre)
                .Con("@UsuarioId", usuarioId)
                .Con("@RazonId", razon.RazonId == 0 ? null : razon.RazonId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public Task CambiarEstadoRazonTiempoMuertoAsync(
        int razonId, bool activo, CancellationToken ct = default) =>
        CambiarEstadoAsync(
            "cat.usp_RazonTiempoMuerto_CambiarEstado", "@RazonId", razonId, activo, ct);

    /// <summary>
    /// Los cinco cambios de estado de este servicio se diferencian solo en el
    /// procedimiento y en el nombre del parámetro del identificador, así que
    /// comparten la llamada en lugar de repetirla cinco veces.
    /// </summary>
    private async Task CambiarEstadoAsync(
        string procedimiento, string parametroId, int id, bool activo, CancellationToken ct)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            procedimiento,
            cmd => cmd
                .Con(parametroId, id)
                .Con("@Activo", activo)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }
}
