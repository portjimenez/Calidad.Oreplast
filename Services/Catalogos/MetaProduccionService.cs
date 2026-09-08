using calidad_app.Data.Sp;
using calidad_app.Models.Catalogos;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Catalogos;

public class MetaProduccionService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IMetaProduccionService
{
    public Task<List<MetaProduccion>> ListarAsync(
        string? concepto = null, int? lineaId = null, int? maquinaId = null,
        int? productoId = null, bool soloActivos = false, bool soloVigentes = false,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Meta_Listar",
            cmd => cmd
                .Con("@Concepto", concepto)
                .Con("@LineaId", lineaId)
                .Con("@MaquinaId", maquinaId)
                .Con("@ProductoId", productoId)
                .Con("@SoloActivos", soloActivos)
                .Con("@SoloVigentes", soloVigentes),
            (lector, token) => lector.LeerListaAsync(MapeosCatalogos.Meta, token),
            ct);

    public async Task<int> GuardarAsync(
        MetaProduccion meta, bool reemplazarVigente = false, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cat.usp_Meta_Guardar",
            cmd => cmd
                .Con("@Concepto", meta.Concepto)
                .Con("@ValorMeta", meta.ValorMeta)
                .Con("@UsuarioId", usuarioId)
                .Con("@Unidad", meta.Unidad)
                .Con("@LineaId", meta.LineaId)
                .Con("@MaquinaId", meta.MaquinaId)
                .Con("@ProductoId", meta.ProductoId)
                .Con("@VigenteDesde", meta.VigenteDesde)
                .Con("@MetaId", meta.MetaId == 0 ? null : meta.MetaId)
                .Con("@ReemplazarVigente", reemplazarVigente)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosCatalogos.LeerIdAsync,
            ct);
    }

    public async Task CambiarEstadoAsync(
        int metaId, bool activo, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "cat.usp_Meta_CambiarEstado",
            cmd => cmd
                .Con("@MetaId", metaId)
                .Con("@Activo", activo)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }
}
