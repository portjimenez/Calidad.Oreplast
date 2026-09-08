using calidad_app.Models.Catalogos;

namespace calidad_app.Services.Catalogos;

/// <summary>
/// Catálogos que describen el proceso y no forman jerarquía entre sí:
/// materiales de mezcla, parámetros medibles, ítems de verificación, tipos de
/// defecto y razones de tiempo muerto.
///
/// Los cuatro últimos son los que los módulos de inspección y calidad ya
/// consumían sin tener dónde mantenerse: la ficha técnica se apoya en los
/// parámetros, el despeje de línea en los ítems de verificación, la no
/// conformidad en los tipos de defecto y el registro de paros en las razones.
///
/// Como en <see cref="ICatalogoService"/>, cada Guardar recibe el modelo
/// completo (id en su valor por defecto es alta) y el usuario y la IP los
/// resuelve el servicio.
/// </summary>
public interface ICatalogoProcesoService
{
    Task<List<MaterialCatalogo>> ListarMaterialesAsync(
        string? busqueda = null, bool soloActivos = false, CancellationToken ct = default);

    Task<int> GuardarMaterialAsync(MaterialCatalogo material, CancellationToken ct = default);

    Task CambiarEstadoMaterialAsync(int materialId, bool activo, CancellationToken ct = default);

    Task<List<ParametroCatalogo>> ListarParametrosAsync(
        string? busqueda = null, int? areaId = null, bool soloActivos = false,
        bool soloCriticos = false, CancellationToken ct = default);

    Task<int> GuardarParametroAsync(ParametroCatalogo parametro, CancellationToken ct = default);

    /// <summary>
    /// Retira el parámetro o lo reincorpora. La base rechaza la baja mientras
    /// alguna ficha técnica activa lo tenga entre sus tolerancias.
    /// </summary>
    Task CambiarEstadoParametroAsync(int parametroId, bool activo, CancellationToken ct = default);

    Task<List<ItemChecklistCatalogo>> ListarItemsChecklistAsync(
        string? tipo = null, int? areaId = null, string? busqueda = null,
        bool soloActivos = false, CancellationToken ct = default);

    Task<int> GuardarItemChecklistAsync(ItemChecklistCatalogo item, CancellationToken ct = default);

    Task CambiarEstadoItemChecklistAsync(int itemId, bool activo, CancellationToken ct = default);

    Task<List<TipoDefectoCatalogo>> ListarTiposDefectoAsync(
        string? busqueda = null, int? areaId = null, bool soloActivos = false,
        CancellationToken ct = default);

    Task<int> GuardarTipoDefectoAsync(TipoDefectoCatalogo tipo, CancellationToken ct = default);

    Task CambiarEstadoTipoDefectoAsync(int tipoDefectoId, bool activo, CancellationToken ct = default);

    Task<List<RazonTiempoMuertoCatalogo>> ListarRazonesTiempoMuertoAsync(
        string? busqueda = null, bool soloActivos = false, CancellationToken ct = default);

    Task<int> GuardarRazonTiempoMuertoAsync(
        RazonTiempoMuertoCatalogo razon, CancellationToken ct = default);

    Task CambiarEstadoRazonTiempoMuertoAsync(
        int razonId, bool activo, CancellationToken ct = default);
}
