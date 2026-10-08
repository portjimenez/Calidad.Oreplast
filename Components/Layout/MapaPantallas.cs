using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace calidad_app.Components.Layout;

/// <summary>
/// Las pantallas de la aplicación tal como se le ofrecen al usuario: grupo,
/// texto, destino y para qué sirven. Lo leen el menú lateral, la barra de
/// herramientas y la página de Ayuda, que así no pueden contradecirse (una
/// pantalla nueva aparece en las tres con una sola línea aquí).
///
/// El permiso de cada entrada NO se escribe aquí: se lee del atributo
/// [Authorize(Policy = "...")] de la propia pantalla destino, buscándola por su
/// ruta. Si mañana una pantalla cambia el permiso que exige, el menú la ofrece o
/// la esconde en consecuencia sin que nadie tenga que acordarse de venir a tocar
/// esta lista, que es justo la clase de olvido que deja un menú ofreciendo un 403.
///
/// Esto NO es el mecanismo de autorización: la que decide sigue siendo la
/// política de cada página (y el FallbackPolicy de Program.cs). Aquí solo se deja
/// de ofrecer lo que no se puede abrir.
/// </summary>
public static class MapaPantallas
{
    public sealed record Pantalla(string Texto, string Href, string Descripcion);
    public sealed record Grupo(string Titulo, IReadOnlyList<Pantalla> Pantallas);

    public static readonly IReadOnlyList<Grupo> Grupos =
    [
        new("Monitoreo", [
            new("Panel de calidad", "/",
                "Lo que está pendiente ahora: alertas abiertas, registros por liberar y no conformidades en curso."),
            new("Alertas", "/alertas",
                "Variaciones detectadas al guardar una medición fuera de la tolerancia de la ficha técnica. Aquí se atienden y se documenta la acción tomada.")
        ]),
        new("Producción", [
            new("Órdenes de producción", "/ordenes-produccion",
                "Las órdenes de producción con su expediente: registros de inspección, bobinas y lotes de cada una."),
            new("Inspección en proceso", "/inspeccion-en-proceso",
                "Registro de la inspección por orden y máquina: setup, producción por bobina con sus mediciones y despeje de línea. Cada medición se compara contra la ficha técnica vigente."),
            new("Asignación de operadores", "/asignacion-operadores",
                "Programación de qué operador trabaja en qué máquina y en qué turno."),
            new("Control de desperdicio", "/control-desperdicio",
                "Desperdicio real contra las metas, por línea, máquina y turno, con el detalle de los paros.")
        ]),
        new("Calidad", [
            new("No conformidades", "/no-conformidades",
                "Levantamiento, seguimiento y cierre de las no conformidades."),
            new("Fichas técnicas", "/fichas-tecnicas",
                "Parámetros y tolerancias de cada producto. Se versionan por fecha de vigencia: una versión ya usada no se edita, se reemplaza por otra."),
            new("Trazabilidad por lote", "/trazabilidad-lote",
                "De un lote a la orden, las bobinas y las mediciones que lo originaron. Desde aquí Ingeniería de Calidad libera el producto."),
            new("Certificados", "/certificados",
                "El certificado de calidad de cada lote, listo para imprimir y entregar al cliente.")
        ]),
        new("Indicadores", [
            new("Tablero de indicadores", "/indicadores",
                "Indicadores del periodo comparados contra el periodo anterior de igual longitud: tendencia, comparativo, foco de calidad y análisis de causas."),
            new("Reportes", "/reportes",
                "Reportes exportables a Excel y PDF. Cada descarga queda registrada en la bitácora con sus filtros.")
        ]),
        new("Administración", [
            new("Usuarios y roles", "/usuarios-roles",
                "Usuarios del dominio que pueden entrar al sistema y permisos de cada rol."),
            new("Catálogos", "/catalogos",
                "Máquinas, líneas, materiales, parámetros, turnos y metas de producción."),
            new("Bitácora", "/bitacora",
                "Registro de auditoría: quién hizo qué y cuándo, incluidos los accesos denegados.")
        ])
    ];

    /// <summary>
    /// Ruta de cada página con la política que exige, leída por reflexión de los
    /// atributos del componente. Se calcula una vez por proceso: recorrer los
    /// tipos del ensamblado es barato pero no gratis, y el menú se dibuja en
    /// cada navegación.
    /// </summary>
    private static readonly Dictionary<string, string> PoliticasPorRuta = ResolverPoliticas();

    /// <summary>Los grupos con solo las pantallas que el usuario puede abrir; un grupo vacío no se devuelve.</summary>
    public static List<Grupo> VisiblesPara(ClaimsPrincipal usuario) =>
        Grupos
            .Select(g => new Grupo(g.Titulo, g.Pantallas.Where(p => PuedeAbrir(usuario, p.Href)).ToList()))
            // Un grupo sin entradas visibles no se dibuja: su título solo
            // anunciaría un apartado vacío (al Administrador, por ejemplo, no le
            // corresponde ninguna pantalla de Producción).
            .Where(g => g.Pantallas.Count > 0)
            .ToList();

    /// <summary>
    /// Se compara contra el claim "permiso", que es la misma fuente que lee
    /// <c>PermisoAuthorizationHandler</c> al autorizar la página. Si las dos
    /// cosas se preguntaran de formas distintas podrían contradecirse.
    ///
    /// Una pantalla sin política (o cuya ruta no se encuentre) se muestra: aquí
    /// no se decide el acceso, y esconder una pantalla por un fallo de resolución
    /// dejaría al usuario sin forma de llegar a algo que sí puede usar. Si no le
    /// corresponde, la propia página lo rechaza.
    /// </summary>
    public static bool PuedeAbrir(ClaimsPrincipal usuario, string href) =>
        usuario.Identity?.IsAuthenticated == true
        && (PermisoDe(href) is not { } permiso || usuario.HasClaim("permiso", permiso));

    private static string? PermisoDe(string href) =>
        PoliticasPorRuta.TryGetValue(Normalizar(href), out var politica) ? politica : null;

    private static Dictionary<string, string> ResolverPoliticas()
    {
        var politicas = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var tipo in typeof(MapaPantallas).Assembly.GetTypes())
        {
            if (!typeof(ComponentBase).IsAssignableFrom(tipo))
            {
                continue;
            }

            var politica = tipo.GetCustomAttributes<AuthorizeAttribute>(inherit: true)
                .Select(a => a.Policy)
                .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p));

            if (politica is null)
            {
                continue;
            }

            foreach (var ruta in tipo.GetCustomAttributes<RouteAttribute>(inherit: true))
            {
                politicas[Normalizar(ruta.Template)] = politica;
            }
        }

        return politicas;
    }

    /// <summary>La barra final no distingue una ruta: "/alertas" y "/alertas/" son la misma.</summary>
    private static string Normalizar(string ruta) =>
        ruta.Length > 1 ? ruta.TrimEnd('/') : ruta;
}
