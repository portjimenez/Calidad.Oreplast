using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Produccion;
using calidad_app.Services.Catalogos;

namespace calidad_app.Services.Produccion;

public class CatalogoProduccionService(EjecutorSp sp) : ICatalogoProduccionService
{
    public Task<SelectoresProduccion> ObtenerSelectoresAsync(
        string? busquedaProducto = null, bool incluirInactivos = false,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Produccion_Selectores",
            cmd => cmd
                .Con("@BusquedaProducto", busquedaProducto)
                .Con("@IncluirInactivos", incluirInactivos),
            LeerSelectoresAsync,
            ct);

    /// <summary>
    /// Nueve conjuntos de resultados, en el mismo orden en que los devuelve el
    /// procedimiento. Las opciones de área, línea, máquina, producto y lista
    /// cerrada se mapean con <see cref="MapeosCatalogos"/>: son las mismas
    /// entidades que administra el módulo 4 y duplicar el mapeo solo
    /// obligaría a mantener dos versiones en paralelo.
    /// </summary>
    private static async Task<SelectoresProduccion> LeerSelectoresAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var selectores = new SelectoresProduccion
        {
            EstadosOrden = await lector.LeerListaAsync(MapeosCatalogos.OpcionClave, ct)
        };

        if (await lector.NextResultAsync(ct))
        {
            selectores.Clientes = await lector.LeerListaAsync(MapeosProduccion.ClienteOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Areas = await lector.LeerListaAsync(MapeosCatalogos.AreaOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Lineas = await lector.LeerListaAsync(MapeosCatalogos.LineaOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Maquinas = await lector.LeerListaAsync(MapeosCatalogos.MaquinaOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Turnos = await lector.LeerListaAsync(MapeosProduccion.TurnoOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Operadores = await lector.LeerListaAsync(MapeosProduccion.OperadorOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Razones = await lector.LeerListaAsync(MapeosProduccion.RazonOpcion, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            selectores.Productos = await lector.LeerListaAsync(MapeosCatalogos.ProductoOpcion, ct);
        }

        return selectores;
    }
}
