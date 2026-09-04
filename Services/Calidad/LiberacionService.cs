using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Calidad;

public class LiberacionService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : ILiberacionService
{
    public Task<EstadoLiberacion?> ObtenerAsync(int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Liberacion_Obtener",
            cmd => cmd.Con("@RegistroId", registroId),
            LeerEstadoAsync,
            ct);

    public async Task<ResultadoLiberacion> RegistrarAsync(
        int registroId,
        string tipo,
        int? loteId = null,
        bool calidadVerificada = true,
        bool inocuidadVerificada = true,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Liberacion_Registrar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@Tipo", tipo)
                .Con("@UsuarioId", usuarioId)
                .Con("@CalidadVerificada", calidadVerificada)
                .Con("@InocuidadVerificada", inocuidadVerificada)
                .Con("@LoteId", loteId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosLotes.ResultadoLiberacion, token) ?? new(),
            ct);
    }

    public async Task<EstadoLiberacion?> GuardarCierreAsync(
        int registroId,
        string? comentarios = null,
        decimal? kgProductoNoConforme = null,
        string? razonNoConforme = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_CierreOrden_Guardar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Comentarios", comentarios)
                .Con("@KgProductoNoConforme", kgProductoNoConforme)
                .Con("@RazonNoConforme", razonNoConforme)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerEstadoAsync,
            ct);
    }

    /// <summary>
    /// Los tres conjuntos del estado de liberación. Lo comparten obtener y
    /// guardar el cierre, porque el guardado devuelve el estado completo.
    /// </summary>
    private static async Task<EstadoLiberacion?> LeerEstadoAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var estado = await lector.LeerUnoAsync(MapeosLotes.EstadoLiberacion, ct);

        if (estado is null)
        {
            return null;
        }

        if (await lector.NextResultAsync(ct))
        {
            estado.Firmas = await lector.LeerListaAsync(MapeosLotes.Firma, ct);
        }

        // Tercer resultado: la sección Cierre de orden, si ya se capturó.
        if (await lector.NextResultAsync(ct))
        {
            estado.Cierre = await lector.LeerUnoAsync(MapeosLotes.CierreOrden, ct);
        }

        return estado;
    }
}
