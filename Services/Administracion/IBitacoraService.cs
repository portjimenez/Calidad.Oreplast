using calidad_app.Models.Administracion;

namespace calidad_app.Services.Administracion;

/// <summary>
/// Consulta de la bitácora de auditoría (aud.Bitacora). Es de solo lectura:
/// cada módulo escribe en ella desde sus propios procedimientos.
///
/// A diferencia de los demás listados del proyecto, aquí se pagina en la base:
/// se escribe una fila en cada carga de página y la tabla crece con el uso
/// diario, así que traerla completa para mostrar 50 filas no escala.
///
/// La base exige VER_BITACORA también en la lectura (no solo la página), porque
/// es información de auditoría; por eso los dos métodos resuelven al usuario.
/// </summary>
public interface IBitacoraService
{
    /// <summary>Una página del resultado, con el total para el paginador.</summary>
    Task<PaginaBitacora> ListarAsync(
        FiltroBitacora filtro, int pagina = 1, int tamanoPagina = 50,
        CancellationToken ct = default);

    /// <summary>Indicadores del rango consultado, sin los demás filtros.</summary>
    Task<ResumenBitacora> ResumenAsync(
        DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct = default);
}
