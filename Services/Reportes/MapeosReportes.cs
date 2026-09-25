using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Reportes;

namespace calidad_app.Services.Reportes;

/// <summary>
/// Traduce cada fila devuelta por los procedimientos del módulo 7 a su modelo.
///
/// Igual que en los módulos anteriores, el mapeo es explícito columna por
/// columna y no por reflexión: si un procedimiento cambia el nombre de una
/// columna, el error aparece aquí, en un solo lugar y con nombre propio, en
/// lugar de convertirse en una propiedad que queda en su valor por defecto sin
/// que nadie se entere.
/// </summary>
internal static class MapeosReportes
{
    /* ---------- Tablero ---------- */

    public static TotalesPeriodo TotalesPeriodo(DbDataReader r) => new()
    {
        FechaDesde = r.SoloFecha("FechaDesde"),
        FechaHasta = r.SoloFecha("FechaHasta"),
        FechaDesdeAnterior = r.SoloFecha("FechaDesdeAnterior"),
        FechaHastaAnterior = r.SoloFecha("FechaHastaAnterior"),
        DiasDelPeriodo = r.Entero("DiasDelPeriodo"),

        Registros = r.Entero("Registros"),
        RegistrosAnterior = r.Entero("RegistrosAnterior"),
        Ordenes = r.Entero("Ordenes"),
        Maquinas = r.Entero("Maquinas"),

        Bobinas = r.Entero("Bobinas"),
        BobinasConformes = r.Entero("BobinasConformes"),
        BobinasNoConformes = r.Entero("BobinasNoConformes"),

        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicio = r.Decimal("KgDesperdicio"),
        KgDuro = r.Decimal("KgDuro"),
        KgRefill = r.Decimal("KgRefill"),
        KgProcesado = r.Decimal("KgProcesado"),
        KgProgramados = r.Decimal("KgProgramados"),
        KgProducidosDeOrdenes = r.Decimal("KgProducidosDeOrdenes"),

        TiempoMuertoMin = r.Entero("TiempoMuertoMin"),

        Mediciones = r.Entero("Mediciones"),
        MedicionesEnRango = r.Entero("MedicionesEnRango"),
        MedicionesFueraRango = r.Entero("MedicionesFueraRango"),

        Alertas = r.Entero("Alertas"),
        AlertasCriticas = r.Entero("AlertasCriticas"),
        AlertasPendientes = r.Entero("AlertasPendientes"),

        NoConformidades = r.Entero("NoConformidades"),
        NoConformidadesAbiertas = r.Entero("NoConformidadesAbiertas"),
        NoConformidadesCerradas = r.Entero("NoConformidadesCerradas"),

        RegistrosLiberados = r.Entero("RegistrosLiberados"),
        Lotes = r.Entero("Lotes"),
        LotesLiberados = r.Entero("LotesLiberados"),
        LotesCertificados = r.Entero("LotesCertificados")
    };

    public static IndicadorKpi IndicadorKpi(DbDataReader r) => new()
    {
        Clave = r.Texto("Clave"),
        Orden = r.Entero("Orden"),
        Unidad = r.Texto("Unidad"),
        MejorSiSube = r.Booleano("MejorSiSube"),
        Valor = r.DecimalNulo("Valor"),
        ValorAnterior = r.DecimalNulo("ValorAnterior"),
        Variacion = r.DecimalNulo("Variacion"),
        VariacionPorcentual = r.DecimalNulo("VariacionPorcentual"),
        MetaId = r.EnteroNulo("MetaId"),
        ValorMeta = r.DecimalNulo("ValorMeta"),
        MetaUnidad = r.TextoNulo("MetaUnidad"),
        CumpleMeta = r.BooleanoNulo("CumpleMeta")
    };

    public static PuntoTendencia PuntoTendencia(DbDataReader r) => new()
    {
        Clave = r.Texto("Clave"),
        Inicio = r.SoloFecha("Inicio"),
        Fin = r.SoloFecha("Fin"),
        Registros = r.Entero("Registros"),
        Ordenes = r.Entero("Ordenes"),
        Maquinas = r.Entero("Maquinas"),
        Bobinas = r.Entero("Bobinas"),
        BobinasConformes = r.Entero("BobinasConformes"),
        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicio = r.Decimal("KgDesperdicio"),
        KgProcesado = r.Decimal("KgProcesado"),
        Mediciones = r.Entero("Mediciones"),
        MedicionesEnRango = r.Entero("MedicionesEnRango"),
        Alertas = r.Entero("Alertas"),
        AlertasCriticas = r.Entero("AlertasCriticas"),
        NoConformidades = r.Entero("NoConformidades"),
        TiempoMuertoMin = r.Entero("TiempoMuertoMin"),
        PorcentajeDesperdicio = r.DecimalNulo("PorcentajeDesperdicio"),
        PorcentajeConformes = r.DecimalNulo("PorcentajeConformes"),
        PorcentajeFichaTecnica = r.DecimalNulo("PorcentajeFichaTecnica")
    };

    public static GrupoIndicador GrupoIndicador(DbDataReader r) => new()
    {
        ClaveId = r.EnteroNulo("ClaveId"),
        ClaveTexto = r.Texto("ClaveTexto"),
        Etiqueta = r.Texto("Etiqueta"),
        Registros = r.Entero("Registros"),
        Ordenes = r.Entero("Ordenes"),
        Bobinas = r.Entero("Bobinas"),
        BobinasConformes = r.Entero("BobinasConformes"),
        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicio = r.Decimal("KgDesperdicio"),
        KgProcesado = r.Decimal("KgProcesado"),
        Mediciones = r.Entero("Mediciones"),
        MedicionesEnRango = r.Entero("MedicionesEnRango"),
        Alertas = r.Entero("Alertas"),
        AlertasCriticas = r.Entero("AlertasCriticas"),
        NoConformidades = r.Entero("NoConformidades"),
        TiempoMuertoMin = r.Entero("TiempoMuertoMin"),
        PorcentajeDesperdicio = r.DecimalNulo("PorcentajeDesperdicio"),
        PorcentajeConformes = r.DecimalNulo("PorcentajeConformes"),
        PorcentajeFichaTecnica = r.DecimalNulo("PorcentajeFichaTecnica")
    };

    public static CausaPareto CausaPareto(DbDataReader r) => new()
    {
        ClaveId = r.EnteroNulo("ClaveId"),
        Etiqueta = r.Texto("Etiqueta"),
        Detalle = r.TextoNulo("Detalle"),
        Eventos = r.Entero("Eventos"),
        Valor = r.Decimal("Valor"),
        Unidad = r.Texto("Unidad"),
        Porcentaje = r.DecimalNulo("Porcentaje"),
        PorcentajeAcumulado = r.DecimalNulo("PorcentajeAcumulado")
    };

    /* ---------- Foco de calidad ---------- */

    public static AlertasKpi AlertasKpi(DbDataReader r) => new()
    {
        Total = r.Entero("Total"),
        Pendientes = r.Entero("Pendientes"),
        Atendidas = r.Entero("Atendidas"),
        Criticas = r.Entero("Criticas"),
        CriticasPendientes = r.Entero("CriticasPendientes"),
        ConNoConformidad = r.Entero("ConNoConformidad"),
        HorasAtencionPromedio = r.DecimalNulo("HorasAtencionPromedio"),
        HorasAtencionMaxima = r.DecimalNulo("HorasAtencionMaxima"),
        HorasPendienteMasAntigua = r.DecimalNulo("HorasPendienteMasAntigua")
    };

    public static NoConformidadPorSeveridad NcPorSeveridad(DbDataReader r) => new()
    {
        SeveridadId = r.Entero("SeveridadId"),
        SeveridadNombre = r.Texto("SeveridadNombre"),
        Total = r.Entero("Total"),
        Abiertas = r.Entero("Abiertas"),
        Cerradas = r.Entero("Cerradas"),
        DiasCierrePromedio = r.DecimalNulo("DiasCierrePromedio"),
        DiasAbiertaPromedio = r.DecimalNulo("DiasAbiertaPromedio")
    };

    public static NoConformidadPorEstado NcPorEstado(DbDataReader r) => new()
    {
        EstadoId = r.Entero("EstadoId"),
        EstadoNombre = r.Texto("EstadoNombre"),
        Orden = r.Entero("Orden"),
        EsFinal = r.Booleano("EsFinal"),
        Total = r.Entero("Total")
    };

    public static EmbudoLiberacion EmbudoLiberacion(DbDataReader r) => new()
    {
        Registros = r.Entero("Registros"),
        RegistrosConDespeje = r.Entero("RegistrosConDespeje"),
        RegistrosConCierre = r.Entero("RegistrosConCierre"),
        Lotes = r.Entero("Lotes"),
        LotesLiberados = r.Entero("LotesLiberados"),
        LotesCertificados = r.Entero("LotesCertificados"),
        PorcentajeLiberados = r.DecimalNulo("PorcentajeLiberados"),
        PorcentajeCertificados = r.DecimalNulo("PorcentajeCertificados")
    };

    /* ---------- Reportes ---------- */

    public static FilaProduccion FilaProduccion(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        Fecha = r.SoloFecha("Fecha"),
        Estado = r.Texto("Estado"),
        Turno = r.Texto("Turno"),
        Area = r.Texto("Area"),
        Linea = r.Texto("Linea"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        OperadorCodigo = r.Texto("OperadorCodigo"),
        Operador = r.Texto("Operador"),
        NumeroOP = r.Texto("NumeroOP"),
        Cliente = r.Texto("Cliente"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        Producto = r.Texto("Producto"),
        Bobinas = r.Entero("Bobinas"),
        BobinasConformes = r.Entero("BobinasConformes"),
        BobinasNoConformes = r.Entero("BobinasNoConformes"),
        KgProducidos = r.Decimal("KgProducidos"),
        KgDesperdicioSetup = r.Decimal("KgDesperdicioSetup"),
        KgDesperdicioProduccion = r.Decimal("KgDesperdicioProduccion"),
        KgDesperdicio = r.Decimal("KgDesperdicio"),
        KgDuro = r.Decimal("KgDuro"),
        KgRefill = r.Decimal("KgRefill"),
        KgProcesado = r.Decimal("KgProcesado"),
        PorcentajeDesperdicio = r.DecimalNulo("PorcentajeDesperdicio"),
        HorasSetup = r.Decimal("HorasSetup"),
        HorasProduccion = r.Decimal("HorasProduccion"),
        TiempoMuertoMin = r.Entero("TiempoMuertoMin"),
        Mediciones = r.Entero("Mediciones"),
        MedicionesEnRango = r.Entero("MedicionesEnRango"),
        MedicionesFueraRango = r.Entero("MedicionesFueraRango"),
        PorcentajeFichaTecnica = r.DecimalNulo("PorcentajeFichaTecnica"),
        Alertas = r.Entero("Alertas"),
        AlertasPendientes = r.Entero("AlertasPendientes"),
        NoConformidades = r.Entero("NoConformidades"),
        ConDespeje = r.Booleano("ConDespeje"),
        ConCierre = r.Booleano("ConCierre")
    };

    public static FilaNoConformidad FilaNoConformidad(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo"),
        FechaRegistro = r.Fecha("FechaRegistro"),
        Severidad = r.Texto("Severidad"),
        Estado = r.Texto("Estado"),
        EsFinal = r.Booleano("EsFinal"),
        TipoDefecto = r.Texto("TipoDefecto"),
        Area = r.Texto("Area"),
        Descripcion = r.Texto("Descripcion"),
        CausaRaiz = r.TextoNulo("CausaRaiz"),
        AccionCorrectiva = r.TextoNulo("AccionCorrectiva"),
        IdRegistro = r.TextoNulo("IdRegistro"),
        NumeroOP = r.TextoNulo("NumeroOP"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        Cliente = r.Texto("Cliente"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        Turno = r.Texto("Turno"),
        Linea = r.Texto("Linea"),
        RegistradaPor = r.Texto("RegistradaPor"),
        Responsable = r.TextoNulo("Responsable"),
        FechaCierre = r.FechaNula("FechaCierre"),
        DiasDeAtencion = r.Decimal("DiasDeAtencion"),
        BobinasVinculadas = r.Entero("BobinasVinculadas"),
        KgVinculados = r.Decimal("KgVinculados")
    };

    public static FilaAlerta FilaAlerta(DbDataReader r) => new()
    {
        AlertaId = r.Entero("AlertaId"),
        FechaDeteccion = r.Fecha("FechaDeteccion"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        Parametro = r.Texto("Parametro"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        Desviacion = r.DecimalNulo("Desviacion"),
        IdRegistro = r.TextoNulo("IdRegistro"),
        NumeroOP = r.TextoNulo("NumeroOP"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        Producto = r.Texto("Producto"),
        Bobina = r.EnteroNulo("Bobina"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        Linea = r.Texto("Linea"),
        Turno = r.Texto("Turno"),
        Operador = r.Texto("Operador"),
        Atendida = r.Booleano("Atendida"),
        FechaAtencion = r.FechaNula("FechaAtencion"),
        AtendidaPor = r.Texto("AtendidaPor"),
        HorasHastaAtencion = r.Decimal("HorasHastaAtencion"),
        Observacion = r.TextoNulo("Observacion"),
        NoConformidad = r.TextoNulo("NoConformidad")
    };

    public static FilaLote FilaLote(DbDataReader r) => new()
    {
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
        Fecha = r.SoloFecha("Fecha"),
        Estado = r.Texto("Estado"),
        NumeroOP = r.Texto("NumeroOP"),
        Cliente = r.Texto("Cliente"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        Producto = r.Texto("Producto"),
        Bobinas = r.Entero("Bobinas"),
        BobinasConformes = r.Entero("BobinasConformes"),
        KgProducidos = r.Decimal("KgProducidos"),
        Maquinas = r.Entero("Maquinas"),
        FechaLiberacion = r.FechaNula("FechaLiberacion"),
        LiberadoPor = r.Texto("LiberadoPor"),
        CodigoCertificado = r.TextoNulo("CodigoCertificado"),
        FechaCertificado = r.FechaNula("FechaCertificado"),
        CertificadoPor = r.Texto("CertificadoPor"),
        TieneCertificado = r.Booleano("TieneCertificado"),
        NoConformidades = r.Entero("NoConformidades")
    };
}
