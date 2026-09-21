using calidad_app.Models.Administracion;

namespace calidad_app.Services.Administracion;

/// <summary>
/// Listas que alimentan los combos de "Usuarios y roles" y de la bitácora. Una
/// sola llamada al abrir la pantalla, igual que en los módulos anteriores.
/// </summary>
public interface ICatalogoAdministracionService
{
    Task<SelectoresAdministracion> ObtenerSelectoresAsync(CancellationToken ct = default);
}
