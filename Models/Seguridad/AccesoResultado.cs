namespace calidad_app.Models.Seguridad;

public class AccesoResultado
{
    public bool Autorizado { get; set; }
    public string Resultado { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public int? UsuarioId { get; set; }
    public string? CodigoColaborador { get; set; }
    public string? NombreCompleto { get; set; }
    public string UsuarioDominio { get; set; } = string.Empty;
    public int? RolId { get; set; }
    public string? RolNombre { get; set; }
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }
    public bool? Activo { get; set; }
}

public static class ResultadoAcceso
{
    public const string Ok = "OK";
    public const string NoRegistrado = "NO_REGISTRADO";
    public const string Inactivo = "INACTIVO";
}
