using calidad_app.Models.Seguridad;

namespace calidad_app.Services.Seguridad;

public interface IAuthService
{
    /// <param name="usuarioElegidoId">
    /// Perfil elegido en "¿Quién está trabajando?" (cookie <see cref="PerfilElegido"/>). Solo
    /// se toma en cuenta si la cuenta es compartida y el usuario es activo de esa misma cuenta.
    /// </param>
    Task<AccesoResultado> ValidarAccesoAsync(string usuarioDominio, string? direccionIp, int? usuarioElegidoId = null);

    Task<List<PermisoUsuario>> ObtenerPermisosAsync(int usuarioId);

    /// <summary>Usuarios activos que comparten la cuenta de dominio.</summary>
    Task<List<PerfilCuenta>> ObtenerPerfilesDeCuentaAsync(string usuarioDominio);
}
