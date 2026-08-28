using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Inspeccion;

namespace calidad_app.Services.Inspeccion;

/// <summary>
/// Traduce cada fila devuelta por los procedimientos a su DTO.
///
/// El mapeo es explícito, columna por columna, y no por reflexión: si un
/// procedimiento cambia el nombre de una columna, el error aparece aquí, en un
/// solo lugar y con nombre propio, en vez de convertirse en una propiedad que
/// silenciosamente queda en su valor por defecto.
/// </summary>
internal static class MapeosInspeccion
{
    public static RegistroResumen RegistroResumen(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        OrdenId = r.Entero("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        EstadoOrden = r.Texto("EstadoOrden"),
        ClienteId = r.Entero("ClienteId"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoId = r.Entero("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        Fecha = r.SoloFecha("Fecha"),
        FechaHoraInicio = r.FechaNula("FechaHoraInicio"),
        TurnoId = r.Entero("TurnoId"),
        TurnoNombre = r.Texto("TurnoNombre"),
        MaquinaId = r.Entero("MaquinaId"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        LineaId = r.EnteroNulo("LineaId"),
        LineaNombre = r.TextoNulo("LineaNombre"),
        OperadorId = r.Entero("OperadorId"),
        OperadorNombre = r.Texto("OperadorNombre"),
        TipoRegistroId = r.Entero("TipoRegistroId"),
        TipoRegistroCodigo = r.Texto("TipoRegistroCodigo"),
        TipoRegistroNombre = r.Texto("TipoRegistroNombre"),
        FormatoCodigo = r.Texto("FormatoCodigo"),
        Estado = r.Texto("Estado"),
        Bloqueado = r.Booleano("Bloqueado"),
        FechaCreacion = r.Fecha("FechaCreacion"),
        TotalBobinas = r.Entero("TotalBobinas"),
        BobinasConfirmadas = r.Entero("BobinasConfirmadas"),
        PesoTotal = r.Decimal("PesoTotal"),
        ParametrosFueraDeRango = r.Entero("ParametrosFueraDeRango"),
        AlertasPendientes = r.Entero("AlertasPendientes")
    };

    public static RegistroDetalle RegistroDetalle(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        OrdenId = r.Entero("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        EstadoOrden = r.Texto("EstadoOrden"),
        KgProgramados = r.DecimalNulo("KgProgramados"),
        ClienteId = r.Entero("ClienteId"),
        ClienteCodigo = r.Texto("ClienteCodigo"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoId = r.Entero("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        ProductoEstructura = r.TextoNulo("ProductoEstructura"),
        Fecha = r.SoloFecha("Fecha"),
        FechaHoraInicio = r.FechaNula("FechaHoraInicio"),
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
        OperadorId = r.Entero("OperadorId"),
        OperadorCodigo = r.Texto("OperadorCodigo"),
        OperadorNombre = r.Texto("OperadorNombre"),
        TipoRegistroId = r.Entero("TipoRegistroId"),
        TipoRegistroCodigo = r.Texto("TipoRegistroCodigo"),
        TipoRegistroNombre = r.Texto("TipoRegistroNombre"),
        FormatoVersionId = r.Entero("FormatoVersionId"),
        FormatoCodigo = r.Texto("FormatoCodigo"),
        FormatoVersion = r.Texto("FormatoVersion"),
        Estado = r.Texto("Estado"),
        Bloqueado = r.Booleano("Bloqueado"),
        FechaCreacion = r.Fecha("FechaCreacion"),
        FichaId = r.EnteroNulo("FichaId"),
        FichaVersion = r.TextoNulo("FichaVersion"),
        FichaVigenteDesde = r.SoloFechaNula("FichaVigenteDesde"),
        DespejeLiberado = r.Booleano("DespejeLiberado"),
        DespejeFechaLiberacion = r.FechaNula("DespejeFechaLiberacion"),
        DespejeLiberadoPor = r.TextoNulo("DespejeLiberadoPor")
    };

    public static LoteResumen Lote(DbDataReader r) => new()
    {
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        PesoTotal = r.DecimalNulo("PesoTotal"),
        Estado = r.Texto("Estado")
    };

    public static EspecificacionProceso Especificacion(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        EspecId = r.EnteroNulo("EspecId"),
        KgAProducir = r.DecimalNulo("KgAProducir"),
        AnchoProduccionMm = r.DecimalNulo("AnchoProduccionMm"),
        CalibreProduccionMic = r.DecimalNulo("CalibreProduccionMic"),
        Fuelle = r.DecimalNulo("Fuelle"),
        Color = r.TextoNulo("Color"),
        Tratado = r.BooleanoNulo("Tratado"),
        TipoBobina = r.TextoNulo("TipoBobina"),
        TipoMaterial = r.TextoNulo("TipoMaterial"),
        TipoSello = r.TextoNulo("TipoSello"),
        Estructura = r.TextoNulo("Estructura"),
        BobinasPorEmbobinador = r.EnteroNulo("BobinasPorEmbobinador"),
        Abierta = r.BooleanoNulo("Abierta"),
        Impresa = r.BooleanoNulo("Impresa"),
        UsoFinal = r.TextoNulo("UsoFinal"),
        Rotulado = r.BooleanoNulo("Rotulado"),
        MetrosAproxBobina = r.DecimalNulo("MetrosAproxBobina"),
        KgAproxBobina = r.DecimalNulo("KgAproxBobina"),
        AnchoProductoMm = r.DecimalNulo("AnchoProductoMm"),
        LargoProductoMm = r.DecimalNulo("LargoProductoMm"),
        CalibreProductoMic = r.DecimalNulo("CalibreProductoMic"),
        AnchoExtrusionMm = r.DecimalNulo("AnchoExtrusionMm"),
        CalibreExtrusionMic = r.DecimalNulo("CalibreExtrusionMic"),
        AlturaImpresion = r.DecimalNulo("AlturaImpresion"),
        Observaciones = r.TextoNulo("Observaciones"),
        Bloqueado = r.Booleano("Bloqueado"),
        Estado = r.Texto("Estado")
    };

    public static MaterialMezcla Material(DbDataReader r) => new()
    {
        RegMaterialId = r.EnteroNulo("RegMaterialId"),
        RegistroId = r.Entero("RegistroId"),
        NumeroMat = r.Entero("NumeroMat"),
        MaterialId = r.Entero("MaterialId"),
        MaterialCodigo = r.Texto("MaterialCodigo"),
        MaterialNombre = r.Texto("MaterialNombre"),
        MaterialActivo = r.Booleano("MaterialActivo"),
        CodigoFabricante = r.TextoNulo("CodigoFabricante"),
        Lote = r.TextoNulo("Lote"),
        Porcentaje = r.Decimal("Porcentaje")
    };

    public static SetupRegistro Setup(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        SetupId = r.EnteroNulo("SetupId"),
        FechaSetup = r.SoloFechaNula("FechaSetup"),
        OperadorSetupId = r.EnteroNulo("OperadorSetupId"),
        OperadorSetupNombre = r.TextoNulo("OperadorSetupNombre"),
        FechaHoraInicio = r.FechaNula("FechaHoraInicio"),
        FechaHoraFin = r.FechaNula("FechaHoraFin"),
        HorasSetup = r.DecimalNulo("HorasSetup"),
        TiempoMuertoMin = r.EnteroNulo("TiempoMuertoMin"),
        RazonId = r.EnteroNulo("RazonId"),
        RazonNombre = r.TextoNulo("RazonNombre"),
        KgDesperdicio = r.DecimalNulo("KgDesperdicio"),
        KgDuro = r.DecimalNulo("KgDuro"),
        Bloqueado = r.Booleano("Bloqueado"),
        Estado = r.Texto("Estado")
    };

    public static ProduccionRegistro Produccion(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        ProduccionId = r.EnteroNulo("ProduccionId"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        OperadorId = r.EnteroNulo("OperadorId"),
        OperadorNombre = r.TextoNulo("OperadorNombre"),
        FechaHoraInicio = r.FechaNula("FechaHoraInicio"),
        FechaHoraFin = r.FechaNula("FechaHoraFin"),
        HorasProduccion = r.DecimalNulo("HorasProduccion"),
        TiempoMuertoMin = r.EnteroNulo("TiempoMuertoMin"),
        RazonId = r.EnteroNulo("RazonId"),
        RazonNombre = r.TextoNulo("RazonNombre"),
        KgDesperdicio = r.DecimalNulo("KgDesperdicio"),
        KgRefill = r.DecimalNulo("KgRefill"),
        KgProducidos = r.Decimal("KgProducidos"),
        TotalBobinas = r.Entero("TotalBobinas"),
        Bloqueado = r.Booleano("Bloqueado"),
        Estado = r.Texto("Estado")
    };

    public static BobinaResumen Bobina(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        RegistroId = r.Entero("RegistroId"),
        IdBobi = r.Entero("IdBobi"),
        Peso = r.DecimalNulo("Peso"),
        Metros = r.DecimalNulo("Metros"),
        Ancho = r.DecimalNulo("Ancho"),
        Fuelle = r.DecimalNulo("Fuelle"),
        Calibre = r.DecimalNulo("Calibre"),
        Ok = r.Booleano("Ok"),
        Confirmada = r.Booleano("Confirmada"),
        FechaConfirmacion = r.FechaNula("FechaConfirmacion"),
        Bloqueada = r.Booleano("Bloqueada"),
        EsConforme = r.Booleano("EsConforme"),
        NoConformidadId = r.EnteroNulo("NoConformidadId"),
        NoConformidadCodigo = r.TextoNulo("NoConformidadCodigo"),
        LoteId = r.EnteroNulo("LoteId"),
        CodigoLote = r.TextoNulo("CodigoLote"),
        FechaCreacion = r.Fecha("FechaCreacion"),
        ItemsCumplidos = r.Entero("ItemsCumplidos"),
        ItemsRespondidos = r.Entero("ItemsRespondidos"),
        ParametrosFueraDeRango = r.Entero("ParametrosFueraDeRango")
    };

    public static AcumuladoMedida Acumulado(DbDataReader r) => new()
    {
        Medida = r.Texto("Medida"),
        Cantidad = r.Entero("Cantidad"),
        Total = r.DecimalNulo("Total"),
        Minimo = r.DecimalNulo("Minimo"),
        Maximo = r.DecimalNulo("Maximo"),
        Promedio = r.DecimalNulo("Promedio"),
        Desviacion = r.DecimalNulo("Desviacion")
    };

    public static ParametroMedicion Parametro(DbDataReader r) => new()
    {
        ParametroId = r.Entero("ParametroId"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        Orden = r.Entero("Orden"),
        RegParamId = r.EnteroNulo("RegParamId"),
        BobinaId = r.EnteroNulo("BobinaId"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        DentroDeRango = r.BooleanoNulo("DentroDeRango"),
        FechaRegistro = r.FechaNula("FechaRegistro"),
        ValorObjetivo = r.DecimalNulo("ValorObjetivo"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        TieneTolerancia = r.Booleano("TieneTolerancia")
    };

    /// <summary>Tolerancias de la ficha: el mismo DTO, pero sin columnas de medición.</summary>
    public static ParametroMedicion Tolerancia(DbDataReader r) => new()
    {
        ParametroId = r.Entero("ParametroId"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        Orden = r.Entero("Orden"),
        ValorObjetivo = r.DecimalNulo("ValorObjetivo"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        TieneTolerancia = r.Booleano("TieneTolerancia")
    };

    public static FichaAplicable Ficha(DbDataReader r) => new()
    {
        TieneFicha = r.Booleano("TieneFicha"),
        FichaId = r.EnteroNulo("FichaId"),
        FichaVersion = r.TextoNulo("FichaVersion"),
        FichaVigenteDesde = r.SoloFechaNula("FichaVigenteDesde"),
        ProductoId = r.Entero("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre")
    };

    public static EvaluacionResumen Evaluacion(DbDataReader r) => new()
    {
        TieneFicha = r.Booleano("TieneFicha"),
        FichaId = r.EnteroNulo("FichaId"),
        ValoresEvaluados = r.Entero("ValoresEvaluados"),
        FueraDeRango = r.Entero("FueraDeRango"),
        DentroDeRango = r.Entero("DentroDeRango"),
        NoEvaluables = r.Entero("NoEvaluables"),
        AlertasGeneradas = r.Entero("AlertasGeneradas")
    };

    public static DesviacionParametro Desviacion(DbDataReader r) => new()
    {
        RegParamId = r.EnteroNulo("RegParamId"),
        BobinaId = r.EnteroNulo("BobinaId"),
        IdBobi = r.EnteroNulo("IdBobi"),
        ParametroId = r.Entero("ParametroId"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        ValorObjetivo = r.DecimalNulo("ValorObjetivo"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        Desviacion = r.Texto("Desviacion")
    };

    /// <summary>Desviaciones devueltas por usp_Bobina_Confirmar (sin columnas de bobina).</summary>
    public static DesviacionParametro DesviacionBobina(DbDataReader r) => new()
    {
        ParametroId = r.Entero("ParametroId"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        ValorObjetivo = r.DecimalNulo("ValorObjetivo"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        Desviacion = r.Texto("Desviacion")
    };

    public static ItemChecklist Item(DbDataReader r) => new()
    {
        ItemId = r.Entero("ItemId"),
        ItemCodigo = r.Texto("ItemCodigo"),
        Texto = r.Texto("Texto"),
        Orden = r.Entero("Orden"),
        Seccion = r.TextoNulo("Seccion"),
        RegChkId = r.EnteroNulo("RegChkId"),
        Cumple = r.Booleano("Cumple"),
        Respondido = r.Booleano("Respondido")
    };

    public static ItemChecklist ItemBobina(DbDataReader r) => new()
    {
        ItemId = r.Entero("ItemId"),
        ItemCodigo = r.Texto("ItemCodigo"),
        Texto = r.Texto("Texto"),
        Orden = r.Entero("Orden"),
        Seccion = SeccionChecklist.CalidadBobina,
        BobinaId = r.EnteroNulo("BobinaId"),
        Cumple = r.Booleano("Cumple"),
        Respondido = r.Booleano("Respondido")
    };

    public static AvanceChecklist Avance(DbDataReader r) => new()
    {
        ItemsTotal = r.Entero("ItemsTotal"),
        ItemsRespondidos = r.Entero("ItemsRespondidos"),
        ItemsCumplidos = r.Entero("ItemsCumplidos"),
        Completo = r.Booleano("Completo")
    };

    public static ValidacionCompletitud Validacion(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        Estado = r.Texto("Estado"),
        ListoParaLiberar = r.Booleano("ListoParaLiberar"),
        TotalBloqueantes = r.Entero("TotalBloqueantes"),
        TotalAdvertencias = r.Entero("TotalAdvertencias")
    };

    public static HallazgoValidacion Hallazgo(DbDataReader r) => new()
    {
        Codigo = r.Texto("Codigo"),
        Severidad = r.Texto("Severidad"),
        Seccion = r.Texto("Seccion"),
        Mensaje = r.Texto("Mensaje"),
        Cantidad = r.EnteroNulo("Cantidad")
    };
}
