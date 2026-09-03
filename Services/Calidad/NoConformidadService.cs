using System.Data.Common;
using System.Text.Json;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Calidad;

public class NoConformidadService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria) : INoConformidadService
{
    public Task<List<NoConformidadResumen>> ListarAsync(
        FiltroNoConformidades filtro, int maxFilas = 200, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_NoConformidad_Listar",
            cmd => cmd
                .Con("@FechaDesde", filtro.FechaDesde)
                .Con("@FechaHasta", filtro.FechaHasta)
                .Con("@AreaId", filtro.AreaId)
                .Con("@SeveridadId", filtro.SeveridadId)
                .Con("@EstadoId", filtro.EstadoId)
                .Con("@TipoDefectoId", filtro.TipoDefectoId)
                .Con("@ResponsableId", filtro.ResponsableId)
                .Con("@RegistroId", filtro.RegistroId)
                .Con("@OrdenId", filtro.OrdenId)
                .Con("@SoloAbiertas", filtro.SoloAbiertas)
                .Con("@Busqueda", filtro.Busqueda)
                .Con("@MaxFilas", maxFilas),
            (lector, token) => lector.LeerListaAsync(MapeosCalidad.NoConformidadResumen, token),
            ct);

    public Task<NoConformidadDetalle?> ObtenerAsync(
        int noConformidadId, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_NoConformidad_Obtener",
            cmd => cmd.Con("@NoConformidadId", noConformidadId),
            LeerExpedienteAsync,
            ct);

    public Task<CatalogosNoConformidad> ObtenerCatalogosAsync(
        int? areaId = null, CancellationToken ct = default) =>
        sp.ConsultarAsync(
            "cal.usp_NoConformidad_Catalogos",
            cmd => cmd.Con("@AreaId", areaId),
            async (lector, token) =>
            {
                var catalogos = new CatalogosNoConformidad
                {
                    Severidades = await lector.LeerListaAsync(MapeosCalidad.Severidad, token)
                };

                if (await lector.NextResultAsync(token))
                {
                    catalogos.Estados = await lector.LeerListaAsync(MapeosCalidad.EstadoNc, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    catalogos.TiposDefecto =
                        await lector.LeerListaAsync(MapeosCalidad.TipoDefecto, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    catalogos.Areas = await lector.LeerListaAsync(MapeosCalidad.Area, token);
                }

                if (await lector.NextResultAsync(token))
                {
                    catalogos.Responsables =
                        await lector.LeerListaAsync(MapeosCalidad.Responsable, token);
                }

                return catalogos;
            },
            ct);

    public async Task<NoConformidadCreada> CrearAsync(
        NuevaNoConformidad nueva, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_NoConformidad_Crear",
            cmd => cmd
                .Con("@TipoDefectoId", nueva.TipoDefectoId)
                .Con("@SeveridadId", nueva.SeveridadId)
                .Con("@Descripcion", nueva.Descripcion)
                .Con("@UsuarioId", usuarioId)
                .Con("@AreaId", nueva.AreaId)
                .Con("@RegistroId", nueva.RegistroId)
                .Con("@OrdenId", nueva.OrdenId)
                .Con("@ResponsableId", nueva.ResponsableId)
                .Con("@CausaRaiz", nueva.CausaRaiz)
                .Con("@AccionCorrectiva", nueva.AccionCorrectiva)
                .Con("@AlertasJson", JsonDe(nueva.AlertaIds, "AlertaId"))
                .Con("@BobinasJson", JsonDe(nueva.BobinaIds, "BobinaId"))
                .Con("@DireccionIp", auditoria.DireccionIp),
            async (lector, token) =>
                await lector.LeerUnoAsync(MapeosCalidad.NoConformidadCreada, token) ?? new(),
            ct);
    }

    public async Task<NoConformidadDetalle?> ActualizarAsync(
        int noConformidadId,
        string? descripcion = null,
        string? causaRaiz = null,
        string? accionCorrectiva = null,
        int? responsableId = null,
        int? severidadId = null,
        int? tipoDefectoId = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_NoConformidad_Actualizar",
            cmd => cmd
                .Con("@NoConformidadId", noConformidadId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Descripcion", descripcion)
                // Una cadena vacía significa "vaciar el campo", así que estos dos
                // no pasan por la normalización que convierte "" en DBNull.
                .ConTextoVaciable("@CausaRaiz", causaRaiz)
                .ConTextoVaciable("@AccionCorrectiva", accionCorrectiva)
                .Con("@ResponsableId", responsableId)
                .Con("@SeveridadId", severidadId)
                .Con("@TipoDefectoId", tipoDefectoId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerExpedienteAsync,
            ct);
    }

    public async Task<NoConformidadDetalle?> CambiarEstadoAsync(
        int noConformidadId,
        int estadoId,
        string? observacion = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_NoConformidad_CambiarEstado",
            cmd => cmd
                .Con("@NoConformidadId", noConformidadId)
                .Con("@EstadoId", estadoId)
                .Con("@UsuarioId", usuarioId)
                .Con("@Observacion", observacion)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerExpedienteAsync,
            ct);
    }

    public async Task<NoConformidadDetalle?> VincularBobinasAsync(
        int noConformidadId,
        IReadOnlyCollection<int> bobinaIds,
        bool vincular = true,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_NoConformidad_VincularBobinas",
            cmd => cmd
                .Con("@NoConformidadId", noConformidadId)
                .Con("@BobinasJson", JsonDe(bobinaIds, "BobinaId"))
                .Con("@UsuarioId", usuarioId)
                .Con("@Vincular", vincular)
                .Con("@DireccionIp", auditoria.DireccionIp),
            LeerExpedienteAsync,
            ct);
    }

    public async Task<EvidenciaNc?> GuardarEvidenciaAsync(
        int noConformidadId,
        string nombreArchivo,
        byte[] contenido,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_NoConformidadEvidencia_Guardar",
            cmd => cmd
                .Con("@NoConformidadId", noConformidadId)
                .Con("@NombreArchivo", nombreArchivo)
                .Con("@UsuarioId", usuarioId)
                .Con("@Archivo", contenido)
                .Con("@DireccionIp", auditoria.DireccionIp),
            (lector, token) => lector.LeerUnoAsync(MapeosCalidad.Evidencia, token),
            ct);
    }

    public async Task<ArchivoEvidencia?> ObtenerArchivoEvidenciaAsync(
        int evidenciaId, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return await sp.ConsultarAsync(
            "cal.usp_NoConformidadEvidencia_ObtenerArchivo",
            cmd => cmd
                .Con("@EvidenciaId", evidenciaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            (lector, token) => lector.LeerUnoAsync(MapeosCalidad.ArchivoEvidencia, token),
            ct);
    }

    public async Task EliminarEvidenciaAsync(int evidenciaId, CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "cal.usp_NoConformidadEvidencia_Eliminar",
            cmd => cmd
                .Con("@EvidenciaId", evidenciaId)
                .Con("@UsuarioId", usuarioId)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /// <summary>
    /// Los seis conjuntos de resultados del expediente. Lo comparten obtener,
    /// actualizar, cambiar estado y vincular bobinas: los cuatro terminan
    /// devolviendo la NC completa, para que la pantalla no tenga que pedirla de
    /// nuevo después de cada cambio.
    /// </summary>
    private static async Task<NoConformidadDetalle?> LeerExpedienteAsync(
        DbDataReader lector, CancellationToken ct)
    {
        var nc = await lector.LeerUnoAsync(MapeosCalidad.NoConformidadDetalle, ct);

        if (nc is null)
        {
            return null;
        }

        if (await lector.NextResultAsync(ct))
        {
            nc.Historial = await lector.LeerListaAsync(MapeosCalidad.MovimientoEstado, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            nc.Evidencias = await lector.LeerListaAsync(MapeosCalidad.Evidencia, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            nc.Bobinas = await lector.LeerListaAsync(MapeosCalidad.BobinaAfectada, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            nc.Alertas = await lector.LeerListaAsync(MapeosCalidad.AlertaDeNoConformidad, ct);
        }

        if (await lector.NextResultAsync(ct))
        {
            nc.EstadosDisponibles = await lector.LeerListaAsync(MapeosCalidad.EstadoDisponible, ct);
        }

        return nc;
    }

    /// <summary>
    /// Lista de ids al JSON que esperan los procedimientos. Una lista vacía se
    /// envía como null, que es como se dice "no toques nada".
    /// </summary>
    private static string? JsonDe(IReadOnlyCollection<int> ids, string propiedad) =>
        ids.Count == 0
            ? null
            : JsonSerializer.Serialize(ids.Select(id =>
                new Dictionary<string, int> { [propiedad] = id }));
}
