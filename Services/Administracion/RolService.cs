using System.Data.Common;
using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Administracion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Administracion;

public class RolService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IRolService
{
    /// <summary>
    /// Se envía quién consulta para que la base marque su rol: en esa columna la
    /// casilla GESTIONAR_USUARIOS va bloqueada.
    /// </summary>
    public async Task<MatrizPermisos> ObtenerMatrizAsync(CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "seg.usp_RolPermiso_Matriz",
            cmd => cmd.Con("@UsuarioId", usuarioId),
            LeerMatrizAsync,
            ct);
    }

    public async Task<ResultadoPermisos> GuardarPermisosAsync(
        int rolId, IEnumerable<string> claves, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        // Un arreglo JSON de claves: ["VER_PANEL","VER_ORDENES"]. Se envía
        // también cuando está vacío, que es un valor válido (rol sin permisos).
        var json = JsonSerializer.Serialize(claves.Distinct().ToList());

        return await sp.ConsultarAsync(
            "seg.usp_RolPermiso_Guardar",
            cmd => cmd
                .Con("@RolId", rolId)
                .Con("@PermisosJson", json)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosAdministracion.LeerResultadoPermisosAsync,
            ct);
    }

    /// <summary>Tres conjuntos: roles, permisos agrupados y casillas marcadas.</summary>
    private static async Task<MatrizPermisos> LeerMatrizAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var matriz = new MatrizPermisos
        {
            Roles = await lector.LeerListaAsync(MapeosAdministracion.RolMatriz, ct)
        };

        if (await lector.NextResultAsync(ct))
        {
            matriz.Permisos = await lector.LeerListaAsync(MapeosAdministracion.PermisoMatriz, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            var asignaciones = await lector.LeerListaAsync(MapeosAdministracion.Asignacion, ct);
            matriz.Asignaciones = [.. asignaciones];
        }

        return matriz;
    }
}
