using calidad_app.Models.Seguridad;

namespace calidad_app.Services.Seguridad;

public interface IAuthService
{
    Task<AccesoResultado> ValidarAccesoAsync(string usuarioDominio, string? direccionIp);

    Task<List<PermisoUsuario>> ObtenerPermisosAsync(int usuarioId);
}
