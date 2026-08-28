using calidad_app.Data.Sp;
using calidad_app.Models.Inspeccion;

namespace calidad_app.Services.Inspeccion;

/// <summary>Catálogos de apoyo de los formularios de inspección.</summary>
public interface ICatalogoInspeccionService
{
    /// <param name="areaId">
    /// Acota los operadores al área del registro; en null devuelve todos.
    /// </param>
    Task<CatalogosInspeccion> ObtenerAsync(int? areaId = null, CancellationToken ct = default);
}

public class CatalogoInspeccionService(EjecutorSp sp) : ICatalogoInspeccionService
{
    public Task<CatalogosInspeccion> ObtenerAsync(int? areaId = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cat.usp_Catalogos_Inspeccion",
            cmd => cmd.Con("@AreaId", areaId),
            async (lector, token) =>
            {
                var catalogos = new CatalogosInspeccion
                {
                    Materiales = await lector.LeerListaAsync(r => new MaterialCatalogo
                    {
                        MaterialId = r.Entero("MaterialId"),
                        Codigo = r.Texto("Codigo"),
                        Nombre = r.Texto("Nombre")
                    }, token)
                };

                // Segundo resultado: razones de tiempo muerto.
                await lector.NextResultAsync(token);
                catalogos.Razones = await lector.LeerListaAsync(r => new RazonTiempoMuerto
                {
                    RazonId = r.Entero("RazonId"),
                    Nombre = r.Texto("Nombre")
                }, token);

                // Tercer resultado: operadores activos del área.
                await lector.NextResultAsync(token);
                catalogos.Operadores = await lector.LeerListaAsync(r => new OperadorCatalogo
                {
                    UsuarioId = r.Entero("UsuarioId"),
                    Codigo = r.Texto("Codigo"),
                    NombreCompleto = r.Texto("NombreCompleto")
                }, token);

                // Cuarto resultado: turnos.
                await lector.NextResultAsync(token);
                catalogos.Turnos = await lector.LeerListaAsync(r => new TurnoCatalogo
                {
                    TurnoId = r.Entero("TurnoId"),
                    Nombre = r.Texto("Nombre")
                }, token);

                // Quinto resultado: máquinas activas (de todas las áreas).
                await lector.NextResultAsync(token);
                catalogos.Maquinas = await lector.LeerListaAsync(r => new MaquinaCatalogo
                {
                    MaquinaId = r.Entero("MaquinaId"),
                    Codigo = r.Texto("Codigo"),
                    Nombre = r.Texto("Nombre"),
                    AreaId = r.Entero("AreaId"),
                    AreaNombre = r.Texto("AreaNombre")
                }, token);

                // Sexto resultado: tipos de registro.
                await lector.NextResultAsync(token);
                catalogos.TiposRegistro = await lector.LeerListaAsync(r => new TipoRegistroCatalogo
                {
                    TipoRegistroId = r.Entero("TipoRegistroId"),
                    Codigo = r.Texto("Codigo"),
                    Nombre = r.Texto("Nombre"),
                    AreaId = r.Entero("AreaId")
                }, token);

                return catalogos;
            },
            ct);
}
