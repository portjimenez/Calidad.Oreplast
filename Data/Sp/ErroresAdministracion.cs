namespace calidad_app.Data.Sp;

/// <summary>
/// Mensajes de los errores que lanzan los procedimientos del módulo de
/// administración (rangos 50400-50459).
///
/// Sigue la misma idea que los catálogos de los módulos anteriores: el mensaje
/// que ve el usuario se redacta aquí, en español y con tildes, y se busca por
/// número, de modo que cambiar la redacción no obliga a tocar la base.
/// </summary>
internal static class ErroresAdministracion
{
    internal static readonly Dictionary<int, string> Mensajes = new()
    {
        // Usuarios
        [50400] = "No tiene autorización para administrar usuarios.",
        [50401] = "El código de colaborador es obligatorio.",
        [50402] = "El nombre completo es obligatorio.",
        [50403] = "La cuenta de dominio es obligatoria.",
        [50404] = "La cuenta de dominio debe tener el formato DOMINIO\\usuario (por ejemplo OREPLAST\\mlopez).",
        [50405] = "El usuario indicado no existe.",
        [50406] = "Ya existe otro usuario con ese código de colaborador.",
        [50407] = "Ya existe otro usuario con esa cuenta de dominio.",
        [50408] = "El rol indicado no existe.",
        [50409] = "El área indicada no existe o está inactiva.",
        [50410] = "No puede cambiar su propio rol.",
        [50411] = "No puede desactivar su propio usuario.",
        [50413] = "El usuario tiene turnos de operador programados que aún no se trabajan. "
                + "El Jefe de Producción debe reasignarlos antes de continuar.",
        [50414] = "El usuario ya se encuentra en ese estado.",

        // Roles y permisos
        [50421] = "La lista de permisos enviada no es válida.",
        [50422] = "La lista incluye un permiso que no existe.",
        [50423] = "Liberar producto y generar certificados son permisos exclusivos de "
                + "Ingeniería de Calidad: no se pueden dar a otro rol ni quitárselos.",
        [50424] = "No puede quitarle a su propio rol el permiso de administrar usuarios.",
        [50425] = "Hay usuarios de este rol con turnos de operador programados; no se le puede "
                + "quitar el permiso de registrar inspecciones hasta que se reasignen.",

        // Bitácora
        [50440] = "No tiene autorización para consultar la bitácora.",
        [50441] = "Debe indicar el rango de fechas.",
        [50442] = "La fecha inicial no puede ser posterior a la final.",
        [50443] = "El rango de fechas no puede superar un año.",
        [50444] = "La página solicitada no es válida."
    };
}
