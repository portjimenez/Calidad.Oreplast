using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Inspeccion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Inspeccion;

public class RegistroInspeccionService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IRegistroInspeccionService
{
    public Task<List<RegistroResumen>> ListarAsync(FiltroRegistros filtro, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_RegistroInspeccion_Listar",
            cmd => cmd
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@TurnoId", filtro.TurnoId)
                .Con("@MaquinaId", filtro.MaquinaId)
                .Con("@AreaId", filtro.AreaId)
                .Con("@OperadorId", filtro.OperadorId)
                .Con("@OrdenId", filtro.OrdenId)
                .Con("@Estado", filtro.Estado)
                .Con("@SoloAbiertos", filtro.SoloAbiertos)
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@MaxFilas", filtro.MaxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosInspeccion.RegistroResumen, token),
            ct);

    public Task<RegistroDetalle?> ObtenerAsync(int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_RegistroInspeccion_Obtener",
            cmd => cmd.Con("@RegistroId", registroId),
            async (lector, token) =>
            {
                var detalle = await lector.LeerUnoAsync(MapeosInspeccion.RegistroDetalle, token);
                if (detalle is null)
                {
                    return null;
                }

                // Segundo resultado: los lotes de las bobinas del registro.
                await lector.NextResultAsync(token);
                detalle.Lotes = await lector.LeerListaAsync(MapeosInspeccion.Lote, token);

                return detalle;
            },
            ct);

    public Task<List<OrdenBusqueda>> BuscarOrdenesAsync(
        string? busqueda, bool incluirCerradas = false, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_OrdenProduccion_Buscar",
            cmd => cmd
                .Con("@Busqueda", busqueda)
                .Con("@IncluirCerradas", incluirCerradas),
            (lector, token) => lector.LeerListaAsync(r => new OrdenBusqueda
            {
                OrdenId = r.Entero("OrdenId"),
                NumeroOP = r.Texto("NumeroOP"),
                Estado = r.Texto("Estado"),
                KgProgramados = r.DecimalNulo("KgProgramados"),
                ClienteNombre = r.Texto("ClienteNombre"),
                ProductoId = r.Entero("ProductoId"),
                ProductoCodigo = r.Texto("ProductoCodigo"),
                ProductoNombre = r.Texto("ProductoNombre"),
                RegistrosExistentes = r.Entero("RegistrosExistentes"),
                ProductoConFicha = r.Booleano("ProductoConFicha")
            }, token),
            ct);

    public async Task<(int RegistroId, string IdRegistro)> CrearAsync(
        NuevoRegistro nuevo, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_RegistroInspeccion_Crear",
            cmd => cmd
                .Con("@OrdenId", nuevo.OrdenId)
                .Con("@TipoRegistroId", nuevo.TipoRegistroId)
                .Con("@Fecha", nuevo.Fecha)
                .Con("@TurnoId", nuevo.TurnoId)
                .Con("@OperadorId", nuevo.OperadorId)
                .Con("@MaquinaId", nuevo.MaquinaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Clasificacion", nuevo.Clasificacion)
                .Con("@FechaHoraInicio", nuevo.FechaHoraInicio)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
            {
                if (!await lector.ReadAsync(token))
                {
                    throw new InvalidOperationException("El registro no se pudo crear.");
                }

                return (lector.Entero("RegistroId"), lector.Texto("IdRegistro"));
            },
            ct);
    }

    public async Task ActualizarEncabezadoAsync(
        int registroId,
        DateOnly? fecha = null,
        int? turnoId = null,
        int? operadorId = null,
        int? maquinaId = null,
        DateTime? fechaHoraInicio = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "prod.usp_RegistroInspeccion_ActualizarEncabezado",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Fecha", fecha)
                .Con("@TurnoId", turnoId)
                .Con("@OperadorId", operadorId)
                .Con("@MaquinaId", maquinaId)
                .Con("@FechaHoraInicio", fechaHoraInicio)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    public Task<EspecificacionProceso?> ObtenerEspecificacionAsync(
        int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_EspecificacionProceso_Obtener",
            cmd => cmd.Con("@RegistroId", registroId),
            (lector, token) => lector.LeerUnoAsync(MapeosInspeccion.Especificacion, token),
            ct);

    public async Task GuardarEspecificacionAsync(
        EspecificacionProceso e, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "prod.usp_EspecificacionProceso_Guardar",
            cmd => cmd
                .Con("@RegistroId", e.RegistroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@KgAProducir", e.KgAProducir)
                .Con("@AnchoProduccionMm", e.AnchoProduccionMm)
                .Con("@CalibreProduccionMic", e.CalibreProduccionMic)
                .Con("@Fuelle", e.Fuelle)
                .Con("@Color", e.Color)
                .Con("@Tratado", e.Tratado)
                .Con("@TipoBobina", e.TipoBobina)
                .Con("@TipoMaterial", e.TipoMaterial)
                .Con("@TipoSello", e.TipoSello)
                .Con("@Estructura", e.Estructura)
                .Con("@BobinasPorEmbobinador", e.BobinasPorEmbobinador)
                .Con("@Abierta", e.Abierta)
                .Con("@Impresa", e.Impresa)
                .Con("@UsoFinal", e.UsoFinal)
                .Con("@Rotulado", e.Rotulado)
                .Con("@MetrosAproxBobina", e.MetrosAproxBobina)
                .Con("@KgAproxBobina", e.KgAproxBobina)
                .Con("@AnchoProductoMm", e.AnchoProductoMm)
                .Con("@LargoProductoMm", e.LargoProductoMm)
                .Con("@CalibreProductoMic", e.CalibreProductoMic)
                .Con("@AnchoExtrusionMm", e.AnchoExtrusionMm)
                .Con("@CalibreExtrusionMic", e.CalibreExtrusionMic)
                .Con("@AlturaImpresion", e.AlturaImpresion)
                .Con("@Observaciones", e.Observaciones)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    public Task<MezclaMateriales> ObtenerMezclaAsync(int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_RegistroMaterial_Listar",
            cmd => cmd.Con("@RegistroId", registroId),
            LeerMezclaAsync,
            ct);

    public async Task<MezclaMateriales> GuardarMezclaAsync(
        int registroId,
        IEnumerable<MaterialEntrada> materiales,
        bool validarSuma = true,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();
        var json = JsonSerializer.Serialize(materiales);

        return await sp.ConsultarAsync(
            "prod.usp_RegistroMaterial_Guardar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@MaterialesJson", json)
                .Con("@ValidarSuma", validarSuma)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerMezclaAsync,
            ct);
    }

    public Task<SetupRegistro?> ObtenerSetupAsync(int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Setup_Obtener",
            cmd => cmd.Con("@RegistroId", registroId),
            (lector, token) => lector.LeerUnoAsync(MapeosInspeccion.Setup, token),
            ct);

    public async Task<SetupRegistro?> GuardarSetupAsync(SetupRegistro s, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Setup_Guardar",
            cmd => cmd
                .Con("@RegistroId", s.RegistroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@FechaSetup", s.FechaSetup)
                .Con("@OperadorSetupId", s.OperadorSetupId)
                .Con("@FechaHoraInicio", s.FechaHoraInicio)
                .Con("@FechaHoraFin", s.FechaHoraFin)
                // HorasSetup no se envía: la base lo deriva del intervalo.
                .Con("@TiempoMuertoMin", s.TiempoMuertoMin)
                .Con("@RazonId", s.RazonId)
                .Con("@KgDesperdicio", s.KgDesperdicio)
                .Con("@KgDuro", s.KgDuro)
                .Con("@DireccionIp", auditoria.DireccionIp),
            (lector, token) => lector.LeerUnoAsync(MapeosInspeccion.Setup, token),
            ct);
    }

    public Task<ProduccionRegistro?> ObtenerProduccionAsync(int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Produccion_Obtener",
            cmd => cmd.Con("@RegistroId", registroId),
            (lector, token) => lector.LeerUnoAsync(MapeosInspeccion.Produccion, token),
            ct);

    public async Task<ProduccionRegistro?> GuardarProduccionAsync(
        ProduccionRegistro p, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Produccion_Guardar",
            cmd => cmd
                .Con("@RegistroId", p.RegistroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@FechaProduccion", p.FechaProduccion)
                .Con("@OperadorId", p.OperadorId)
                .Con("@FechaHoraInicio", p.FechaHoraInicio)
                .Con("@FechaHoraFin", p.FechaHoraFin)
                .Con("@TiempoMuertoMin", p.TiempoMuertoMin)
                .Con("@RazonId", p.RazonId)
                .Con("@KgDesperdicio", p.KgDesperdicio)
                .Con("@KgRefill", p.KgRefill)
                .Con("@DireccionIp", auditoria.DireccionIp),
            (lector, token) => lector.LeerUnoAsync(MapeosInspeccion.Produccion, token),
            ct);
    }

    public Task<Checklist> ObtenerChecklistAsync(
        int registroId, string seccion = SeccionChecklist.DespejeLinea, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Checklist_ListarItems",
            cmd => cmd.Con("@RegistroId", registroId).Con("@Seccion", seccion),
            LeerChecklistAsync,
            ct);

    public async Task<Checklist> GuardarChecklistAsync(
        int registroId,
        IEnumerable<RespuestaChecklist> respuestas,
        string seccion = SeccionChecklist.DespejeLinea,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();
        var json = JsonSerializer.Serialize(respuestas);

        return await sp.ConsultarAsync(
            "prod.usp_RegistroChecklist_Guardar",
            cmd => cmd
                .Con("@RegistroId", registroId)
                .Con("@UsuarioId", usuarioId)
                .Con("@RespuestasJson", json)
                .Con("@Seccion", seccion)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerChecklistAsync,
            ct);
    }

    public Task<ValidacionCompletitud> ValidarCompletitudAsync(
        int registroId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_RegistroInspeccion_ValidarCompletitud",
            cmd => cmd.Con("@RegistroId", registroId),
            async (lector, token) =>
            {
                var validacion = await lector.LeerUnoAsync(MapeosInspeccion.Validacion, token)
                                 ?? new ValidacionCompletitud { RegistroId = registroId };

                // Segundo resultado: el detalle de los hallazgos.
                await lector.NextResultAsync(token);
                validacion.Hallazgos = await lector.LeerListaAsync(MapeosInspeccion.Hallazgo, token);

                return validacion;
            },
            ct);

    /* ---- Lecturas compartidas entre obtener y guardar ---- */

    private static async Task<MezclaMateriales> LeerMezclaAsync(
        System.Data.Common.DbDataReader lector, CancellationToken ct)
    {
        var mezcla = new MezclaMateriales
        {
            Materiales = await lector.LeerListaAsync(MapeosInspeccion.Material, ct)
        };

        // Segundo resultado: el total y si suma 100 %.
        if (await lector.NextResultAsync(ct) && await lector.ReadAsync(ct))
        {
            mezcla.PorcentajeTotal = lector.Decimal("PorcentajeTotal");
            mezcla.SumaCompleta = lector.Booleano("SumaCompleta");
            mezcla.TotalMateriales = lector.Entero("TotalMateriales");
        }

        return mezcla;
    }

    private static async Task<Checklist> LeerChecklistAsync(
        System.Data.Common.DbDataReader lector, CancellationToken ct)
    {
        var checklist = new Checklist
        {
            Items = await lector.LeerListaAsync(MapeosInspeccion.Item, ct)
        };

        // Segundo resultado: el avance de la sección.
        if (await lector.NextResultAsync(ct))
        {
            checklist.Avance = await lector.LeerUnoAsync(MapeosInspeccion.Avance, ct) ?? new AvanceChecklist();
        }

        return checklist;
    }
}
