using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Produccion;

namespace calidad_app.Services.Produccion;

/// <summary>
/// Traduce cada fila devuelta por los procedimientos del módulo de producción
/// a su modelo.
///
/// Igual que en los módulos anteriores, el mapeo es explícito columna por
/// columna y no por reflexión: si un procedimiento cambia el nombre de una
/// columna, el error aparece aquí, en un solo lugar y con nombre propio, en
/// lugar de convertirse en una propiedad que queda en su valor por defecto sin
/// que nadie se entere.
/// </summary>
internal static class MapeosProduccion
{
    /* ---------- Órdenes de producción ---------- */

    public static OrdenResumen OrdenResumen(DbDataReader r) =>
        LlenarOrden(new OrdenResumen(), r);

    public static OrdenDetalle OrdenDetalle(DbDataReader r)
    {
        var orden = LlenarOrden(new OrdenDetalle(), r);

        orden.Estructura = r.TextoNulo("Estructura");
        orden.FichaId = r.EnteroNulo("FichaId");
        orden.KgDesperdicio = r.Decimal("KgDesperdicio");
        orden.KgDuro = r.Decimal("KgDuro");
        orden.KgRefill = r.Decimal("KgRefill");
        orden.TiempoMuertoMin = r.Entero("TiempoMuertoMin");

        return orden;
    }

    /// <summary>
    /// Las columnas que comparten la lista y el detalle. El detalle las repite
    /// todas (es el mismo encabezado con más datos), así que se llenan una vez
    /// y el mapeo del detalle solo agrega lo suyo.
    /// </summary>
    private static T LlenarOrden<T>(T orden, DbDataReader r) where T : OrdenResumen
    {
        orden.OrdenId = r.Entero("OrdenId");
        orden.NumeroOP = r.Texto("NumeroOP");
        orden.Estado = r.Texto("Estado");
        orden.FechaCreacion = r.Fecha("FechaCreacion");
        orden.KgProgramados = r.DecimalNulo("KgProgramados");

        orden.ClienteId = r.Entero("ClienteId");
        orden.ClienteCodigo = r.Texto("ClienteCodigo");
        orden.ClienteNombre = r.Texto("ClienteNombre");

        orden.ProductoId = r.Entero("ProductoId");
        orden.ProductoCodigo = r.Texto("ProductoCodigo");
        orden.ProductoNombre = r.Texto("ProductoNombre");

        orden.Registros = r.Entero("Registros");
        orden.RegistrosAbiertos = r.Entero("RegistrosAbiertos");
        orden.PrimerRegistro = r.SoloFechaNula("PrimerRegistro");
        orden.UltimoRegistro = r.SoloFechaNula("UltimoRegistro");

        orden.KgProducidos = r.Decimal("KgProducidos");
        orden.Bobinas = r.Entero("Bobinas");
        orden.BobinasNoConformes = r.Entero("BobinasNoConformes");

        orden.Lotes = r.Entero("Lotes");
        orden.LotesSinLiberar = r.Entero("LotesSinLiberar");
        orden.NoConformidadesAbiertas = r.Entero("NoConformidadesAbiertas");

        orden.PorcentajeAvance = r.DecimalNulo("PorcentajeAvance");
        orden.TieneRegistros = r.Booleano("TieneRegistros");
        orden.PuedeCerrarse = r.Booleano("PuedeCerrarse");
        orden.ProductoConFicha = r.Booleano("ProductoConFicha");

        return orden;
    }

    public static ResumenOrdenes ResumenOrdenes(DbDataReader r) => new()
    {
        Total = r.Entero("Total"),
        Abiertas = r.Entero("Abiertas"),
        EnProceso = r.Entero("EnProceso"),
        Cerradas = r.Entero("Cerradas"),
        KgProgramados = r.Decimal("KgProgramados"),
        KgProducidos = r.Decimal("KgProducidos"),
        RegistrosAbiertos = r.Entero("RegistrosAbiertos"),
        LotesSinLiberar = r.Entero("LotesSinLiberar"),
        OrdenesConNoConformidades = r.Entero("OrdenesConNoConformidades"),
        PorcentajeAvance = r.DecimalNulo("PorcentajeAvance")
    };

    public static RegistroDeOrden RegistroDeOrden(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        Fecha = r.SoloFecha("Fecha"),
        Estado = r.Texto("Estado"),
        Bloqueado = r.Booleano("Bloqueado"),
        FechaHoraInicio = r.FechaNula("FechaHoraInicio"),
        TurnoId = r.Entero("TurnoId"),
        TurnoNombre = r.Texto("TurnoNombre"),
        MaquinaId = r.Entero("MaquinaId"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        LineaId = r.EnteroNulo("LineaId"),
        LineaCodigo = r.TextoNulo("LineaCodigo"),
        LineaNombre = r.TextoNulo("LineaNombre"),
        OperadorId = r.Entero("OperadorId"),
        OperadorCodigo = r.Texto("OperadorCodigo"),
        OperadorNombre = r.Texto("OperadorNombre"),
        Bobinas = r.Entero("Bobinas"),
        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicioSetup = r.DecimalNulo("KgDesperdicioSetup"),
        KgDesperdicioProduccion = r.DecimalNulo("KgDesperdicioProduccion"),
        DespejeFirmado = r.Booleano("DespejeFirmado"),
        DespejeFecha = r.FechaNula("DespejeFecha"),
        CierreFirmado = r.Booleano("CierreFirmado"),
        CierreFecha = r.FechaNula("CierreFecha")
    };

    public static LoteDeOrden LoteDeOrden(DbDataReader r) => new()
    {
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        Estado = r.Texto("Estado"),
        FechaCreacion = r.Fecha("FechaCreacion"),
        TotalBobinas = r.Entero("TotalBobinas"),
        PesoTotal = r.Decimal("PesoTotal"),
        BobinasNoConformes = r.Entero("BobinasNoConformes"),
        CertificadoId = r.EnteroNulo("CertificadoId"),
        CertificadoCodigo = r.TextoNulo("CertificadoCodigo"),
        CertificadoFecha = r.FechaNula("CertificadoFecha"),
        TieneCertificado = r.Booleano("TieneCertificado")
    };

    public static BobinaDeOrden BobinaDeOrden(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        IdBobi = r.Entero("IdBobi"),
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        Fecha = r.SoloFecha("Fecha"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        OperadorNombre = r.Texto("OperadorNombre"),
        LoteId = r.EnteroNulo("LoteId"),
        CodigoLote = r.TextoNulo("CodigoLote"),
        Peso = r.DecimalNulo("Peso"),
        Metros = r.DecimalNulo("Metros"),
        Ok = r.Booleano("Ok"),
        Confirmada = r.Booleano("Confirmada"),
        Bloqueada = r.Booleano("Bloqueada"),
        EsConforme = r.Booleano("EsConforme"),
        NoConformidadId = r.EnteroNulo("NoConformidadId"),
        NoConformidadCodigo = r.TextoNulo("NoConformidadCodigo")
    };

    public static NoConformidadDeOrden NoConformidadDeOrden(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo"),
        Descripcion = r.Texto("Descripcion"),
        FechaRegistro = r.Fecha("FechaRegistro"),
        RegistroId = r.EnteroNulo("RegistroId"),
        IdRegistro = r.TextoNulo("IdRegistro"),
        TipoDefecto = r.Texto("TipoDefecto"),
        Severidad = r.Texto("Severidad"),
        Estado = r.Texto("Estado"),
        EsFinal = r.Booleano("EsFinal"),
        AreaNombre = r.Texto("AreaNombre"),
        RegistradaPor = r.Texto("RegistradaPor"),
        Responsable = r.TextoNulo("Responsable")
    };

    /* ---------- Asignación operador - máquina - turno ---------- */

    public static CeldaAsignacion CeldaAsignacion(DbDataReader r) => new()
    {
        Fecha = r.SoloFecha("Fecha"),
        MaquinaId = r.Entero("MaquinaId"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        MaquinaActiva = r.Booleano("MaquinaActiva"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        LineaId = r.EnteroNulo("LineaId"),
        LineaCodigo = r.TextoNulo("LineaCodigo"),
        LineaNombre = r.TextoNulo("LineaNombre"),
        TurnoId = r.Entero("TurnoId"),
        TurnoNombre = r.Texto("TurnoNombre"),
        AsignacionId = r.EnteroNulo("AsignacionId"),
        AsignadaEl = r.FechaNula("AsignadaEl"),
        OperadorId = r.EnteroNulo("OperadorId"),
        OperadorCodigo = r.TextoNulo("OperadorCodigo"),
        OperadorNombre = r.TextoNulo("OperadorNombre"),
        OperadorActivo = r.BooleanoNulo("OperadorActivo"),
        Registros = r.Entero("Registros"),
        RegistrosDeOtroOperador = r.Entero("RegistrosDeOtroOperador"),
        Asignada = r.Booleano("Asignada"),
        PuedeEliminarse = r.Booleano("PuedeEliminarse")
    };

    public static OperadorDisponible OperadorDisponible(DbDataReader r) => new()
    {
        OperadorId = r.Entero("OperadorId"),
        OperadorCodigo = r.Texto("OperadorCodigo"),
        OperadorNombre = r.Texto("OperadorNombre"),
        AreaId = r.EnteroNulo("AreaId"),
        AreaNombre = r.TextoNulo("AreaNombre"),
        RolNombre = r.Texto("RolNombre"),
        AsignacionId = r.EnteroNulo("AsignacionId"),
        MaquinaId = r.EnteroNulo("MaquinaId"),
        MaquinaCodigo = r.TextoNulo("MaquinaCodigo"),
        MaquinaNombre = r.TextoNulo("MaquinaNombre"),
        Ocupado = r.Booleano("Ocupado"),
        TurnosEnElDia = r.Entero("TurnosEnElDia")
    };

    public static ResultadoCopiaAsignaciones ResultadoCopia(DbDataReader r) => new()
    {
        FechaDestino = r.SoloFecha("FechaDestino"),
        EnOrigen = r.Entero("EnOrigen"),
        Copiadas = r.Entero("Copiadas"),
        Omitidas = r.Entero("Omitidas")
    };

    /* ---------- Control de desperdicio ---------- */

    public static ResumenDesperdicio ResumenDesperdicio(DbDataReader r) => new()
    {
        FechaDesde = r.SoloFecha("FechaDesde"),
        FechaHasta = r.SoloFecha("FechaHasta"),
        Registros = r.Entero("Registros"),
        Ordenes = r.Entero("Ordenes"),
        Maquinas = r.Entero("Maquinas"),
        Bobinas = r.Entero("Bobinas"),
        BobinasNoConformes = r.Entero("BobinasNoConformes"),
        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicioSetup = r.Decimal("KgDesperdicioSetup"),
        KgDesperdicioProduccion = r.Decimal("KgDesperdicioProduccion"),
        KgDesperdicio = r.Decimal("KgDesperdicio"),
        KgDuro = r.Decimal("KgDuro"),
        KgRefill = r.Decimal("KgRefill"),
        KgProcesado = r.Decimal("KgProcesado"),
        PorcentajeDesperdicio = r.DecimalNulo("PorcentajeDesperdicio"),
        PorcentajeDuro = r.DecimalNulo("PorcentajeDuro"),
        PorcentajeRefill = r.DecimalNulo("PorcentajeRefill"),
        TiempoMuertoMin = r.Entero("TiempoMuertoMin"),
        HorasSetup = r.Decimal("HorasSetup"),
        HorasProduccion = r.Decimal("HorasProduccion")
    };

    public static MetaComparada MetaComparada(DbDataReader r) => new()
    {
        Concepto = r.Texto("Concepto"),
        Nombre = r.Texto("Nombre"),
        Orden = r.Entero("Orden"),
        EsMaximo = r.Booleano("EsMaximo"),
        ValorReal = r.DecimalNulo("ValorReal"),
        MetaId = r.EnteroNulo("MetaId"),
        ValorMeta = r.DecimalNulo("ValorMeta"),
        Unidad = r.TextoNulo("Unidad"),
        MetaVigenteDesde = r.SoloFechaNula("MetaVigenteDesde"),
        Desviacion = r.DecimalNulo("Desviacion"),
        Cumple = r.BooleanoNulo("Cumple")
    };

    public static RegistroDesperdicio RegistroDesperdicio(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        Fecha = r.SoloFecha("Fecha"),
        Estado = r.Texto("Estado"),
        TurnoId = r.Entero("TurnoId"),
        TurnoNombre = r.Texto("TurnoNombre"),
        MaquinaId = r.Entero("MaquinaId"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        LineaId = r.EnteroNulo("LineaId"),
        LineaCodigo = r.TextoNulo("LineaCodigo"),
        LineaNombre = r.TextoNulo("LineaNombre"),
        OperadorNombre = r.Texto("OperadorNombre"),
        OrdenId = r.Entero("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        ProductoId = r.Entero("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        Bobinas = r.Entero("Bobinas"),
        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicioSetup = r.Decimal("KgDesperdicioSetup"),
        KgDesperdicioProduccion = r.Decimal("KgDesperdicioProduccion"),
        KgDesperdicio = r.Decimal("KgDesperdicio"),
        KgDuro = r.Decimal("KgDuro"),
        KgRefill = r.Decimal("KgRefill"),
        KgProcesado = r.Decimal("KgProcesado"),
        PorcentajeDesperdicio = r.DecimalNulo("PorcentajeDesperdicio"),
        TiempoMuertoMin = r.Entero("TiempoMuertoMin"),
        RazonSetup = r.TextoNulo("RazonSetup"),
        RazonProduccion = r.TextoNulo("RazonProduccion"),
        HorasSetup = r.DecimalNulo("HorasSetup"),
        HorasProduccion = r.DecimalNulo("HorasProduccion"),
        MetaId = r.EnteroNulo("MetaId"),
        ValorMeta = r.DecimalNulo("ValorMeta"),
        MetaUnidad = r.TextoNulo("MetaUnidad"),
        ValorComparado = r.DecimalNulo("ValorComparado"),
        CumpleMeta = r.BooleanoNulo("CumpleMeta")
    };

    public static GrupoDesperdicio GrupoDesperdicio(DbDataReader r) => new()
    {
        ClaveId = r.EnteroNulo("ClaveId"),
        ClaveTexto = r.Texto("ClaveTexto"),
        Etiqueta = r.Texto("Etiqueta"),
        Registros = r.Entero("Registros"),
        Bobinas = r.Entero("Bobinas"),
        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicioSetup = r.Decimal("KgDesperdicioSetup"),
        KgDesperdicioProduccion = r.Decimal("KgDesperdicioProduccion"),
        KgDesperdicio = r.Decimal("KgDesperdicio"),
        KgDuro = r.Decimal("KgDuro"),
        KgRefill = r.Decimal("KgRefill"),
        KgProcesado = r.Decimal("KgProcesado"),
        PorcentajeDesperdicio = r.DecimalNulo("PorcentajeDesperdicio"),
        TiempoMuertoMin = r.Entero("TiempoMuertoMin")
    };

    public static ParoPorRazon ParoPorRazon(DbDataReader r) => new()
    {
        RazonId = r.EnteroNulo("RazonId"),
        RazonNombre = r.Texto("RazonNombre"),
        Eventos = r.Entero("Eventos"),
        EventosSetup = r.Entero("EventosSetup"),
        EventosProduccion = r.Entero("EventosProduccion"),
        MinutosTotal = r.Entero("MinutosTotal"),
        MinutosSetup = r.Entero("MinutosSetup"),
        MinutosProduccion = r.Entero("MinutosProduccion"),
        Registros = r.Entero("Registros"),
        HorasTotal = r.Decimal("HorasTotal"),
        PorcentajeDelTotal = r.DecimalNulo("PorcentajeDelTotal"),
        PorcentajeAcumulado = r.DecimalNulo("PorcentajeAcumulado")
    };

    /* ---------- Selectores ---------- */

    public static ClienteOpcion ClienteOpcion(DbDataReader r) => new()
    {
        ClienteId = r.Entero("ClienteId"),
        Codigo = r.Texto("Codigo"),
        Nombre = r.Texto("Nombre"),
        Activo = r.Booleano("Activo")
    };

    public static TurnoOpcion TurnoOpcion(DbDataReader r) => new()
    {
        TurnoId = r.Entero("TurnoId"),
        Nombre = r.Texto("Nombre")
    };

    public static OperadorOpcion OperadorOpcion(DbDataReader r) => new()
    {
        UsuarioId = r.Entero("UsuarioId"),
        Codigo = r.Texto("Codigo"),
        NombreCompleto = r.Texto("NombreCompleto"),
        AreaId = r.EnteroNulo("AreaId"),
        AreaNombre = r.TextoNulo("AreaNombre"),
        RolNombre = r.Texto("RolNombre"),
        Activo = r.Booleano("Activo")
    };

    public static RazonOpcion RazonOpcion(DbDataReader r) => new()
    {
        RazonId = r.Entero("RazonId"),
        Nombre = r.Texto("Nombre"),
        Activo = r.Booleano("Activo")
    };

    /// <summary>
    /// Los procedimientos de guardado devuelven una sola fila con el id del
    /// elemento (el que ya tenía, o el recién generado). Se lee por posición
    /// porque cada uno lo nombra con su propia columna (OrdenId,
    /// AsignacionId), y aquí solo interesa el número.
    /// </summary>
    public static async Task<int> LeerIdAsync(DbDataReader lector, CancellationToken ct) =>
        await lector.ReadAsync(ct) ? lector.GetInt32(0) : 0;
}
