using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Administracion;
using calidad_app.Services.Catalogos;

namespace calidad_app.Services.Administracion;

public class CatalogoAdministracionService(EjecutorSp sp) : ICatalogoAdministracionService
{
    public Task<SelectoresAdministracion> ObtenerSelectoresAsync(CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "seg.usp_Administracion_Selectores",
            _ => { },
            LeerSelectoresAsync,
            ct);

    /// <summary>
    /// Cinco conjuntos, en el orden en que los devuelve el procedimiento. Las
    /// áreas se mapean con <see cref="MapeosCatalogos"/>: son las mismas que
    /// administra el módulo 4.
    /// </summary>
    private static async Task<SelectoresAdministracion> LeerSelectoresAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var selectores = new SelectoresAdministracion
        {
            Roles = await lector.LeerListaAsync(MapeosAdministracion.RolOpcion, ct)
        };

        if (await lector.NextResultAsync(ct))
        {
            selectores.Areas = await lector.LeerListaAsync(MapeosCatalogos.AreaOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Usuarios = await lector.LeerListaAsync(MapeosAdministracion.UsuarioOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Acciones = await lector.LeerListaAsync(r => r.Texto("Accion"), ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Entidades = await lector.LeerListaAsync(r => r.Texto("Entidad"), ct);
        }

        return selectores;
    }
}
