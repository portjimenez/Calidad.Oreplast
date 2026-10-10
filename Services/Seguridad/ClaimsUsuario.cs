using System.Security.Claims;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Claims propios que SegUsuarioClaimsTransformation agrega a la identidad, y su lectura.
/// </summary>
public static class ClaimsUsuario
{
    /// <summary>Área del usuario en seg.Usuario; no existe si el usuario es transversal.</summary>
    public const string AreaId = "area_id";

    /// <summary>Nombre del área tal como está en cat.Area (sin tildes).</summary>
    public const string AreaNombre = "area_nombre";

    /// <summary>
    /// Presente cuando varios usuarios activos comparten la cuenta de dominio: la barra de
    /// título ofrece entonces "Cambiar de usuario" para el cambio de turno.
    /// </summary>
    public const string CuentaCompartida = "cuenta_compartida";

    public static bool TieneCuentaCompartida(this ClaimsPrincipal usuario) =>
        usuario.HasClaim(c => c.Type == CuentaCompartida);

    /// <summary>
    /// Área a la que pertenece el usuario, o null si no tiene (Calidad, Jefe y Gerente de
    /// Producción, Administrador). Las pantallas de captura la usan para mostrar solo las
    /// máquinas, procesos y registros de esa área; no es un permiso.
    /// </summary>
    public static int? AreaIdDe(this ClaimsPrincipal usuario) =>
        int.TryParse(usuario.FindFirst(AreaId)?.Value, out var areaId) ? areaId : null;

    public static string? AreaNombreDe(this ClaimsPrincipal usuario) =>
        usuario.FindFirst(AreaNombre)?.Value;
}
