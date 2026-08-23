using calidad_app.Models.Seguridad;

namespace calidad_app.Services.Seguridad;

/// <summary>
/// Expone el último resultado de validación de acceso (seg.usp_Usuario_ValidarAcceso)
/// para que la pantalla de Acceso No Autorizado pueda mostrar el motivo exacto.
/// </summary>
public interface IAccesoActualInfo
{
    AccesoResultado? UltimoResultado { get; }
}
