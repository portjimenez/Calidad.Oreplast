using calidad_app.Models.Seguridad;

namespace calidad_app.Services.Seguridad;

public class AccesoActualInfo(IHttpContextAccessor httpContextAccessor) : IAccesoActualInfo
{
    // Capturado en el momento de construcción (durante la petición HTTP que arma el circuito);
    // lo dejó SegUsuarioClaimsTransformation en HttpContext.Items.
    public AccesoResultado? UltimoResultado { get; } =
        httpContextAccessor.HttpContext?.Items["AccesoResultado"] as AccesoResultado;
}
