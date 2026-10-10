namespace calidad_app.Services.Seguridad;

/// <summary>
/// Cuenta de dominio compartida: los operadores que rotan por turnos en la misma PC entran
/// con la misma cuenta de Windows, y Windows Authentication solo le dice a la aplicación la
/// cuenta, no la persona. La persona se elige en "¿Quién está trabajando?" y se recuerda en
/// esta cookie hasta cerrar el navegador o pulsar "Cambiar de usuario".
///
/// La cookie solo guarda el UsuarioId y no se cifra a propósito: no da ningún poder, porque
/// seg.usp_Usuario_ValidarAcceso la acepta solo si es un usuario activo de la MISMA cuenta
/// que Windows autenticó, es decir, alguien que la persona ya podía elegir en la pantalla.
/// </summary>
public static class PerfilElegido
{
    public const string Cookie = "oreplast_perfil";

    public static int? Leer(HttpRequest request) =>
        int.TryParse(request.Cookies[Cookie], out var usuarioId) ? usuarioId : null;

    public static void Guardar(HttpContext contexto, int usuarioId) =>
        contexto.Response.Cookies.Append(Cookie, usuarioId.ToString(), new CookieOptions
        {
            // Sin Expires: cookie de sesión. Al cerrar el navegador se vuelve a preguntar.
            HttpOnly = true,
            Secure = contexto.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
        });

    public static void Borrar(HttpContext contexto) =>
        contexto.Response.Cookies.Delete(Cookie);

    /// <summary>
    /// Destino de la redirección después de elegir: solo rutas locales ("/algo"), para que el
    /// formulario no se pueda usar como redirección abierta hacia otro sitio.
    /// </summary>
    public static string DestinoSeguro(string? volver) =>
        volver is { Length: > 0 } && volver[0] == '/'
            && !volver.StartsWith("//") && !volver.StartsWith("/\\")
            ? volver
            : "/";
}
