namespace calidad_app.Components.Layout;

/// <summary>
/// Lo que la pantalla abierta le ofrece a la barra de herramientas.
///
/// La barra vive en el layout y no conoce los datos de ninguna pantalla: no sabe
/// qué consulta repetir para actualizar, ni qué archivo generar al exportar. Por
/// eso la pantalla lo anuncia aquí (con el componente &lt;AccionesDePantalla&gt;) y
/// la barra solo invoca lo anunciado. Una sola barra para toda la aplicación, sin
/// repetirla en cada componente.
///
/// Es scoped: en Blazor Server equivale a un circuito, es decir, a una pestaña
/// del navegador de un usuario. Dos personas nunca comparten la misma instancia.
/// </summary>
public sealed class AccionesPantalla
{
    private Registro? _actual;

    /// <summary>Avisa a la barra que cambió lo que puede ofrecer.</summary>
    public event Action? Cambio;

    /// <summary>Recarga propia de la pantalla (conserva sus filtros); null si no la ofrece.</summary>
    public Func<Task>? Actualizar => _actual?.Actualizar;

    /// <summary>Exportación de la pantalla; null si no exporta.</summary>
    public Func<Task>? Exportar => _actual?.Exportar;

    /// <summary>La pantalla exporta pero en este momento no hay qué (sin filas, ocupada).</summary>
    public bool ExportarHabilitado => _actual is { Exportar: not null, ExportarHabilitado: true };

    public string? TextoExportar => _actual?.TextoExportar;

    /// <summary>
    /// Lo que anuncia una pantalla. Es un objeto propio de cada instancia de
    /// &lt;AccionesDePantalla&gt;, y eso es lo que permite retirarlo sin pisar al
    /// siguiente: al navegar, Blazor crea la pantalla nueva ANTES de desechar la
    /// anterior, así que si la anterior borrara "lo que haya" al desecharse se
    /// llevaría por delante las acciones que la nueva acaba de publicar.
    /// </summary>
    public sealed class Registro
    {
        public Func<Task>? Actualizar { get; set; }
        public Func<Task>? Exportar { get; set; }
        public bool ExportarHabilitado { get; set; }
        public string? TextoExportar { get; set; }
    }

    public void Publicar(Registro registro)
    {
        _actual = registro;
        Cambio?.Invoke();
    }

    public void Retirar(Registro registro)
    {
        if (ReferenceEquals(_actual, registro))
        {
            _actual = null;
            Cambio?.Invoke();
        }
    }
}
