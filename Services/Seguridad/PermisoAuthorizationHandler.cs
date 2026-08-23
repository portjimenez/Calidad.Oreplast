using Microsoft.AspNetCore.Authorization;

namespace calidad_app.Services.Seguridad;

public class PermisoAuthorizationHandler : AuthorizationHandler<PermisoRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermisoRequirement requirement)
    {
        if (context.User.HasClaim("permiso", requirement.Clave))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
