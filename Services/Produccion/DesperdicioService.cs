using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Produccion;

namespace calidad_app.Services.Produccion;

public class DesperdicioService(EjecutorSp sp) : IDesperdicioService
{
    public Task<ResumenDesperdicio> ResumenAsync(
        FiltroDesperdicio filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Desperdicio_Resumen",
            cmd => Filtrar(cmd, filtro),
            LeerResumenAsync,
            ct);

    public Task<List<RegistroDesperdicio>> ListarAsync(
        FiltroDesperdicio filtro, bool soloIncumplen = false, int maxFilas = 500,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Desperdicio_Listar",
            cmd => Filtrar(cmd, filtro)
                .Con("@SoloIncumplen", soloIncumplen)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosProduccion.RegistroDesperdicio, token),
            ct);

    public Task<List<GrupoDesperdicio>> AgruparAsync(
        FiltroDesperdicio filtro, string agrupar, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Desperdicio_Agrupado",
            cmd => Filtrar(cmd, filtro).Con("@Agrupar", agrupar),
            (lector, token) => lector.LeerListaAsync(MapeosProduccion.GrupoDesperdicio, token),
            ct);

    public Task<List<ParoPorRazon>> TiempoMuertoAsync(
        FiltroDesperdicio filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_TiempoMuerto_Resumen",
            cmd => Filtrar(cmd, filtro),
            (lector, token) => lector.LeerListaAsync(MapeosProduccion.ParoPorRazon, token),
            ct);

    /// <summary>
    /// Los cuatro procedimientos aceptan el mismo ámbito, así que los
    /// parámetros se arman una sola vez: si mañana se agrega una dimensión al
    /// filtro, se agrega aquí y las cuatro consultas la reciben.
    /// </summary>
    private static DbCommand Filtrar(DbCommand cmd, FiltroDesperdicio filtro) => cmd
        .Con("@FechaDesde", filtro.FechaDesde)
        .Con("@FechaHasta", filtro.FechaHasta)
        .Con("@AreaId", filtro.AreaId)
        .Con("@LineaId", filtro.LineaId)
        .Con("@MaquinaId", filtro.MaquinaId)
        .Con("@TurnoId", filtro.TurnoId)
        .Con("@ProductoId", filtro.ProductoId)
        .Con("@OrdenId", filtro.OrdenId);

    /// <summary>
    /// El resumen llega en dos conjuntos: los totales y una fila por concepto
    /// de meta. Se juntan en un solo objeto porque la pantalla los muestra
    /// como una sola fila de tarjetas.
    /// </summary>
    private static async Task<ResumenDesperdicio> LeerResumenAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var resumen = await lector.LeerUnoAsync(MapeosProduccion.ResumenDesperdicio, ct)
                      ?? new ResumenDesperdicio();

        if (await lector.NextResultAsync(ct))
        {
            resumen.Metas = await lector.LeerListaAsync(MapeosProduccion.MetaComparada, ct);
        }

        return resumen;
    }
}
