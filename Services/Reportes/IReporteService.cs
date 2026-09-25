using calidad_app.Models.Reportes;

namespace calidad_app.Services.Reportes;

/// <summary>
/// El centro de reportes: consulta un reporte y lo entrega como archivo.
///
/// A diferencia del tablero, aquí los datos SALEN de la aplicación, así que el
/// permiso GENERAR_REPORTES se valida también en la base (los procedimientos
/// reciben @UsuarioId) y cada archivo generado queda en la bitácora con quién
/// lo pidió, de qué reporte, en qué formato y con qué filtros.
///
/// Los cuatro reportes se devuelven como <see cref="TablaReporte"/>, es decir
/// ya convertidos en columnas y filas. Se hace así porque el destino de un
/// reporte es una tabla: la vista previa de la pantalla, la hoja de Excel y la
/// tabla del PDF salen todas de la misma estructura, y agregar un quinto
/// reporte no obliga a escribir una tabla HTML ni un exportador nuevos.
/// </summary>
public interface IReporteService
{
    /// <summary>
    /// Ejecuta el reporte y lo devuelve como tabla, para la vista previa.
    /// </summary>
    /// <param name="ambito">
    /// Descripción en una línea de los filtros aplicados ("Línea EXT-1, turno
    /// A"), que la pantalla arma porque es la que tiene los catálogos
    /// cargados. Va al encabezado del archivo y a la bitácora, y es lo que
    /// permite reconstruir después qué se llevó exactamente.
    /// </param>
    Task<TablaReporte> ConsultarAsync(
        string reporte, FiltroReporte filtro, string? ambito = null,
        CancellationToken ct = default);

    /// <summary>
    /// Ejecuta el reporte, genera el archivo (Excel o PDF) y registra la
    /// descarga en la bitácora.
    /// </summary>
    Task<ArchivoGenerado> ExportarAsync(
        string reporte, FiltroReporte filtro, FormatoArchivo formato,
        string? ambito = null, CancellationToken ct = default);
}
