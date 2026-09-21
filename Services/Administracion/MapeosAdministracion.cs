using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Administracion;
using calidad_app.Models.Seguridad;

namespace calidad_app.Services.Administracion;

/// <summary>
/// Traduce cada fila devuelta por los procedimientos del módulo de
/// administración a su modelo.
///
/// Igual que en los módulos anteriores, el mapeo es explícito columna por
/// columna y no por reflexión: si un procedimiento cambia el nombre de una
/// columna, el error aparece aquí, en un solo lugar y con nombre propio.
/// </summary>
internal static class MapeosAdministracion
{
    /* ---------- Usuarios ---------- */

    public static UsuarioResumen UsuarioResumen(DbDataReader r)
    {
        var usuario = LlenarUsuario(new UsuarioResumen(), r);
        usuario.LineaReciente = r.TextoNulo("LineaReciente");
        return usuario;
    }

    public static UsuarioDetalle UsuarioDetalle(DbDataReader r)
    {
        var usuario = LlenarUsuario(new UsuarioDetalle(), r);
        // Sin área no hay nada retirado que advertir.
        usuario.AreaActiva = r.BooleanoNulo("AreaActiva") ?? true;
        return usuario;
    }

    /// <summary>
    /// Columnas que comparten la lista y el detalle. <c>LineaReciente</c> solo
    /// la trae la lista: el detalle se abre desde una fila que ya la tiene.
    /// </summary>
    private static T LlenarUsuario<T>(T u, DbDataReader r) where T : UsuarioResumen
    {
        u.UsuarioId = r.Entero("UsuarioId");
        u.Codigo = r.Texto("Codigo");
        u.NombreCompleto = r.Texto("NombreCompleto");
        u.UsuarioDominio = r.Texto("UsuarioDominio");
        u.RolId = r.Entero("RolId");
        u.RolNombre = r.Texto("RolNombre");
        u.AreaId = r.EnteroNulo("AreaId");
        u.AreaNombre = r.TextoNulo("AreaNombre");
        u.Activo = r.Booleano("Activo");
        u.FechaCreacion = r.Fecha("FechaCreacion");
        u.UltimoAcceso = r.FechaNula("UltimoAcceso");
        u.AsignacionesPendientes = r.Entero("AsignacionesPendientes");
        return u;
    }

    public static PermisoUsuario Permiso(DbDataReader r) => new()
    {
        PermisoId = r.Entero("PermisoId"),
        Clave = r.Texto("Clave"),
        Descripcion = r.TextoNulo("Descripcion")
    };

    public static ResumenAdministracion ResumenAdministracion(DbDataReader r) => new()
    {
        TotalUsuarios = r.Entero("TotalUsuarios"),
        UsuariosActivos = r.Entero("UsuariosActivos"),
        UsuariosInactivos = r.Entero("UsuariosInactivos"),
        Roles = r.Entero("Roles"),
        Permisos = r.Entero("Permisos"),
        UsuariosConAccesoHoy = r.Entero("UsuariosConAccesoHoy"),
        AccesosDenegados7Dias = r.Entero("AccesosDenegados7Dias")
    };

    /* ---------- Roles y permisos ---------- */

    public static RolMatriz RolMatriz(DbDataReader r) => new()
    {
        RolId = r.Entero("RolId"),
        Nombre = r.Texto("Nombre"),
        Descripcion = r.TextoNulo("Descripcion"),
        Usuarios = r.Entero("Usuarios"),
        UsuariosActivos = r.Entero("UsuariosActivos"),
        Permisos = r.Entero("Permisos"),
        EsRolPropio = r.Booleano("EsRolPropio")
    };

    public static PermisoMatriz PermisoMatriz(DbDataReader r) => new()
    {
        PermisoId = r.Entero("PermisoId"),
        Clave = r.Texto("Clave"),
        Descripcion = r.TextoNulo("Descripcion"),
        Grupo = r.Texto("Grupo"),
        OrdenGrupo = r.Entero("OrdenGrupo"),
        RolReservadoId = r.EnteroNulo("RolReservadoId"),
        RolReservadoNombre = r.TextoNulo("RolReservadoNombre")
    };

    public static (int RolId, int PermisoId) Asignacion(DbDataReader r) =>
        (r.Entero("RolId"), r.Entero("PermisoId"));

    public static async Task<ResultadoPermisos> LeerResultadoPermisosAsync(
        DbDataReader lector, CancellationToken ct) =>
        await lector.ReadAsync(ct)
            ? new ResultadoPermisos(lector.Entero("Agregados"), lector.Entero("Quitados"))
            : new ResultadoPermisos(0, 0);

    /* ---------- Bitácora ---------- */

    public static EventoBitacora EventoBitacora(DbDataReader r) => new()
    {
        BitacoraId = r.EnteroLargo("BitacoraId"),
        FechaHora = r.Fecha("FechaHora"),
        UsuarioId = r.EnteroNulo("UsuarioId"),
        UsuarioCodigo = r.TextoNulo("UsuarioCodigo"),
        UsuarioNombre = r.TextoNulo("UsuarioNombre"),
        UsuarioDominio = r.TextoNulo("UsuarioDominio"),
        Accion = r.Texto("Accion"),
        Entidad = r.TextoNulo("Entidad"),
        EntidadId = r.TextoNulo("EntidadId"),
        Detalle = r.TextoNulo("Detalle"),
        DireccionIp = r.TextoNulo("DireccionIp")
    };

    /// <summary>
    /// Los movimientos del detalle de usuario son filas de la misma tabla, pero
    /// sin las columnas del usuario: ya se sabe de quién son.
    /// </summary>
    public static EventoBitacora MovimientoUsuario(DbDataReader r) => new()
    {
        BitacoraId = r.EnteroLargo("BitacoraId"),
        FechaHora = r.Fecha("FechaHora"),
        Accion = r.Texto("Accion"),
        Entidad = r.TextoNulo("Entidad"),
        EntidadId = r.TextoNulo("EntidadId"),
        Detalle = r.TextoNulo("Detalle"),
        DireccionIp = r.TextoNulo("DireccionIp")
    };

    public static ConteoAccion ConteoAccion(DbDataReader r) =>
        new(r.Texto("Accion"), r.Entero("Total"));

    /* ---------- Selectores ---------- */

    public static RolOpcion RolOpcion(DbDataReader r) => new()
    {
        RolId = r.Entero("RolId"),
        Nombre = r.Texto("Nombre"),
        Descripcion = r.TextoNulo("Descripcion")
    };

    public static UsuarioOpcion UsuarioOpcion(DbDataReader r) => new()
    {
        UsuarioId = r.Entero("UsuarioId"),
        Codigo = r.Texto("Codigo"),
        NombreCompleto = r.Texto("NombreCompleto"),
        UsuarioDominio = r.Texto("UsuarioDominio"),
        Activo = r.Booleano("Activo")
    };

    public static async Task<int> LeerIdAsync(DbDataReader lector, CancellationToken ct) =>
        await lector.ReadAsync(ct) ? lector.GetInt32(0) : 0;
}
