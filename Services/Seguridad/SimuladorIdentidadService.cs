using calidad_app.Data;
using calidad_app.Models.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Desarrollo: lista los usuarios de seg.Usuario disponibles para simular en el selector de la
/// topbar. La cuenta activa la decide el esquema "Simulacion" (cookie simulacion_usuario, ver
/// SimulacionAuthenticationHandler); este servicio solo alimenta el &lt;select&gt;.
/// </summary>
public class SimuladorIdentidadService(IDbContextFactory<AppDbContext> dbFactory, IConfiguration configuration)
{
    private const string RolOperador = "Operador";

    /// <param name="usuarioActual">
    /// Cuenta que se está simulando: siempre se incluye, aunque no sea un operador de
    /// demostración, para que el selector no pierda la selección actual.
    /// </param>
    public async Task<List<UsuarioSimulado>> ObtenerUsuariosDisponiblesAsync(string? usuarioActual = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var usuarios = await db.UsuariosSimulados
            .FromSqlRaw("""
                SELECT u.UsuarioId, u.Codigo, u.NombreCompleto, u.UsuarioDominio,
                       r.Nombre AS RolNombre, a.Nombre AS AreaNombre
                FROM seg.Usuario u
                JOIN seg.Rol r ON r.RolId = u.RolId
                LEFT JOIN cat.Area a ON a.AreaId = u.AreaId
                WHERE u.Activo = 1
                ORDER BY r.RolId, a.AreaId, u.NombreCompleto
                """)
            .AsNoTracking()
            .ToListAsync();

        // Con decenas de operadores reales el selector se vuelve inmanejable en una
        // demostración. Simulacion:OperadoresDemostracion deja uno por área; sin esa lista
        // se muestran todos. Los demás roles son pocos y se muestran siempre.
        var demostracion = configuration
            .GetSection("Simulacion:OperadoresDemostracion")
            .Get<string[]>() ?? [];

        if (demostracion.Length == 0)
        {
            return usuarios;
        }

        var visibles = new HashSet<string>(demostracion, StringComparer.OrdinalIgnoreCase);

        return usuarios
            .Where(u => u.RolNombre != RolOperador
                        || visibles.Contains(u.UsuarioDominio)
                        || string.Equals(u.UsuarioDominio, usuarioActual, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
