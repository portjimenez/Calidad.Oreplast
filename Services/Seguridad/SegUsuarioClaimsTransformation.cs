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
        // En una cuenta compartida, la persona elegida en "¿Quién está trabajando?".
        var usuarioElegidoId = httpContext is null ? null : PerfilElegido.Leer(httpContext.Request);
        var resultado = await authService.ValidarAccesoAsync(usuarioDominio, direccionIp, usuarioElegidoId);

        if (httpContext is not null)
        {
            httpContext.Items["AccesoResultado"] = resultado;
        }

        // ELEGIR_PERFIL también llega aquí como no autorizado: AccesoNoAutorizado muestra
        // entonces la pantalla para elegir quién trabaja en lugar del mensaje de rechazo.
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
        if (resultado.AreaId is { } areaId)
        {
            claims.Add(new Claim(ClaimsUsuario.AreaId, areaId.ToString()));
            claims.Add(new Claim(ClaimsUsuario.AreaNombre, resultado.AreaNombre ?? string.Empty));
        }
        if (resultado.CuentaCompartida)
        {
            claims.Add(new Claim(ClaimsUsuario.CuentaCompartida, "1"));
        }

        claims.AddRange(permisos.Select(p => new Claim("permiso", p.Clave)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, TipoIdentidadEnriquecida));
    }
}
