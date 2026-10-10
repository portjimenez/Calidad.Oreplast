namespace calidad_app.Models.Seguridad;

/// <summary>
/// Usuario activo que comparte una cuenta de dominio (seg.usp_Usuario_PerfilesDeCuenta):
/// una opción de la pantalla "¿Quién está trabajando?".
/// </summary>
public class PerfilCuenta
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string RolNombre { get; set; } = string.Empty;
    public string? AreaNombre { get; set; }
}
