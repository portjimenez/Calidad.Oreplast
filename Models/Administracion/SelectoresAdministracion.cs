using calidad_app.Models.Catalogos;

namespace calidad_app.Models.Administracion;

/// <summary>
/// Listas que alimentan los combos de las dos pantallas del módulo: rol y área
/// en el formulario de usuario; usuario, acción y entidad en los filtros de la
/// bitácora. Se cargan de una sola llamada al abrir la pantalla.
///
/// Áreas y usuarios incluyen los INACTIVOS, marcados: al abrir un usuario
/// asignado a un área ya retirada el combo tiene que poder mostrarla, y en la
/// bitácora interesa encontrar lo que hizo alguien que ya se fue.
///
/// Reutiliza <see cref="AreaOpcion"/> del módulo de catálogos: es la misma
/// entidad y duplicar la clase solo obligaría a mantener dos mapeos.
/// </summary>
public class SelectoresAdministracion
{
    public List<RolOpcion> Roles { get; set; } = [];
    public List<AreaOpcion> Areas { get; set; } = [];
    public List<UsuarioOpcion> Usuarios { get; set; } = [];

    /// <summary>Acciones que existen en la bitácora (las agrega cada módulo).</summary>
    public List<string> Acciones { get; set; } = [];

    /// <summary>Tablas afectadas que existen en la bitácora.</summary>
    public List<string> Entidades { get; set; } = [];
}

public class RolOpcion
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public string NombreVisible => TextosAdministracion.Rol(Nombre);
}

public class UsuarioOpcion
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string UsuarioDominio { get; set; } = string.Empty;
    public bool Activo { get; set; }

    public string Etiqueta => Activo
        ? $"{NombreCompleto} ({Codigo})"
        : $"{NombreCompleto} ({Codigo}) — inactivo";
}
