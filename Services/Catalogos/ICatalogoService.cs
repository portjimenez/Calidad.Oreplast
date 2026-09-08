using calidad_app.Models.Catalogos;

namespace calidad_app.Services.Catalogos;

/// <summary>
/// Estructura de la planta: áreas, líneas, máquinas y turnos, más los dos
/// procedimientos transversales de la pantalla de catálogos (el resumen de las
/// pestañas y las listas de los combos).
///
/// Van juntos porque forman una jerarquía y sus reglas se cruzan: la línea
/// pertenece a un área, la máquina a un área y a una línea, y la baja de un
/// nivel depende de lo que le cuelgue. Los catálogos que no forman jerarquía
/// están en <see cref="ICatalogoProcesoService"/>, y las metas en
/// <see cref="IMetaProduccionService"/>, que tienen reglas propias.
///
/// Todos los guardados exigen el permiso GESTIONAR_CATALOGOS, que la base
/// vuelve a verificar. El identificador del usuario y la IP no se reciben
/// desde la pantalla: los resuelve el servicio, para que no puedan falsearse.
///
/// Cada Guardar recibe el propio modelo: con el id en su valor por defecto es
/// alta y con valor es modificación, igual que en el procedimiento. Devuelve
/// el id resultante.
/// </summary>
public interface ICatalogoService
{
    /// <summary>Conteos de los diez catálogos, para las pestañas.</summary>
    Task<List<ResumenCatalogo>> ResumenAsync(CancellationToken ct = default);

    /// <summary>
    /// Listas de los combos. Los productos solo se devuelven cuando se busca:
    /// son más de trescientos.
    /// </summary>
    Task<SelectoresCatalogo> SelectoresAsync(
        string? busquedaProducto = null, CancellationToken ct = default);

    Task<List<AreaCatalogo>> ListarAreasAsync(
        string? busqueda = null, bool soloActivos = false, CancellationToken ct = default);

    Task<int> GuardarAreaAsync(AreaCatalogo area, CancellationToken ct = default);

    /// <summary>
    /// Retira el área o la reincorpora. La base rechaza la baja si todavía
    /// tiene líneas o máquinas activas.
    /// </summary>
    Task CambiarEstadoAreaAsync(int areaId, bool activo, CancellationToken ct = default);

    Task<List<LineaCatalogo>> ListarLineasAsync(
        string? busqueda = null, int? areaId = null, bool soloActivos = false,
        CancellationToken ct = default);

    Task<int> GuardarLineaAsync(LineaCatalogo linea, CancellationToken ct = default);

    Task CambiarEstadoLineaAsync(int lineaId, bool activo, CancellationToken ct = default);

    Task<List<MaquinaCatalogo>> ListarMaquinasAsync(
        string? busqueda = null, int? areaId = null, int? lineaId = null,
        bool soloActivos = false, CancellationToken ct = default);

    Task<int> GuardarMaquinaAsync(MaquinaCatalogo maquina, CancellationToken ct = default);

    /// <summary>
    /// Retira la máquina o la reincorpora. La base rechaza la baja mientras
    /// tenga registros de inspección abiertos.
    /// </summary>
    Task CambiarEstadoMaquinaAsync(int maquinaId, bool activo, CancellationToken ct = default);

    Task<List<TurnoCatalogo>> ListarTurnosAsync(
        string? busqueda = null, CancellationToken ct = default);

    /// <summary>
    /// Alta y renombrado de un turno. No hay baja: <c>cat.Turno</c> es la
    /// única tabla de catálogo sin columna Activo.
    /// </summary>
    Task<int> GuardarTurnoAsync(TurnoCatalogo turno, CancellationToken ct = default);
}
