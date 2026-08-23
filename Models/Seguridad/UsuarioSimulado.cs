namespace calidad_app.Models.Seguridad;

public class UsuarioSimulado
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string UsuarioDominio { get; set; } = string.Empty;
    public string RolNombre { get; set; } = string.Empty;
}
