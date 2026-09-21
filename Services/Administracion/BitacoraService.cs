using calidad_app.Data.Sp;
using calidad_app.Models.Administracion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Administracion;

public class BitacoraService(EjecutorSp sp, IUsuarioActual usuarioActual) : IBitacoraService
{
    public async Task<PaginaBitacora> ListarAsync(
        FiltroBitacora filtro, int pagina = 1, int tamanoPagina = 50,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        var resultado = await sp.ConsultarAsync(
            "aud.usp_Bitacora_Listar",
            cmd => cmd
                .Con("@UsuarioId", usuarioId)
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@UsuarioFiltroId", filtro.UsuarioId)
                .Con("@Accion", filtro.Accion)
                .Con("@Entidad", filtro.Entidad)
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@OcultarAccesosExitosos", filtro.OcultarAccesosExitosos)
                .Con("@Pagina", pagina)
                .Con("@TamanoPagina", tamanoPagina),
            async (lector, token) =>
            {
                // Primer conjunto: el total; segundo: la página.
                var total = await lector.ReadAsync(token) ? lector.Entero("Total") : 0;

                var eventos = await lector.NextResultAsync(token)
                    ? await lector.LeerListaAsync(MapeosAdministracion.EventoBitacora, token)
                    : [];

                return new PaginaBitacora
                {
                    Total = total,
                    Pagina = pagina,
                    TamanoPagina = tamanoPagina,
                    Eventos = eventos
                };
            },
            ct);

        return resultado;
    }

    public async Task<ResumenBitacora> ResumenAsync(
        DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "aud.usp_Bitacora_Resumen",
            cmd => cmd
                .Con("@UsuarioId", usuarioId)
                .Con("@FechaDesde", fechaDesde)
                .Con("@FechaHasta", fechaHasta),
            async (lector, token) =>
            {
                var resumen = new ResumenBitacora();

                if (await lector.ReadAsync(token))
                {
                    resumen.TotalEventos = lector.Entero("TotalEventos");
                    resumen.AccesosExitosos = lector.Entero("AccesosExitosos");
                    resumen.AccesosDenegados = lector.Entero("AccesosDenegados");
                    resumen.Operaciones = lector.Entero("Operaciones");
                    resumen.UsuariosDistintos = lector.Entero("UsuariosDistintos");
                }

                if (await lector.NextResultAsync(token))
                {
                    resumen.PorAccion =
                        await lector.LeerListaAsync(MapeosAdministracion.ConteoAccion, token);
                }

                return resumen;
            },
            ct);
    }
}
