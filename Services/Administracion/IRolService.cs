using calidad_app.Models.Administracion;

namespace calidad_app.Services.Administracion;

/// <summary>
/// La matriz de permisos por rol.
///
/// Los cinco roles son fijos (no se crean, renombran ni eliminan desde la
/// aplicación) y el catálogo de permisos es de solo lectura: cada clave está
/// escrita en el código, en un <c>[Authorize(Policy = "...")]</c> o en un
/// procedimiento, y un permiso creado desde la pantalla no protegería nada. Lo
/// único editable es qué permisos tiene cada rol.
///
/// La escritura exige GESTIONAR_USUARIOS y la base rechaza, sobre el conjunto
/// final, lo que rompería el proceso: mover los permisos reservados a
/// Ingeniería de Calidad, quitarle a uno mismo la administración de usuarios o
/// dejar sin permiso de inspeccionar a operadores con turnos programados.
/// </summary>
public interface IRolService
{
    Task<MatrizPermisos> ObtenerMatrizAsync(CancellationToken ct = default);

    /// <summary>
    /// Reemplaza los permisos de un rol por <paramref name="claves"/> (la columna
    /// completa, tal como quedó en pantalla). Se envía el conjunto y no casilla
    /// por casilla para que las reglas se evalúen sobre el resultado final.
    /// </summary>
    Task<ResultadoPermisos> GuardarPermisosAsync(
        int rolId, IEnumerable<string> claves, CancellationToken ct = default);
}
