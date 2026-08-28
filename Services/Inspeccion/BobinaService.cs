using System.Data.Common;
using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Inspeccion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Inspeccion;

public class BobinaService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IBobinaService
{
    public Task<ProduccionPorBobina> ListarAsync(int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Bobina_Listar",
            cmd => cmd.Con("@RegistroId", registroId),
            LeerProduccionAsync,
            ct);

    public async Task<(int BobinaId, int IdBobi)> GuardarAsync(
        int registroId, BobinaEntrada bobina, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Bobina_Guardar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@BobinaId", bobina.BobinaId)
                .Con("@IdBobi", bobina.IdBobi)
                .Con("@Peso", bobina.Peso)
                .Con("@Metros", bobina.Metros)
                .Con("@Ancho", bobina.Ancho)
                .Con("@Fuelle", bobina.Fuelle)
                .Con("@Calibre", bobina.Calibre)
                .Con("@LoteId", bobina.LoteId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
            {
                if (!await lector.ReadAsync(token))
                {
                    throw new InvalidOperationException("La bobina no se pudo guardar.");
                }

                return (lector.Entero("BobinaId"), lector.Entero("IdBobi"));
            },
            ct);
    }

    public async Task<ConfirmacionBobina> ConfirmarAsync(int bobinaId, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Bobina_Confirmar",
            cmd => cmd
                .Con("@BobinaId", bobinaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Confirmar", true)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
            {
                var confirmacion = await lector.LeerUnoAsync(Confirmacion, token)
                                   ?? throw new InvalidOperationException("La bobina no se pudo confirmar.");

                // Segundo resultado: las desviaciones detectadas al confirmar.
                await lector.NextResultAsync(token);
                confirmacion.Desviaciones =
                    await lector.LeerListaAsync(MapeosInspeccion.DesviacionBobina, token);

                return confirmacion;
            },
            ct);
    }

    public async Task<ProduccionPorBobina> RetirarConfirmacionAsync(
        int bobinaId, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        // Con @Confirmar = 0 el procedimiento devuelve el listado actualizado.
        return await sp.ConsultarAsync(
            "prod.usp_Bobina_Confirmar",
            cmd => cmd
                .Con("@BobinaId", bobinaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Confirmar", false)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerProduccionAsync,
            ct);
    }

    public async Task<ProduccionPorBobina> EliminarAsync(int bobinaId, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Bobina_Eliminar",
            cmd => cmd
                .Con("@BobinaId", bobinaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerProduccionAsync,
            ct);
    }

    public Task<List<ItemChecklist>> ObtenerChecklistAsync(int bobinaId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_BobinaChecklist_Listar",
            cmd => cmd.Con("@BobinaId", bobinaId),
            (lector, token) => lector.LeerListaAsync(MapeosInspeccion.ItemBobina, token),
            ct);

    public async Task<List<ItemChecklist>> GuardarChecklistAsync(
        int bobinaId, IEnumerable<RespuestaChecklist> respuestas, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();
        var json = JsonSerializer.Serialize(respuestas);

        return await sp.ConsultarAsync(
            "prod.usp_BobinaChecklist_Guardar",
            cmd => cmd
                .Con("@BobinaId", bobinaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@RespuestasJson", json)
                .Con("@DireccionIp", auditoria.DireccionIp),
            (lector, token) => lector.LeerListaAsync(MapeosInspeccion.ItemBobina, token),
            ct);
    }

    private static async Task<ProduccionPorBobina> LeerProduccionAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var produccion = new ProduccionPorBobina
        {
            Bobinas = await lector.LeerListaAsync(MapeosInspeccion.Bobina, ct)
        };

        // Segundo resultado: los acumulados por medida.
        if (await lector.NextResultAsync(ct))
        {
            produccion.Acumulados = await lector.LeerListaAsync(MapeosInspeccion.Acumulado, ct);
        }

        return produccion;
    }

    private static ConfirmacionBobina Confirmacion(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        IdBobi = r.Entero("IdBobi"),
        Confirmada = r.Booleano("Confirmada"),
        Ok = r.Booleano("Ok"),
        ParametrosFueraDeRango = r.Entero("ParametrosFueraDeRango"),
        ItemsChecklistPendientes = r.Entero("ItemsChecklistPendientes"),
        AlertasGeneradas = r.Entero("AlertasGeneradas")
    };
}
