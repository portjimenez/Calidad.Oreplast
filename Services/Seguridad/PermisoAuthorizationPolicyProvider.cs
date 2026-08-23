using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Traduce cualquier Policy="CLAVE_PERMISO" (ej. "LIBERAR_PRODUCTO") en un PermisoRequirement,
/// sin tener que registrar cada permiso de seg.Permiso a mano en Program.cs. Es el único lugar
/// donde vive la regla "qué policy exige qué claim" — los componentes solo declaran la clave.
/// </summary>
public class PermisoAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermisoRequirement(policyName))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
