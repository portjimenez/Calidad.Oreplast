using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Administracion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Administracion;

public class UsuarioService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IUsuarioService
{
    public Task<ResumenAdministracion> ResumenAsync(CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "seg.usp_Administracion_Resumen",
            _ => { },
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosAdministracion.ResumenAdministracion, token) ?? new(),
            ct);

    public Task<List<UsuarioResumen>> ListarAsync(
        FiltroUsuarios filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "seg.usp_Usuario_Listar",
            cmd => cmd
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@RolId", filtro.RolId)
                .Con("@AreaId", filtro.AreaId)
                .Con("@Activo", filtro.Activo),
            (lector, token) => lector.LeerListaAsync(MapeosAdministracion.UsuarioResumen, token),
            ct);

    public Task<UsuarioDetalle?> ObtenerAsync(int usuarioId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "seg.usp_Usuario_Obtener",
            cmd => cmd.Con("@UsuarioGestionadoId", usuarioId),
            LeerDetalleAsync,
            ct);

    public async Task<int> GuardarAsync(UsuarioEdicion usuario, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "seg.usp_Usuario_Guardar",
            cmd => cmd
                .Con("@Codigo", usuario.Codigo)
                .Con("@NombreCompleto", usuario.NombreCompleto)
                .Con("@UsuarioDominio", usuario.UsuarioDominio)
                .Con("@RolId", usuario.RolId)
                .Con("@UsuarioId", usuarioId)
                .Con("@AreaId", usuario.AreaId)
                .Con("@UsuarioGestionadoId", usuario.UsuarioId == 0 ? null : usuario.UsuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosAdministracion.LeerIdAsync,
            ct);
    }

    public async Task CambiarEstadoAsync(int usuarioId, bool activo, CancellationToken ct = default)
    {
        var actorId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "seg.usp_Usuario_CambiarEstado",
            cmd => cmd
                .Con("@UsuarioGestionadoId", usuarioId)
                .Con("@Activo", activo)
                .Con("@UsuarioId", actorId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /// <summary>
    /// Tres conjuntos: el usuario, los permisos de su rol y sus últimos
    /// movimientos. Un primer conjunto vacío significa que el usuario no existe.
    /// </summary>
    private static async Task<UsuarioDetalle?> LeerDetalleAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var usuario = await lector.LeerUnoAsync(MapeosAdministracion.UsuarioDetalle, ct);

        if (usuario is null)
        {
            return null;
        }

        if (await lector.NextResultAsync(ct))
        {
            usuario.Permisos = await lector.LeerListaAsync(MapeosAdministracion.Permiso, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            usuario.Movimientos =
                await lector.LeerListaAsync(MapeosAdministracion.MovimientoUsuario, ct);
        }

        return usuario;
    }
}
