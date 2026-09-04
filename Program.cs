using calidad_app.Components;
using calidad_app.Data;
using calidad_app.Data.Sp;
using calidad_app.Services.Calidad;
using calidad_app.Services.Inspeccion;
using calidad_app.Services.Seguridad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

// Fábrica en vez de un DbContext con ámbito: varios componentes pueden inicializarse en
// paralelo dentro del mismo circuito (ver MainLayout + SelectorUsuarioSimulado), y un solo
// DbContext no admite operaciones concurrentes.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OreplastCalidad")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccesoActualInfo, AccesoActualInfo>();
// Identidad y datos de auditoría de quien ejecuta cada acción: los servicios de
// inspección resuelven con ellos el @UsuarioId y la IP que exigen los procedimientos,
// para que la pantalla nunca tenga que enviarlos (ni pueda falsearlos).
builder.Services.AddScoped<IUsuarioActual, UsuarioActual>();
builder.Services.AddScoped<IContextoAuditoria, ContextoAuditoria>();

// Módulo 2 - Inspección en proceso. EjecutorSp baja a ADO.NET porque varios
// procedimientos devuelven más de un conjunto de resultados y EF Core solo lee el primero.
builder.Services.AddScoped<EjecutorSp>();
builder.Services.AddScoped<IRegistroInspeccionService, RegistroInspeccionService>();
builder.Services.AddScoped<IBobinaService, BobinaService>();
builder.Services.AddScoped<IParametroService, ParametroService>();
builder.Services.AddScoped<ICatalogoInspeccionService, CatalogoInspeccionService>();

// Módulo 3 - Calidad. Comparte EjecutorSp con el módulo 2: los procedimientos de
// alertas y del panel también devuelven varios conjuntos de resultados.
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IPanelCalidadService, PanelCalidadService>();
builder.Services.AddScoped<INoConformidadService, NoConformidadService>();
builder.Services.AddScoped<ILoteService, LoteService>();
builder.Services.AddScoped<ILiberacionService, LiberacionService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();

// Enriquece HttpContext.User contra seg.Usuario justo después de autenticar (Negotiate o
// "Simulacion"), para que AuthorizeRouteView/FallbackPolicy vean la decisión real ya en la
// primera carga de página, no solo dentro del árbol de componentes de Blazor.
builder.Services.AddTransient<IClaimsTransformation, SegUsuarioClaimsTransformation>();

if (builder.Environment.IsDevelopment())
{
    // Desarrollo: identidad de dominio simulada (selector de usuario/rol en la topbar), no
    // depende de Windows Authentication. El esquema "Simulacion" solo aporta el nombre de
    // cuenta (cookie simulacion_usuario); SegUsuarioClaimsTransformation hace la validación
    // real. Nada de esto debe registrarse en producción.
    builder.Services.AddAuthentication("Simulacion")
        .AddScheme<AuthenticationSchemeOptions, SimulacionAuthenticationHandler>("Simulacion", null);

    builder.Services.AddScoped<SimuladorIdentidadService>();
}
else
{
    // Producción: Windows Authentication (Negotiate) detrás de IIS, dominio OREPLAST.
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();
}

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization(options =>
{
    // Toda página requiere un usuario autenticado por defecto (seg.Usuario válido y activo,
    // resuelto en SegUsuarioClaimsTransformation); Components/Pages/Error.razor es la única
    // excepción explícita, con [AllowAnonymous]. Así no hay que repetir [Authorize] en cada
    // componente nuevo de los próximos módulos.
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermisoAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermisoAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Un 401/403 del FallbackPolicy llega vacío por defecto (ocurre antes de que Blazor renderice
// nada); reexpone ese código como página real vía EstadoHttp.razor ([AllowAnonymous]). Tiene
// que ir ANTES de autenticación/autorización explícitas: si se dejaran auto-insertadas por
// WebApplication (sin estas dos líneas), el framework las coloca justo después de enrutar,
// por delante de este middleware, y entonces nunca llega a ver el 403 para reescribirlo.
app.UseStatusCodePagesWithReExecute("/estado-http/{0}", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    // Cambia/borra la cookie del usuario simulado con una navegación real (igual que un login
    // real), para que la petición completa vuelva a pasar por autenticación/autorización.
    app.MapGet("/dev/simular", (string usuario, HttpContext ctx) =>
    {
        ctx.Response.Cookies.Append(SimulacionConstantes.CookieUsuario, usuario);
        var volver = ctx.Request.Headers.Referer.FirstOrDefault() ?? "/";
        return Results.Redirect(volver);
    }).AllowAnonymous();

    app.MapGet("/dev/salir", (HttpContext ctx) =>
    {
        ctx.Response.Cookies.Delete(SimulacionConstantes.CookieUsuario);
        return Results.Redirect("/");
    }).AllowAnonymous();
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
