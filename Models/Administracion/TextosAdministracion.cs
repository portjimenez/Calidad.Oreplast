namespace calidad_app.Models.Administracion;

/// <summary>
/// Textos visibles de los datos de seguridad que la base guarda sin acentos.
///
/// Los nombres de rol ("Ingenieria de Calidad"), los grupos de la matriz y las
/// acciones de la bitácora ("Modificacion") son literales de SQL Server, que el
/// proyecto escribe sin tildes para evitar problemas de codificación. La
/// pantalla los muestra en español correcto con este diccionario, igual que la
/// pantalla de catálogos hace con los nombres de sus pestañas.
///
/// Si llega un valor que no está aquí (un rol o una acción nuevos), se muestra
/// tal como viene: se ve sin tilde, pero no se pierde.
/// </summary>
public static class TextosAdministracion
{
    public static string Rol(string? nombre) => nombre switch
    {
        null or "" => "—",
        "Ingenieria de Calidad" => "Ingeniería de Calidad",
        "Jefe de Produccion" => "Jefe de Producción",
        "Gerente de Produccion" => "Gerente de Producción",
        _ => nombre
    };

    public static string Grupo(string grupo) => grupo switch
    {
        "Produccion" => "Producción",
        "Administracion" => "Administración",
        _ => grupo
    };

    public static string Accion(string accion) => accion switch
    {
        "Creacion" => "Creación",
        "Modificacion" => "Modificación",
        "CambioEstado" => "Cambio de estado",
        "Liberacion" => "Liberación",
        "Eliminacion" => "Eliminación",
        _ => accion
    };

    public static string Area(string? nombre) => nombre switch
    {
        null or "" => "Sin área",
        "Extrusion" => "Extrusión",
        "Impresion" => "Impresión",
        "Laminacion" => "Laminación",
        _ => nombre
    };
}
