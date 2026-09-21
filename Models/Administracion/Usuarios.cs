using calidad_app.Models.Seguridad;

namespace calidad_app.Models.Administracion;

/// <summary>
/// Filtros de la pestaña Usuarios. Todos opcionales: en null no filtran.
/// </summary>
public class FiltroUsuarios
{
    public string? Busqueda { get; set; }
    public int? RolId { get; set; }
    public int? AreaId { get; set; }

    /// <summary>
    /// Null = todos, true = activos, false = inactivos. Se admite pedir solo los
    /// inactivos porque es donde se busca a quien hay que reactivar cuando vuelve
    /// a la planta.
    /// </summary>
    public bool? Activo { get; set; }
}

/// <summary>
/// Un usuario en la tabla maestra, con los datos derivados que el
/// Administrador necesita para decidir sin abrir otra pantalla.
/// </summary>
public class UsuarioResumen
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>Tal como llega de Windows Authentication: DOMINIO\usuario.</summary>
    public string UsuarioDominio { get; set; } = string.Empty;

    public int RolId { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }

    /// <summary>Último inicio de sesión exitoso según la bitácora. Null si nunca entró.</summary>
    public DateTime? UltimoAcceso { get; set; }

    /// <summary>
    /// Línea de su última asignación de operador (módulo 5). seg.Usuario solo
    /// guarda el área; la línea en la que se mueve la dice la programación.
    /// </summary>
    public string? LineaReciente { get; set; }

    /// <summary>
    /// Turnos programados de hoy en adelante que todavía no se trabajan. Mientras
    /// sea mayor que cero la base no deja darlo de baja ni quitarle el permiso de
    /// inspeccionar: la programación quedaría con alguien que no puede capturar.
    /// </summary>
    public int AsignacionesPendientes { get; set; }

    public bool TieneAsignacionesPendientes => AsignacionesPendientes > 0;

    public string RolVisible => TextosAdministracion.Rol(RolNombre);

    /// <summary>"Extrusión · LN-EXT-01", como en el mockup; solo el área si no tiene línea.</summary>
    public string Ubicacion =>
        AreaId is null && LineaReciente is null
            ? "—"
            : LineaReciente is null
                ? TextosAdministracion.Area(AreaNombre)
                : $"{TextosAdministracion.Area(AreaNombre)} · {LineaReciente}";
}

/// <summary>
/// Detalle de un usuario: lo del maestro más los permisos que le da su rol y
/// sus últimos movimientos en la bitácora.
/// </summary>
public class UsuarioDetalle : UsuarioResumen
{
    /// <summary>False si el área asignada ya se retiró del catálogo.</summary>
    public bool AreaActiva { get; set; } = true;

    /// <summary>
    /// Los permisos EFECTIVOS que le da su rol. Si el usuario está inactivo se
    /// devuelven igual, para que se vea lo que recuperaría al reactivarlo.
    /// </summary>
    public List<PermisoUsuario> Permisos { get; set; } = [];

    public List<EventoBitacora> Movimientos { get; set; } = [];
}

/// <summary>
/// Lo que se envía al guardar. <see cref="UsuarioId"/> en 0 significa alta.
/// </summary>
public class UsuarioEdicion
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Se puede escribir solo el usuario ("mlopez"): la base le antepone
    /// OREPLAST\ y normaliza mayúsculas.
    /// </summary>
    public string UsuarioDominio { get; set; } = string.Empty;

    public int RolId { get; set; }
    public int? AreaId { get; set; }

    public static UsuarioEdicion Desde(UsuarioResumen u) => new()
    {
        UsuarioId = u.UsuarioId,
        Codigo = u.Codigo,
        NombreCompleto = u.NombreCompleto,
        UsuarioDominio = u.UsuarioDominio,
        RolId = u.RolId,
        AreaId = u.AreaId
    };
}

/// <summary>Indicadores del encabezado de "Usuarios y roles".</summary>
public class ResumenAdministracion
{
    public int TotalUsuarios { get; set; }
    public int UsuariosActivos { get; set; }
    public int UsuariosInactivos { get; set; }
    public int Roles { get; set; }
    public int Permisos { get; set; }
    public int UsuariosConAccesoHoy { get; set; }

    /// <summary>
    /// Intentos de entrada rechazados en los últimos siete días. Una cuenta que
    /// insiste suele ser alguien nuevo al que falta darle de alta.
    /// </summary>
    public int AccesosDenegados7Dias { get; set; }
}
