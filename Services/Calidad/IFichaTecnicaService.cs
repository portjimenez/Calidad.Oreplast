using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Fichas técnicas: la referencia contra la cual la inspección detecta las
/// variaciones.
///
/// Una ficha no se edita una vez usada: se versiona. La ficha que aplica a un
/// registro de inspección no se guarda en el registro, se recalcula por fecha
/// cada vez que se consulta o se certifica; cambiar una versión ya usada
/// cambiaría los límites que esos registros muestran. Por eso solo el
/// borrador es editable, y publicar exige una fecha de vigencia que no tome
/// registros ya capturados.
///
/// Toda la escritura exige GESTIONAR_FICHA (Ingeniería de Calidad y Jefe de
/// Producción), que la base comprueba en cada procedimiento.
/// </summary>
public interface IFichaTecnicaService
{
    /// <summary>
    /// Productos con el estado de su ficha. Los que no tienen ficha y sí
    /// órdenes abiertas vienen primero.
    /// </summary>
    Task<List<ProductoFicha>> ListarAsync(
        FiltroFichas filtro, int maxFilas = 200, CancellationToken ct = default);

    /// <summary>
    /// El producto, sus versiones y las tolerancias de <paramref name="fichaId"/>
    /// (en null: el borrador si lo hay, si no la vigente). Null si el producto
    /// no existe.
    /// </summary>
    Task<FichaProducto?> ObtenerAsync(
        int productoId, int? fichaId = null, CancellationToken ct = default);

    /// <summary>
    /// Abre un borrador, opcionalmente como copia de otra versión (del mismo
    /// producto o de otro parecido). Devuelve su id.
    /// </summary>
    Task<int> CrearVersionAsync(
        int productoId, int? copiarDeFichaId = null, CancellationToken ct = default);

    /// <summary>Guarda la fecha de vigencia y la tabla COMPLETA de tolerancias del borrador.</summary>
    Task GuardarBorradorAsync(
        int fichaId, DateOnly vigenteDesde, IEnumerable<ToleranciaEdicion> tolerancias,
        CancellationToken ct = default);

    Task PublicarAsync(int fichaId, CancellationToken ct = default);

    /// <summary>Devuelve a borrador una versión publicada que ningún registro usa.</summary>
    Task RetirarPublicacionAsync(int fichaId, CancellationToken ct = default);

    Task EliminarBorradorAsync(int fichaId, CancellationToken ct = default);
}
