using calidad_app.Models.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace calidad_app.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AccesoResultado> AccesoResultados => Set<AccesoResultado>();
    public DbSet<PermisoUsuario> PermisosUsuario => Set<PermisoUsuario>();
    public DbSet<UsuarioSimulado> UsuariosSimulados => Set<UsuarioSimulado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DTOs alimentados solo por procedimientos almacenados (sin tabla propia).
        modelBuilder.Entity<AccesoResultado>().HasNoKey();
        modelBuilder.Entity<PermisoUsuario>().HasNoKey();
        modelBuilder.Entity<UsuarioSimulado>().HasNoKey();
    }
}
