using Microsoft.AspNetCore.Authorization;

namespace calidad_app.Services.Seguridad;

/// <summary>Exige que el usuario tenga el permiso (seg.Permiso.Clave) indicado.</summary>
public class PermisoRequirement(string clave) : IAuthorizationRequirement
{
    public string Clave { get; } = clave;
}
