using System.Data.Common;
using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Calidad;

public class FichaTecnicaService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IFichaTecnicaService
{
    public Task<List<ProductoFicha>> ListarAsync(
        FiltroFichas filtro, int maxFilas = 200, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Ficha_Listar",
            cmd => cmd
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@Estado", filtro.Estado)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosFichas.ProductoFicha, token),
            ct);

    public Task<FichaProducto?> ObtenerAsync(
        int productoId, int? fichaId = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_Ficha_Obtener",
            cmd => cmd
                .Con("@ProductoId", productoId)
                .Con("@FichaId", fichaId),
            LeerDetalleAsync,
            ct);

    public async Task<int> CrearVersionAsync(
        int productoId, int? copiarDeFichaId = null, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_Ficha_CrearVersion",
            cmd => cmd
                .Con("@ProductoId", productoId)
                .Con("@UsuarioId", usuarioId)
                .Con("@CopiarDeFichaId", copiarDeFichaId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosFichas.LeerIdAsync,
            ct);
    }

    public async Task GuardarBorradorAsync(
        int fichaId, DateOnly vigenteDesde, IEnumerable<ToleranciaEdicion> tolerancias,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        // [{"ParametroId":15,"ValorObjetivo":80,"LimiteInferior":76,"LimiteSuperior":84}, ...]
        // Los nombres de propiedad del record coinciden con los que lee OPENJSON.
        var json = JsonSerializer.Serialize(tolerancias.ToList());

        await sp.EjecutarAsync(
            "cal.usp_Ficha_GuardarBorrador",
            cmd => cmd
                .Con("@FichaId", fichaId)
                .Con("@VigenteDesde", vigenteDesde)
                .Con("@ToleranciasJson", json)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    public Task PublicarAsync(int fichaId, CancellationToken ct = default) =>
        EjecutarSobreFichaAsync("cal.usp_Ficha_Publicar", fichaId, ct);

    public Task RetirarPublicacionAsync(int fichaId, CancellationToken ct = default) =>
        EjecutarSobreFichaAsync("cal.usp_Ficha_RetirarPublicacion", fichaId, ct);

    public Task EliminarBorradorAsync(int fichaId, CancellationToken ct = default) =>
        EjecutarSobreFichaAsync("cal.usp_Ficha_EliminarBorrador", fichaId, ct);

    /// <summary>Los tres procedimientos de estado reciben exactamente lo mismo.</summary>
    private async Task EjecutarSobreFichaAsync(string procedimiento, int fichaId, CancellationToken ct)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            procedimiento,
            cmd => cmd
                .Con("@FichaId", fichaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /// <summary>
    /// Tres conjuntos: el producto, sus versiones y las tolerancias de la
    /// versión elegida. Un primer conjunto vacío significa que el producto no
    /// existe.
    /// </summary>
    private static async Task<FichaProducto?> LeerDetalleAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var ficha = await lector.LeerUnoAsync(MapeosFichas.FichaProducto, ct);

        if (ficha is null)
        {
            return null;
        }

        if (await lector.NextResultAsync(ct))
        {
            ficha.Versiones = await lector.LeerListaAsync(MapeosFichas.VersionFicha, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            ficha.Tolerancias = await lector.LeerListaAsync(MapeosFichas.ToleranciaFicha, ct);
        }

        return ficha;
    }
}
