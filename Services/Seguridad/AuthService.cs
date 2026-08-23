using calidad_app.Data;
using calidad_app.Models.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace calidad_app.Services.Seguridad;

public class AuthService(IDbContextFactory<AppDbContext> dbFactory) : IAuthService
{
    public async Task<AccesoResultado> ValidarAccesoAsync(string usuarioDominio, string? direccionIp)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var resultado = await db.AccesoResultados
            .FromSqlInterpolated(
                $"EXEC seg.usp_Usuario_ValidarAcceso @UsuarioDominio = {usuarioDominio}, @DireccionIp = {direccionIp}")
            .AsNoTracking()
            .ToListAsync();

        return resultado.Single();
    }

    public async Task<List<PermisoUsuario>> ObtenerPermisosAsync(int usuarioId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.PermisosUsuario
            .FromSqlInterpolated($"EXEC seg.usp_Usuario_ObtenerPermisos @UsuarioId = {usuarioId}")
            .AsNoTracking()
            .ToListAsync();
    }
}
