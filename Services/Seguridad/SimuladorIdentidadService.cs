using calidad_app.Data;
using calidad_app.Models.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Desarrollo: lista los usuarios de seg.Usuario disponibles para simular en el selector de la
/// topbar. La cuenta activa la decide el esquema "Simulacion" (cookie simulacion_usuario, ver
/// SimulacionAuthenticationHandler); este servicio solo alimenta el &lt;select&gt;.
/// </summary>
public class SimuladorIdentidadService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<UsuarioSimulado>> ObtenerUsuariosDisponiblesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.UsuariosSimulados
            .FromSqlRaw("""
                SELECT u.UsuarioId, u.Codigo, u.NombreCompleto, u.UsuarioDominio, r.Nombre AS RolNombre
                FROM seg.Usuario u
                JOIN seg.Rol r ON r.RolId = u.RolId
                WHERE u.Activo = 1
                ORDER BY r.RolId, u.NombreCompleto
                """)
            .AsNoTracking()
            .ToListAsync();
    }
}
