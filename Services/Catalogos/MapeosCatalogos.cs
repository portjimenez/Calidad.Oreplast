using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Catalogos;

namespace calidad_app.Services.Catalogos;

/// <summary>
/// Traduce cada fila devuelta por los procedimientos del módulo de catálogos a
/// su modelo.
///
/// Igual que en los módulos anteriores, el mapeo es explícito columna por
/// columna y no por reflexión: si un procedimiento cambia el nombre de una
/// columna, el error aparece aquí, en un solo lugar y con nombre propio, en
/// lugar de convertirse en una propiedad que queda en su valor por defecto sin
/// que nadie se entere.
/// </summary>
internal static class MapeosCatalogos
{
    public static ResumenCatalogo Resumen(DbDataReader r) => new()
    {
        Orden = r.Entero("Orden"),
        Clave = r.Texto("Clave"),
        Nombre = r.Texto("Nombre"),
        Total = r.Entero("Total"),
        Activos = r.Entero("Activos"),
        Inactivos = r.Entero("Inactivos"),
        AdmiteBaja = r.Booleano("AdmiteBaja")
    };

    public static AreaCatalogo Area(DbDataReader r) => new()
    {
        AreaId = r.Entero("AreaId"),
        Nombre = r.Texto("Nombre"),
        Activo = r.Booleano("Activo"),
        Lineas = r.Entero("Lineas"),
        LineasActivas = r.Entero("LineasActivas"),
        Maquinas = r.Entero("Maquinas"),
        MaquinasActivas = r.Entero("MaquinasActivas"),
        Parametros = r.Entero("Parametros"),
        ItemsChecklist = r.Entero("ItemsChecklist"),
        TiposDefecto = r.Entero("TiposDefecto"),
        TiposRegistro = r.Entero("TiposRegistro"),
        Usuarios = r.Entero("Usuarios"),
        NoConformidades = r.Entero("NoConformidades"),
        EnUso = r.Booleano("EnUso")
    };

    public static LineaCatalogo Linea(DbDataReader r) => new()
    {
        LineaId = r.Entero("LineaId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        AreaActiva = r.Booleano("AreaActiva"),
        Activo = r.Booleano("Activo"),
        Maquinas = r.Entero("Maquinas"),
        MaquinasActivas = r.Entero("MaquinasActivas"),
        Metas = r.Entero("Metas"),
        EnUso = r.Booleano("EnUso")
    };

    public static MaquinaCatalogo Maquina(DbDataReader r) => new()
    {
        MaquinaId = r.Entero("MaquinaId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        AreaActiva = r.Booleano("AreaActiva"),
        LineaId = r.EnteroNulo("LineaId"),
        LineaCodigo = r.TextoNulo("LineaCodigo"),
        LineaNombre = r.TextoNulo("LineaNombre"),
        Activo = r.Booleano("Activo"),
        Registros = r.Entero("Registros"),
        RegistrosAbiertos = r.Entero("RegistrosAbiertos"),
        UltimoRegistro = r.SoloFechaNula("UltimoRegistro"),
        Metas = r.Entero("Metas"),
        EnUso = r.Booleano("EnUso")
    };

    public static TurnoCatalogo Turno(DbDataReader r) => new()
    {
        TurnoId = r.Entero("TurnoId"),
        Nombre = r.Texto("Nombre"),
        Registros = r.Entero("Registros"),
        UltimoRegistro = r.SoloFechaNula("UltimoRegistro"),
        EnUso = r.Booleano("EnUso")
    };

    public static MaterialCatalogo Material(DbDataReader r) => new()
    {
        MaterialId = r.Entero("MaterialId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        Activo = r.Booleano("Activo"),
        Usos = r.Entero("Usos"),
        UltimoUso = r.SoloFechaNula("UltimoUso"),
        EnUso = r.Booleano("EnUso")
    };

    public static ParametroCatalogo Parametro(DbDataReader r) => new()
    {
        ParametroId = r.Entero("ParametroId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        Unidad = r.TextoNulo("Unidad"),
        AreaId = r.EnteroNulo("AreaId"),
        AreaNombre = r.TextoNulo("AreaNombre"),
        EsCritico = r.Booleano("EsCritico"),
        Orden = r.Entero("Orden"),
        Activo = r.Booleano("Activo"),
        Tolerancias = r.Entero("Tolerancias"),
        FichasActivas = r.Entero("FichasActivas"),
        Mediciones = r.Entero("Mediciones"),
        Alertas = r.Entero("Alertas"),
        EnUso = r.Booleano("EnUso")
    };

    public static MetaProduccion Meta(DbDataReader r) => new()
    {
        MetaId = r.Entero("MetaId"),
        Concepto = r.Texto("Concepto"),
        ValorMeta = r.Decimal("ValorMeta"),
        Unidad = r.TextoNulo("Unidad"),
        VigenteDesde = r.SoloFecha("VigenteDesde"),
        Activo = r.Booleano("Activo"),
        LineaId = r.EnteroNulo("LineaId"),
        LineaCodigo = r.TextoNulo("LineaCodigo"),
        LineaNombre = r.TextoNulo("LineaNombre"),
        MaquinaId = r.EnteroNulo("MaquinaId"),
        MaquinaCodigo = r.TextoNulo("MaquinaCodigo"),
        MaquinaNombre = r.TextoNulo("MaquinaNombre"),
        ProductoId = r.EnteroNulo("ProductoId"),
        ProductoCodigo = r.TextoNulo("ProductoCodigo"),
        ProductoNombre = r.TextoNulo("ProductoNombre"),
        Especificidad = r.Entero("Especificidad"),
        Vigente = r.Booleano("Vigente")
    };

    public static ItemChecklistCatalogo ItemChecklist(DbDataReader r) => new()
    {
        ItemId = r.Entero("ItemId"),
        Codigo = r.Texto("Codigo"),
        Texto = r.Texto("Texto"),
        Tipo = r.Texto("Tipo"),
        AreaId = r.EnteroNulo("AreaId"),
        AreaNombre = r.TextoNulo("AreaNombre"),
        Orden = r.Entero("Orden"),
        Activo = r.Booleano("Activo"),
        Respuestas = r.Entero("Respuestas"),
        EnUso = r.Booleano("EnUso")
    };

    public static TipoDefectoCatalogo TipoDefecto(DbDataReader r) => new()
    {
        TipoDefectoId = r.Entero("TipoDefectoId"),
        Nombre = r.Texto("Nombre"),
        AreaId = r.EnteroNulo("AreaId"),
        AreaNombre = r.TextoNulo("AreaNombre"),
        Activo = r.Booleano("Activo"),
        NoConformidades = r.Entero("NoConformidades"),
        Abiertas = r.Entero("Abiertas"),
        EnUso = r.Booleano("EnUso")
    };

    public static RazonTiempoMuertoCatalogo RazonTiempoMuerto(DbDataReader r) => new()
    {
        RazonId = r.Entero("RazonId"),
        Nombre = r.Texto("Nombre"),
        Activo = r.Booleano("Activo"),
        UsosEnSetup = r.Entero("UsosEnSetup"),
        UsosEnProduccion = r.Entero("UsosEnProduccion"),
        EnUso = r.Booleano("EnUso")
    };

    public static AreaOpcion AreaOpcion(DbDataReader r) => new()
    {
        AreaId = r.Entero("AreaId"),
        Nombre = r.Texto("Nombre"),
        Activo = r.Booleano("Activo")
    };

    public static LineaOpcion LineaOpcion(DbDataReader r) => new()
    {
        LineaId = r.Entero("LineaId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        Activo = r.Booleano("Activo")
    };

    public static MaquinaOpcion MaquinaOpcion(DbDataReader r) => new()
    {
        MaquinaId = r.Entero("MaquinaId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        LineaId = r.EnteroNulo("LineaId"),
        LineaNombre = r.TextoNulo("LineaNombre"),
        Activo = r.Booleano("Activo")
    };

    public static ProductoOpcion ProductoOpcion(DbDataReader r) => new()
    {
        ProductoId = r.Entero("ProductoId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        Activo = r.Booleano("Activo")
    };

    public static OpcionClave OpcionClave(DbDataReader r) => new()
    {
        Clave = r.Texto("Clave"),
        Nombre = r.Texto("Nombre"),
        Orden = r.Entero("Orden")
    };

    /// <summary>
    /// Los procedimientos de guardado devuelven una sola fila con el id del
    /// elemento (el que ya tenía, o el recién generado). Se lee por posición
    /// porque cada catálogo lo nombra con su propia columna (AreaId, LineaId,
    /// MaquinaId...), y aquí solo interesa el número.
    /// </summary>
    public static async Task<int> LeerIdAsync(DbDataReader lector, CancellationToken ct) =>
        await lector.ReadAsync(ct) ? lector.GetInt32(0) : 0;
}
