using calidad_app.Data.Sp;
using calidad_app.Models.Catalogos;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Catalogos;

public class CatalogoService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : ICatalogoService
{
    public Task<List<ResumenCatalogo>> ResumenAsync(CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Catalogos_Resumen",
            _ => { },
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Resumen, token),
            ct);

    public Task<SelectoresCatalogo> SelectoresAsync(
        string? busquedaProducto = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Catalogos_Selectores",
            cmd => cmd.Con("@BusquedaProducto", busquedaProducto),
            async (lector, token) =>
            {
                var selectores = new SelectoresCatalogo
                {
                    Areas = await lector.LeerListaAsync(MapeosCatalogos.AreaOpcion, token)
                };

                if (await lector.NextResultAsync(token))
                {
                    selectores.Lineas = await lector.LeerListaAsync(MapeosCatalogos.LineaOpcion, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    selectores.Maquinas = await lector.LeerListaAsync(MapeosCatalogos.MaquinaOpcion, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    selectores.Productos = await lector.LeerListaAsync(MapeosCatalogos.ProductoOpcion, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    selectores.ConceptosMeta = await lector.LeerListaAsync(MapeosCatalogos.OpcionClave, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    selectores.TiposItemChecklist =
                        await lector.LeerListaAsync(MapeosCatalogos.OpcionClave, token);
                }

                return selectores;
            },
            ct);

    /* ---- Áreas ---- */

    public Task<List<AreaCatalogo>> ListarAreasAsync(
        string? busqueda = null, bool soloActivos = false, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Area_Listar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@SoloActivos", soloActivos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Area, token),
            ct);

    public async Task<int> GuardarAreaAsync(AreaCatalogo area, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_Area_Guardar",
            cmd => cmd
                .Con("@Nombre", area.Nombre)
                .Con("@UsuarioId", usuarioId)
                .Con("@AreaId", area.AreaId == 0 ? null : area.AreaId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public async Task CambiarEstadoAreaAsync(
        int areaId, bool activo, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "cat.usp_Area_CambiarEstado",
            cmd => cmd
                .Con("@AreaId", areaId)
                .Con("@Activo", activo)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /* ---- Líneas de producción ---- */

    public Task<List<LineaCatalogo>> ListarLineasAsync(
        string? busqueda = null, int? areaId = null, bool soloActivos = false,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Linea_Listar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@AreaId", areaId)
                .Con("@SoloActivos", soloActivos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Linea, token),
            ct);

    public async Task<int> GuardarLineaAsync(LineaCatalogo linea, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_Linea_Guardar",
            cmd => cmd
                .Con("@Codigo", linea.Codigo)
                .Con("@Nombre", linea.Nombre)
                .Con("@AreaId", linea.AreaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@LineaId", linea.LineaId == 0 ? null : linea.LineaId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public async Task CambiarEstadoLineaAsync(
        int lineaId, bool activo, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "cat.usp_Linea_CambiarEstado",
            cmd => cmd
                .Con("@LineaId", lineaId)
                .Con("@Activo", activo)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /* ---- Máquinas ---- */

    public Task<List<MaquinaCatalogo>> ListarMaquinasAsync(
        string? busqueda = null, int? areaId = null, int? lineaId = null,
        bool soloActivos = false, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Maquina_Listar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@AreaId", areaId)
                .Con("@LineaId", lineaId)
                .Con("@SoloActivos", soloActivos),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Maquina, token),
            ct);

    public async Task<int> GuardarMaquinaAsync(
        MaquinaCatalogo maquina, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_Maquina_Guardar",
            cmd => cmd
                .Con("@Codigo", maquina.Codigo)
                .Con("@Nombre", maquina.Nombre)
                .Con("@AreaId", maquina.AreaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@LineaId", maquina.LineaId)
                .Con("@MaquinaId", maquina.MaquinaId == 0 ? null : maquina.MaquinaId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public async Task CambiarEstadoMaquinaAsync(
        int maquinaId, bool activo, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "cat.usp_Maquina_CambiarEstado",
            cmd => cmd
                .Con("@MaquinaId", maquinaId)
                .Con("@Activo", activo)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /* ---- Turnos ---- */

    public Task<List<TurnoCatalogo>> ListarTurnosAsync(
        string? busqueda = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Turno_Listar",
            cmd => cmd.Con("@Busqueda", busqueda),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Turno, token),
            ct);

    public async Task<int> GuardarTurnoAsync(TurnoCatalogo turno, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_Turno_Guardar",
            cmd => cmd
                .Con("@Nombre", turno.Nombre)
                .Con("@UsuarioId", usuarioId)
                .Con("@TurnoId", turno.TurnoId == 0 ? null : turno.TurnoId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }
}
