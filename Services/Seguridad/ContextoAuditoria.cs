namespace calidad_app.Services.Seguridad;

/// <summary>
/// Datos de la petición que se guardan en aud.Bitacora junto a cada escritura.
///
/// Se capturan en el constructor, es decir durante la petición HTTP que arma el
/// circuito de Blazor: una vez que el circuito vive sobre SignalR ya no hay
/// HttpContext del que leerlos. Es el mismo criterio que usa
/// <see cref="AccesoActualInfo"/>.
/// </summary>
public interface IContextoAuditoria
{
    string? DireccionIp { get; }
}

public class ContextoAuditoria(IHttpContextAccessor httpContextAccessor) : IContextoAuditoria
{
    public string? DireccionIp { get; } =
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
