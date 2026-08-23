using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Desarrollo: identidad de dominio simulada, sin depender de Windows Authentication. La cuenta
/// activa se guarda en la cookie "simulacion_usuario" (la cambia el selector de la topbar vía
/// /dev/simular, con una recarga real, igual que un login real) y si no hay cookie usa
/// Simulacion:UsuarioPredeterminado. SegUsuarioClaimsTransformation hace la validación real
/// contra seg.Usuario justo después. NO debe registrarse en producción.
/// </summary>
public class SimulacionAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var usuarioDominio = Request.Cookies[SimulacionConstantes.CookieUsuario];
        if (string.IsNullOrWhiteSpace(usuarioDominio))
        {
            usuarioDominio = configuration["Simulacion:UsuarioPredeterminado"];
        }

        if (string.IsNullOrWhiteSpace(usuarioDominio))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, usuarioDominio)], authenticationType: "Simulacion");
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
