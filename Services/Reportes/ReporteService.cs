using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Reportes;
using calidad_app.Services.Seguridad;

namespace calidad_app.Services.Reportes;

public class ReporteService(
    EjecutorSp sp,
    IUsuarioActual usuarioActual,
    IContextoAuditoria auditoria,
    ExportadorArchivos exportador) : IReporteService
{
    public async Task<TablaReporte> ConsultarAsync(
        string reporte, FiltroReporte filtro, string? ambito = null,
        CancellationToken ct = default)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        return reporte switch
        {
            ClavesReporte.NoConformidades => Tabla(reporte, filtro, ambito,
                await NoConformidadesAsync(usuarioId, filtro, ct)),

            ClavesReporte.Alertas => Tabla(reporte, filtro, ambito,
                await AlertasAsync(usuarioId, filtro, ct)),

            ClavesReporte.Lotes => Tabla(reporte, filtro, ambito,
                await LotesAsync(usuarioId, filtro, ct)),

            _ => Tabla(ClavesReporte.Produccion, filtro, ambito,
                await ProduccionAsync(usuarioId, filtro, ct))
        };
    }

    public async Task<ArchivoGenerado> ExportarAsync(
        string reporte, FiltroReporte filtro, FormatoArchivo formato,
        string? ambito = null, CancellationToken ct = default)
    {
        var tabla = await ConsultarAsync(reporte, filtro, ambito, ct);
        var definicion = ClavesReporte.Definicion(reporte);

        var archivo = exportador.Generar(tabla, formato, NombreDeArchivo(definicion.Clave));

        await RegistrarDescargaAsync(definicion.Clave, formato, tabla, ct);

        return archivo;
    }

    /* ------------------------------------------------------------------
       Consultas
       ------------------------------------------------------------------ */

    private Task<List<FilaProduccion>> ProduccionAsync(
        int usuarioId, FiltroReporte filtro, CancellationToken ct) =>
        sp.ConsultarAsync(
            "prod.usp_Reporte_Produccion",
            cmd => Ambito(cmd, usuarioId, filtro)
                .Con("@AreaId", filtro.AreaId)
                .Con("@LineaId", filtro.LineaId)
                .Con("@MaquinaId", filtro.MaquinaId)
                .Con("@TurnoId", filtro.TurnoId)
                .Con("@ProductoId", filtro.ProductoId)
                .Con("@ClienteId", filtro.ClienteId)
                .Con("@OperadorId", filtro.OperadorId),
            (lector, token) => lector.LeerListaAsync(MapeosReportes.FilaProduccion, token),
            ct);

    private Task<List<FilaNoConformidad>> NoConformidadesAsync(
        int usuarioId, FiltroReporte filtro, CancellationToken ct) =>
        sp.ConsultarAsync(
            "cal.usp_Reporte_NoConformidades",
            cmd => Ambito(cmd, usuarioId, filtro)
                .Con("@AreaId", filtro.AreaId)
                .Con("@LineaId", filtro.LineaId)
                .Con("@MaquinaId", filtro.MaquinaId)
                .Con("@TurnoId", filtro.TurnoId)
                .Con("@ProductoId", filtro.ProductoId)
                .Con("@ClienteId", filtro.ClienteId)
                .Con("@SeveridadId", filtro.SeveridadId)
                .Con("@SoloAbiertas", filtro.SoloAbiertas),
            (lector, token) => lector.LeerListaAsync(MapeosReportes.FilaNoConformidad, token),
            ct);

    private Task<List<FilaAlerta>> AlertasAsync(
        int usuarioId, FiltroReporte filtro, CancellationToken ct) =>
        sp.ConsultarAsync(
            "cal.usp_Reporte_Alertas",
            cmd => Ambito(cmd, usuarioId, filtro)
                .Con("@AreaId", filtro.AreaId)
                .Con("@LineaId", filtro.LineaId)
                .Con("@MaquinaId", filtro.MaquinaId)
                .Con("@TurnoId", filtro.TurnoId)
                .Con("@ParametroId", filtro.ParametroId)
                .Con("@SoloPendientes", filtro.SoloPendientes)
                .Con("@SoloCriticas", filtro.SoloCriticas),
            (lector, token) => lector.LeerListaAsync(MapeosReportes.FilaAlerta, token),
            ct);

    private Task<List<FilaLote>> LotesAsync(
        int usuarioId, FiltroReporte filtro, CancellationToken ct) =>
        sp.ConsultarAsync(
            "cal.usp_Reporte_Lotes",
            cmd => Ambito(cmd, usuarioId, filtro)
                .Con("@ClienteId", filtro.ClienteId)
                .Con("@ProductoId", filtro.ProductoId)
                .Con("@Estado", filtro.EstadoLote)
                .Con("@SoloSinCertificado", filtro.SoloSinCertificado),
            (lector, token) => lector.LeerListaAsync(MapeosReportes.FilaLote, token),
            ct);

    /// <summary>
    /// Lo que comparten los cuatro reportes: quién lo pide, el periodo y el
    /// tope de filas. El @UsuarioId lo resuelve el servidor desde la sesión,
    /// nunca lo envía la pantalla: si viajara desde el navegador se podría
    /// falsear y tanto el permiso como la bitácora dejarían de valer.
    /// </summary>
    private static DbCommand Ambito(DbCommand cmd, int usuarioId, FiltroReporte filtro) => cmd
        .Con("@UsuarioId", usuarioId)
        .Con("@FechaDesde", filtro.FechaDesde)
        .Con("@FechaHasta", filtro.FechaHasta)
        .Con("@MaxFilas", filtro.MaxFilas);

    private async Task RegistrarDescargaAsync(
        string reporte, FormatoArchivo formato, TablaReporte tabla, CancellationToken ct)
    {
        var usuarioId = await usuarioActual.ObtenerIdAsync();

        await sp.EjecutarAsync(
            "aud.usp_Reporte_RegistrarDescarga",
            cmd => cmd
                .Con("@UsuarioId", usuarioId)
                .Con("@Reporte", reporte)
                .Con("@Formato", formato == FormatoArchivo.Excel ? "Excel" : "PDF")
                .Con("@Filtros", Recortar(tabla.Subtitulo, 600))
                .Con("@Filas", tabla.Filas.Count)
                .Con("@DireccionIp", auditoria.DireccionIp),
            ct);
    }

    /* ------------------------------------------------------------------
       Armado de la tabla
       ------------------------------------------------------------------ */

    private static TablaReporte Tabla<T>(
        string reporte, FiltroReporte filtro, string? ambito, List<T> filas)
    {
        var definicion = ClavesReporte.Definicion(reporte);

        var tabla = new TablaReporte
        {
            Titulo = definicion.Nombre,
            Subtitulo = Describir(filtro, ambito, filas.Count),
            Columnas = Columnas(reporte),
            Filas = filas.Select(f => Valores(reporte, f!)).ToList()
        };

        // El tope de filas no es un detalle técnico: quien exporta tiene que
        // saber que está viendo una parte y no el total.
        if (filas.Count >= filtro.MaxFilas)
        {
            tabla.Nota = $"El reporte se recortó a las primeras {filtro.MaxFilas:N0} filas. " +
                         "Acote el periodo o agregue un filtro para verlo completo.";
        }

        return tabla;
    }

    private static string Describir(FiltroReporte filtro, string? ambito, int filas)
    {
        var partes = new List<string> { $"Periodo del {filtro.DescribirPeriodo()}" };

        if (!string.IsNullOrWhiteSpace(ambito))
        {
            partes.Add(ambito);
        }

        partes.Add(filas == 1 ? "1 fila" : $"{filas:N0} filas");

        return string.Join(" · ", partes);
    }

    private static string NombreDeArchivo(string reporte) => reporte switch
    {
        ClavesReporte.NoConformidades => "no-conformidades",
        ClavesReporte.Alertas => "desviaciones-ficha-tecnica",
        ClavesReporte.Lotes => "lotes-y-certificados",
        _ => "produccion-e-inspeccion"
    };

    private static string Recortar(string texto, int maximo) =>
        texto.Length <= maximo ? texto : texto[..maximo];

    private static List<ColumnaReporte> Columnas(string reporte) => reporte switch
    {
        ClavesReporte.NoConformidades =>
        [
            new("Código", TipoColumna.Texto, 1.7),
            new("Fecha", TipoColumna.Fecha, 1.8),
            new("Severidad", TipoColumna.Texto, 1.6),
            new("Estado", TipoColumna.Texto, 1.9),
            new("Tipo de defecto", TipoColumna.Texto, 2.6),
            new("Área", TipoColumna.Texto, 1.8),
            new("Registro", TipoColumna.Texto, 2.0),
            new("OP", TipoColumna.Texto, 1.8),
            new("Producto", TipoColumna.Texto, 2.2),
            new("Máquina", TipoColumna.Texto, 1.5),
            new("Descripción", TipoColumna.Texto, 4.5),
            new("Causa raíz", TipoColumna.Texto, 3.2),
            new("Acción correctiva", TipoColumna.Texto, 3.2),
            new("Responsable", TipoColumna.Texto, 2.4),
            new("Días", TipoColumna.Decimal, 1.2),
            new("Bobinas", TipoColumna.Entero, 1.3),
            new("Kg", TipoColumna.Decimal, 1.5)
        ],

        ClavesReporte.Alertas =>
        [
            new("Detección", TipoColumna.FechaHora, 2.4),
            new("Parámetro", TipoColumna.Texto, 3.4),
            new("Unidad", TipoColumna.Texto, 1.2),
            new("Crítico", TipoColumna.Booleano, 1.2),
            new("Valor", TipoColumna.Decimal, 1.5),
            new("Límite inf.", TipoColumna.Decimal, 1.5),
            new("Límite sup.", TipoColumna.Decimal, 1.5),
            new("Desviación", TipoColumna.Decimal, 1.6),
            new("Registro", TipoColumna.Texto, 2.0),
            new("OP", TipoColumna.Texto, 1.8),
            new("Producto", TipoColumna.Texto, 2.6),
            new("Máquina", TipoColumna.Texto, 1.5),
            new("Turno", TipoColumna.Texto, 1.5),
            new("Atendida", TipoColumna.Booleano, 1.3),
            new("Horas", TipoColumna.Decimal, 1.3),
            new("Atendida por", TipoColumna.Texto, 2.6),
            new("NC", TipoColumna.Texto, 1.5)
        ],

        ClavesReporte.Lotes =>
        [
            new("Lote", TipoColumna.Texto, 2.2),
            new("Fecha", TipoColumna.Fecha, 1.8),
            new("Estado", TipoColumna.Texto, 1.6),
            new("OP", TipoColumna.Texto, 1.8),
            new("Cliente", TipoColumna.Texto, 3.0),
            new("Producto", TipoColumna.Texto, 3.2),
            new("Bobinas", TipoColumna.Entero, 1.3),
            new("Conformes", TipoColumna.Entero, 1.4),
            new("Kg", TipoColumna.Decimal, 1.6),
            new("Liberación", TipoColumna.FechaHora, 2.2),
            new("Liberado por", TipoColumna.Texto, 2.6),
            new("Certificado", TipoColumna.Texto, 1.9),
            new("Fecha cert.", TipoColumna.FechaHora, 2.2),
            new("NC", TipoColumna.Entero, 1.1)
        ],

        _ =>
        [
            new("Registro", TipoColumna.Texto, 2.1),
            new("Fecha", TipoColumna.Fecha, 1.8),
            new("Turno", TipoColumna.Texto, 1.5),
            new("Línea", TipoColumna.Texto, 1.4),
            new("Máquina", TipoColumna.Texto, 1.6),
            new("Operador", TipoColumna.Texto, 2.8),
            new("OP", TipoColumna.Texto, 1.8),
            new("Cliente", TipoColumna.Texto, 2.6),
            new("Producto", TipoColumna.Texto, 3.0),
            new("Bobinas", TipoColumna.Entero, 1.3),
            new("Kg producidos", TipoColumna.Decimal, 1.8),
            new("Kg desperdicio", TipoColumna.Decimal, 1.8),
            new("% desperdicio", TipoColumna.Porcentaje, 1.6),
            new("% ficha", TipoColumna.Porcentaje, 1.4),
            new("Alertas", TipoColumna.Entero, 1.2),
            new("NC", TipoColumna.Entero, 1.0),
            new("Paro (min)", TipoColumna.Entero, 1.4),
            new("Estado", TipoColumna.Texto, 1.6)
        ]
    };

    /// <summary>
    /// Convierte una fila tipada en el arreglo de valores que espera la tabla,
    /// en el mismo orden que <see cref="Columnas"/>. Los valores viajan con su
    /// tipo (decimal, fecha, bool) y no como texto, para que el Excel salga con
    /// números de verdad.
    /// </summary>
    private static object?[] Valores(string reporte, object fila) => (reporte, fila) switch
    {
        (ClavesReporte.NoConformidades, FilaNoConformidad n) =>
        [
            n.Codigo, n.FechaRegistro, n.Severidad, n.Estado, n.TipoDefecto, n.Area,
            n.IdRegistro, n.NumeroOP, n.ProductoCodigo, n.MaquinaCodigo,
            n.Descripcion, n.CausaRaiz, n.AccionCorrectiva, n.Responsable,
            n.DiasDeAtencion, n.BobinasVinculadas, n.KgVinculados
        ],

        (ClavesReporte.Alertas, FilaAlerta a) =>
        [
            a.FechaDeteccion, $"{a.ParametroCodigo} - {a.Parametro}", a.Unidad, a.EsCritico,
            a.ValorRegistrado, a.LimiteInferior, a.LimiteSuperior, a.Desviacion,
            a.IdRegistro, a.NumeroOP, a.ProductoCodigo, a.MaquinaCodigo, a.Turno,
            a.Atendida, a.HorasHastaAtencion, a.AtendidaPor, a.NoConformidad
        ],

        (ClavesReporte.Lotes, FilaLote l) =>
        [
            l.CodigoLote, l.Fecha, l.Estado, l.NumeroOP, l.Cliente, l.Producto,
            l.Bobinas, l.BobinasConformes, l.KgProducidos,
            l.FechaLiberacion, l.LiberadoPor, l.CodigoCertificado, l.FechaCertificado,
            l.NoConformidades
        ],

        (_, FilaProduccion p) =>
        [
            p.IdRegistro, p.Fecha, p.Turno, p.Linea, p.MaquinaCodigo, p.Operador,
            p.NumeroOP, p.Cliente, p.ProductoCodigo, p.Bobinas,
            p.KgProducidos, p.KgDesperdicio, p.PorcentajeDesperdicio, p.PorcentajeFichaTecnica,
            p.Alertas, p.NoConformidades, p.TiempoMuertoMin, p.Estado
        ],

        _ => []
    };
}
