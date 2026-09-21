using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Mapeo de los procedimientos de fichas técnicas. Va aparte de
/// <see cref="MapeosCalidad"/> por la misma razón que <c>MapeosLotes</c>: es
/// un bloque con sus propios modelos, y juntarlo solo haría más largo un
/// archivo que ya lo es.
/// </summary>
internal static class MapeosFichas
{
    public static ProductoFicha ProductoFicha(DbDataReader r) => new()
    {
        ProductoId = r.Entero("ProductoId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        Estructura = r.TextoNulo("Estructura"),
        Activo = r.Booleano("Activo"),
        ClienteNombre = r.TextoNulo("ClienteNombre"),
        FichaVigenteId = r.EnteroNulo("FichaVigenteId"),
        VersionVigente = r.TextoNulo("VersionVigente"),
        VigenteDesde = r.SoloFechaNula("VigenteDesde"),
        BorradorId = r.EnteroNulo("BorradorId"),
        VersionBorrador = r.TextoNulo("VersionBorrador"),
        VersionProgramada = r.TextoNulo("VersionProgramada"),
        ProgramadaDesde = r.SoloFechaNula("ProgramadaDesde"),
        Versiones = r.Entero("Versiones"),
        OrdenesAbiertas = r.Entero("OrdenesAbiertas"),
        TotalFiltrado = r.Entero("TotalFiltrado")
    };

    public static FichaProducto FichaProducto(DbDataReader r) => new()
    {
        ProductoId = r.Entero("ProductoId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        Estructura = r.TextoNulo("Estructura"),
        Activo = r.Booleano("Activo"),
        ClienteNombre = r.TextoNulo("ClienteNombre"),
        OrdenesAbiertas = r.Entero("OrdenesAbiertas"),
        Registros = r.Entero("Registros"),
        UltimoRegistro = r.SoloFechaNula("UltimoRegistro"),
        FechaMinimaVigencia = r.SoloFecha("FechaMinimaVigencia"),
        FichaSeleccionadaId = r.EnteroNulo("FichaSeleccionadaId")
    };

    public static VersionFicha VersionFicha(DbDataReader r) => new()
    {
        FichaId = r.Entero("FichaId"),
        Version = r.Texto("Version"),
        VigenteDesde = r.SoloFecha("VigenteDesde"),
        VigenteHasta = r.SoloFechaNula("VigenteHasta"),
        Activa = r.Booleano("Activa"),
        Estado = r.Texto("Estado"),
        Tolerancias = r.Entero("Tolerancias"),
        Registros = r.Entero("Registros")
    };

    public static ToleranciaFicha ToleranciaFicha(DbDataReader r) => new()
    {
        ParametroId = r.Entero("ParametroId"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        Orden = r.Entero("Orden"),
        ParametroActivo = r.Booleano("ParametroActivo"),
        AreaNombre = r.TextoNulo("AreaNombre"),
        Aplica = r.Booleano("TieneTolerancia"),
        ValorObjetivo = r.DecimalNulo("ValorObjetivo"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior")
    };

    public static async Task<int> LeerIdAsync(DbDataReader lector, CancellationToken ct) =>
        await lector.ReadAsync(ct) ? lector.GetInt32(0) : 0;
}
