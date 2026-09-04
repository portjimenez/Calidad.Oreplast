using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// La firma con la que Ingeniería de Calidad libera el producto.
///
/// Es el punto de control del módulo: la base comprueba el permiso
/// LIBERAR_PRODUCTO, vuelve a ejecutar la validación de completitud del
/// registro y rechaza la firma si hay hallazgos bloqueantes. Nada de eso se
/// repite aquí; la pantalla solo refleja el resultado.
///
/// La liberación no se revierte. Si algo se firmó por error, el camino es
/// levantar una no conformidad: la firma es evidencia y borrarla dejaría el
/// historial mintiendo.
/// </summary>
public interface ILiberacionService
{
    /// <summary>
    /// Estado de las dos firmas del registro (despeje y cierre) y los datos de
    /// la sección Cierre de orden.
    /// </summary>
    Task<EstadoLiberacion?> ObtenerAsync(int registroId, CancellationToken ct = default);

    /// <summary>
    /// Firma una liberación. Las verificaciones de calidad e inocuidad son las
    /// casillas que el formato en papel ya exigía y la base las requiere en
    /// true.
    /// </summary>
    Task<ResultadoLiberacion> RegistrarAsync(
        int registroId,
        string tipo,
        int? loteId = null,
        bool calidadVerificada = true,
        bool inocuidadVerificada = true,
        CancellationToken ct = default);

    /// <summary>
    /// Guarda la sección Cierre de orden: comentarios, kilos de producto no
    /// conforme y su razón. Se captura antes de firmar el cierre.
    /// </summary>
    Task<EstadoLiberacion?> GuardarCierreAsync(
        int registroId,
        string? comentarios = null,
        decimal? kgProductoNoConforme = null,
        string? razonNoConforme = null,
        CancellationToken ct = default);
}
