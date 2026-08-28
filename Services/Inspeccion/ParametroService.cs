using System.Data.Common;
using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Inspeccion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Inspeccion;

public class ParametroService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IParametroService
{
    public Task<FichaAplicable> ObtenerFichaAplicableAsync(
        int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_FichaTecnica_ObtenerTolerancias",
            cmd => cmd.Con("@RegistroId", registroId),
            async (lector, token) =>
            {
                var ficha = await lector.LeerUnoAsync(MapeosInspeccion.Ficha, token)
                            ?? new FichaAplicable();

                // Segundo resultado: los parámetros aplicables con sus límites.
                await lector.NextResultAsync(token);
                ficha.Parametros = await lector.LeerListaAsync(MapeosInspeccion.Tolerancia, token);

                return ficha;
            },
            ct);

    public Task<List<ParametroMedicion>> ListarAsync(
        int registroId,
        string ambito = AmbitoParametro.Registro,
        int? bobinaId = null,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_RegistroParametro_Listar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@Ambito", ambito)
                .Con("@BobinaId", bobinaId),
            (lector, token) => lector.LeerListaAsync(MapeosInspeccion.Parametro, token),
            ct);

    public async Task<EvaluacionResumen> GuardarAsync(
        int registroId,
        IEnumerable<ValorParametro> valores,
        int? bobinaId = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();
        var json = JsonSerializer.Serialize(valores);

        return await sp.ConsultarAsync(
            "prod.usp_RegistroParametro_Guardar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@ValoresJson", json)
                .Con("@BobinaId", bobinaId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerEvaluacionAsync,
            ct);
    }

    public Task<EvaluacionResumen> ReevaluarAsync(
        int registroId, int? bobinaId = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_RegistroParametro_Evaluar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@BobinaId", bobinaId)
                .Con("@GenerarAlertas", true),
            LeerEvaluacionAsync,
            ct);

    private static async Task<EvaluacionResumen> LeerEvaluacionAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var evaluacion = await lector.LeerUnoAsync(MapeosInspeccion.Evaluacion, ct)
                         ?? new EvaluacionResumen();

        // Segundo resultado: el detalle de las desviaciones, para mostrarlas al instante.
        if (await lector.NextResultAsync(ct))
        {
            evaluacion.Desviaciones = await lector.LeerListaAsync(MapeosInspeccion.Desviacion, ct);
        }

        return evaluacion;
    }
}
