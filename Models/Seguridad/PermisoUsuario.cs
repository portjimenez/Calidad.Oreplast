namespace calidad_app.Models.Seguridad;

public class PermisoUsuario
{
    public int PermisoId { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
