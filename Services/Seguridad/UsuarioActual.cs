using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Identidad del usuario que está ejecutando la acción, resuelta desde los
/// claims que dejó <see cref="SegUsuarioClaimsTransformation"/>.
///
/// Todos los procedimientos de escritura reciben @UsuarioId para la bitácora.
/// Ese dato lo resuelve el servidor a partir del claim, nunca lo envía la
/// pantalla: si viajara como parámetro desde el navegador se podría falsear y
/// la bitácora dejaría de ser confiable.
/// </summary>
public interface IUsuarioActual
{
    /// <summary>Id en seg.Usuario. Lanza si no hay sesión válida.</summary>
    Task<int> ObtenerIdAsync();

    Task<string> ObtenerNombreAsync();

    Task<bool> TienePermisoAsync(string clave);
}

public class UsuarioActual(AuthenticationStateProvider proveedor) : IUsuarioActual
{
    public async Task<int> ObtenerIdAsync()
    {
        var usuario = await ObtenerPrincipalAsync();
        var claim = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(claim, out var usuarioId))
        {
            throw new InvalidOperationException(
                "No hay un usuario válido en la sesión actual para registrar la acción.");
        }

        return usuarioId;
    }

    public async Task<string> ObtenerNombreAsync()
    {
        var usuario = await ObtenerPrincipalAsync();
        return usuario.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    public async Task<bool> TienePermisoAsync(string clave)
    {
        var usuario = await ObtenerPrincipalAsync();
        return usuario.HasClaim("permiso", clave);
    }

    private async Task<ClaimsPrincipal> ObtenerPrincipalAsync()
    {
        var estado = await proveedor.GetAuthenticationStateAsync();
        return estado.User;
    }
}
