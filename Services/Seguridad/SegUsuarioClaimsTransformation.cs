using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Enriquece el HttpContext.User inmediatamente después de la autenticación (Negotiate en
/// producción, el esquema "Simulacion" en desarrollo) contra seg.Usuario: agrega el rol y los
/// permisos como claims, o lo reduce a anónimo si la cuenta no está registrada o está inactiva.
/// Corre ANTES de la autorización, así que AuthorizeRouteView/FallbackPolicy (y por lo tanto
/// AccesoNoAutorizado) ven siempre la decisión real, incluso en la primera carga de página.
/// </summary>
public class SegUsuarioClaimsTransformation(
    IAuthService authService, IHttpContextAccessor httpContextAccessor) : IClaimsTransformation
{
    private const string TipoIdentidadEnriquecida = "Oreplast";

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // IClaimsTransformation puede invocarse más de una vez por petición.
        if (principal.Identities.Any(i => i.AuthenticationType == TipoIdentidadEnriquecida))
        {
            return principal;
        }

        var usuarioDominio = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuarioDominio))
        {
            return principal;
        }

        var httpContext = httpContextAccessor.HttpContext;
        var direccionIp = httpContext?.Connection.RemoteIpAddress?.ToString();
        var resultado = await authService.ValidarAccesoAsync(usuarioDominio, direccionIp);

        if (httpContext is not null)
        {
            httpContext.Items["AccesoResultado"] = resultado;
        }

        if (!resultado.Autorizado || resultado.UsuarioId is null)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var permisos = await authService.ObtenerPermisosAsync(resultado.UsuarioId.Value);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, resultado.UsuarioId.Value.ToString()),
            new(ClaimTypes.Name, resultado.NombreCompleto ?? resultado.UsuarioDominio),
            new(ClaimTypes.Role, resultado.RolNombre ?? string.Empty),
            new("usuario_dominio", resultado.UsuarioDominio),
        };
        claims.AddRange(permisos.Select(p => new Claim("permiso", p.Clave)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, TipoIdentidadEnriquecida));
    }
}
