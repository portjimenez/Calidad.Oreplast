using calidad_app.Data.Sp;
using calidad_app.Models.Produccion;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Produccion;

public class AsignacionOperadorService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : IAsignacionOperadorService
{
    public Task<List<CeldaAsignacion>> ListarRejillaAsync(
        DateOnly fecha, int? areaId = null, int? lineaId = null,
        int? maquinaId = null, int? turnoId = null, CancellationToken ct = default) =>
        ListarAsync(fecha, null, areaId, lineaId, maquinaId, turnoId, null,
                    incluirSinAsignar: true, ct);

    public Task<List<CeldaAsignacion>> ListarHistorialAsync(
        DateOnly fechaDesde, DateOnly fechaHasta, int? areaId = null, int? lineaId = null,
        int? maquinaId = null, int? turnoId = null, int? operadorId = null,
        CancellationToken ct = default) =>
        ListarAsync(fechaDesde, fechaHasta, areaId, lineaId, maquinaId, turnoId, operadorId,
                    incluirSinAsignar: false, ct);

    /// <summary>
    /// Los dos modos comparten procedimiento y solo se distinguen por
    /// <c>@IncluirSinAsignar</c>. Se exponen como dos métodos porque desde la
    /// pantalla son dos acciones distintas (programar y consultar) y porque en
    /// modo rejilla el rango no aplica: sería un producto cartesiano de
    /// máquinas por turnos por días.
    /// </summary>
    private Task<List<CeldaAsignacion>> ListarAsync(
        DateOnly fechaDesde, DateOnly? fechaHasta, int? areaId, int? lineaId,
        int? maquinaId, int? turnoId, int? operadorId, bool incluirSinAsignar,
        CancellationToken ct) =>
        sp.ConsultarAsync(
            "prod.usp_Asignacion_Listar",
            cmd => cmd
                .Con("@FechaDesde", fechaDesde)
                .Con("@FechaHasta", fechaHasta)
                .Con("@AreaId", areaId)
                .Con("@LineaId", lineaId)
                .Con("@MaquinaId", maquinaId)
                .Con("@TurnoId", turnoId)
                .Con("@OperadorId", operadorId)
                .Con("@IncluirSinAsignar", incluirSinAsignar),
            (lector, token) => lector.LeerListaAsync(MapeosProduccion.CeldaAsignacion, token),
            ct);

    public Task<List<OperadorDisponible>> ListarOperadoresAsync(
        DateOnly fecha, int turnoId, int? areaId = null, string? busqueda = null,
        CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "prod.usp_Asignacion_OperadoresDisponibles",
            cmd => cmd
                .Con("@Fecha", fecha)
                .Con("@TurnoId", turnoId)
                .Con("@AreaId", areaId)
                .Con("@Busqueda", busqueda),
            (lector, token) => lector.LeerListaAsync(MapeosProduccion.OperadorDisponible, token),
            ct);

    public async Task<int> GuardarAsync(
        AsignacionEdicion asignacion, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Asignacion_Guardar",
            cmd => cmd
                .Con("@OperadorId", asignacion.OperadorId)
                .Con("@MaquinaId", asignacion.MaquinaId)
                .Con("@TurnoId", asignacion.TurnoId)
                .Con("@Fecha", asignacion.Fecha)
                .Con("@UsuarioId", usuarioId)
                .Con("@AsignacionId", asignacion.AsignacionId == 0 ? null : asignacion.AsignacionId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            MapeosProduccion.LeerIdAsync,
            ct);
    }

    public async Task EliminarAsync(int asignacionId, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "prod.usp_Asignacion_Eliminar",
            cmd => cmd
                .Con("@AsignacionId", asignacionId)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    public async Task<ResultadoCopiaAsignaciones> CopiarAsync(
        DateOnly fechaOrigen, DateOnly fechaDestino, int? areaId = null,
        int? lineaId = null, bool sobrescribir = false, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "prod.usp_Asignacion_Copiar",
            cmd => cmd
                .Con("@FechaOrigen", fechaOrigen)
                .Con("@FechaDestino", fechaDestino)
                .Con("@UsuarioId", usuarioId)
                .Con("@AreaId", areaId)
                .Con("@LineaId", lineaId)
                .Con("@Sobrescribir", sobrescribir)
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosProduccion.ResultadoCopia, token) ?? new(),
            ct);
    }
}
