namespace calidad_app.Models.Administracion;

/// <summary>
/// La matriz de permisos: los permisos en las filas y los cinco roles en las
/// columnas. La base la entrega en tres listas (roles, permisos y casillas
/// marcadas) y aquí se cruzan, para no depender de cuántos roles haya.
/// </summary>
public class MatrizPermisos
{
    public List<RolMatriz> Roles { get; set; } = [];
    public List<PermisoMatriz> Permisos { get; set; } = [];

    /// <summary>Casillas marcadas, como pares (RolId, PermisoId).</summary>
    public HashSet<(int RolId, int PermisoId)> Asignaciones { get; set; } = [];

    public bool Tiene(int rolId, int permisoId) => Asignaciones.Contains((rolId, permisoId));

    /// <summary>Permisos agrupados por área funcional, en el orden que fija la base.</summary>
    public IEnumerable<IGrouping<string, PermisoMatriz>> PorGrupo() =>
        Permisos.GroupBy(p => p.Grupo);
}

public class RolMatriz
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Usuarios { get; set; }
    public int UsuariosActivos { get; set; }
    public int Permisos { get; set; }

    /// <summary>
    /// El rol de quien está consultando. En esa columna GESTIONAR_USUARIOS va
    /// bloqueado: nadie puede quitarle a su propio rol la administración de
    /// usuarios.
    /// </summary>
    public bool EsRolPropio { get; set; }

    public string NombreVisible => TextosAdministracion.Rol(Nombre);
}

public class PermisoMatriz
{
    public int PermisoId { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Grupo { get; set; } = string.Empty;
    public int OrdenGrupo { get; set; }

    /// <summary>
    /// No es null en los permisos reservados a un solo rol (liberar producto y
    /// generar certificados, de Ingeniería de Calidad): esa fila entera va
    /// bloqueada.
    /// </summary>
    public int? RolReservadoId { get; set; }
    public string? RolReservadoNombre { get; set; }

    public bool EsReservado => RolReservadoId is not null;
}

/// <summary>Lo que cambió al guardar la columna de un rol.</summary>
public record ResultadoPermisos(int Agregados, int Quitados)
{
    public bool HuboCambios => Agregados + Quitados > 0;
}
