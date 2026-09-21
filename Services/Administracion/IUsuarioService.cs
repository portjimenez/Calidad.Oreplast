using calidad_app.Models.Administracion;

namespace calidad_app.Services.Administracion;

/// <summary>
/// Usuarios del sistema: quién puede entrar y con qué rol.
///
/// Toda la escritura exige el permiso GESTIONAR_USUARIOS, que la base comprueba
/// en cada procedimiento; la pantalla nunca envía quién ejecuta la acción, lo
/// resuelve el servicio a partir de la identidad autenticada.
///
/// Las reglas que protegen al sistema de quedarse sin administrador (nadie se
/// cambia su propio rol ni se desactiva a sí mismo) y las que protegen la
/// programación de operadores viven en la base y no aquí: son las mismas para
/// cualquiera que escriba en <c>seg.Usuario</c>, y repetirlas en C# solo
/// abriría la puerta a que las dos versiones dejen de coincidir. La pantalla
/// solo las anticipa para no ofrecer un botón que va a terminar en error.
///
/// Un usuario nunca se borra: su id está en la bitácora, en los registros que
/// firmó y en la programación. Se desactiva.
/// </summary>
public interface IUsuarioService
{
    /// <summary>Indicadores del encabezado de "Usuarios y roles".</summary>
    Task<ResumenAdministracion> ResumenAsync(CancellationToken ct = default);

    Task<List<UsuarioResumen>> ListarAsync(FiltroUsuarios filtro, CancellationToken ct = default);

    /// <summary>
    /// Detalle con los permisos que le da su rol y sus últimos movimientos en la
    /// bitácora. Null si el usuario no existe.
    /// </summary>
    Task<UsuarioDetalle?> ObtenerAsync(int usuarioId, CancellationToken ct = default);

    /// <summary>
    /// Alta o modificación. Devuelve el id del usuario. La base normaliza la
    /// cuenta de dominio (le antepone OREPLAST\ si no trae dominio).
    /// </summary>
    Task<int> GuardarAsync(UsuarioEdicion usuario, CancellationToken ct = default);

    /// <summary>
    /// Baja o reactivación. La baja surte efecto de inmediato para escribir
    /// (ningún procedimiento acepta nada de un usuario inactivo) y en la
    /// siguiente carga de página para entrar.
    /// </summary>
    Task CambiarEstadoAsync(int usuarioId, bool activo, CancellationToken ct = default);
}
