using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Traduce cada fila devuelta por los procedimientos del módulo de calidad a su
/// DTO.
///
/// El mapeo es explícito, columna por columna, y no por reflexión: si un
/// procedimiento cambia el nombre de una columna, el error aparece aquí, en un
/// solo lugar y con nombre propio, en vez de convertirse en una propiedad que
/// silenciosamente queda en su valor por defecto.
/// </summary>
internal static class MapeosCalidad
{
    public static AlertaResumen AlertaResumen(DbDataReader r) =>
        LlenarAlerta(new AlertaResumen(), r);

    public static AlertaDetalle AlertaDetalle(DbDataReader r)
    {
        var alerta = LlenarAlerta(new AlertaDetalle(), r);

        alerta.FechaRegistroInspeccion = r.SoloFechaNula("Fecha");
        alerta.EstadoRegistro = r.Texto("EstadoRegistro");
        alerta.ClienteNombre = r.Texto("ClienteNombre");
        alerta.BobinaPeso = r.DecimalNulo("BobinaPeso");
        alerta.BobinaEsConforme = r.BooleanoNulo("BobinaEsConforme");
        alerta.FichaId = r.EnteroNulo("FichaId");
        alerta.FichaVersion = r.TextoNulo("FichaVersion");
        alerta.ValorObjetivo = r.DecimalNulo("ValorObjetivo");
        alerta.NoConformidadEstado = r.TextoNulo("NoConformidadEstado");

        return alerta;
    }

    /// <summary>
    /// Columnas comunes al listado y al detalle. El detalle es un
    /// <see cref="AlertaResumen"/> con más contexto, así que comparten mapeo en
    /// lugar de repetir treinta asignaciones.
    /// </summary>
    private static T LlenarAlerta<T>(T alerta, DbDataReader r) where T : AlertaResumen
    {
        alerta.AlertaId = r.Entero("AlertaId");
        alerta.RegistroId = r.EnteroNulo("RegistroId");
        alerta.IdRegistro = r.Texto("IdRegistro");
        alerta.OrdenId = r.EnteroNulo("OrdenId");
        alerta.NumeroOP = r.Texto("NumeroOP");
        alerta.ProductoId = r.EnteroNulo("ProductoId");
        alerta.ProductoCodigo = r.Texto("ProductoCodigo");
        alerta.ProductoNombre = r.Texto("ProductoNombre");
        alerta.BobinaId = r.EnteroNulo("BobinaId");
        alerta.IdBobi = r.EnteroNulo("IdBobi");
        alerta.LoteId = r.EnteroNulo("LoteId");
        alerta.CodigoLote = r.TextoNulo("CodigoLote");
        alerta.ParametroId = r.Entero("ParametroId");
        alerta.ParametroCodigo = r.Texto("ParametroCodigo");
        alerta.ParametroNombre = r.Texto("ParametroNombre");
        alerta.Unidad = r.TextoNulo("Unidad");
        alerta.EsCritico = r.Booleano("EsCritico");
        alerta.ValorRegistrado = r.DecimalNulo("ValorRegistrado");
        alerta.LimiteInferior = r.DecimalNulo("LimiteInferior");
        alerta.LimiteSuperior = r.DecimalNulo("LimiteSuperior");
        alerta.Desviacion = r.Texto("Desviacion");
        alerta.MaquinaId = r.EnteroNulo("MaquinaId");
        alerta.MaquinaCodigo = r.Texto("MaquinaCodigo");
        alerta.MaquinaNombre = r.Texto("MaquinaNombre");
        alerta.AreaId = r.EnteroNulo("AreaId");
        alerta.AreaNombre = r.Texto("AreaNombre");
        alerta.TurnoId = r.EnteroNulo("TurnoId");
        alerta.TurnoNombre = r.Texto("TurnoNombre");
        alerta.OperadorId = r.EnteroNulo("OperadorId");
        alerta.OperadorNombre = r.Texto("OperadorNombre");
        alerta.FechaDeteccion = r.Fecha("FechaDeteccion");
        alerta.Atendida = r.Booleano("Atendida");
        alerta.FechaAtencion = r.FechaNula("FechaAtencion");
        alerta.AtendidaPorId = r.EnteroNulo("AtendidaPorId");
        alerta.AtendidaPorNombre = r.TextoNulo("AtendidaPorNombre");
        alerta.Observacion = r.TextoNulo("Observacion");
        alerta.NoConformidadId = r.EnteroNulo("NoConformidadId");
        alerta.NoConformidadCodigo = r.TextoNulo("NoConformidadCodigo");
        alerta.HorasSinAtender = r.Entero("HorasSinAtender");

        return alerta;
    }

    public static MedicionParametro Medicion(DbDataReader r) => new()
    {
        RegParamId = r.Entero("RegParamId"),
        BobinaId = r.EnteroNulo("BobinaId"),
        IdBobi = r.EnteroNulo("IdBobi"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        DentroDeRango = r.BooleanoNulo("DentroDeRango"),
        FechaRegistro = r.Fecha("FechaRegistro")
    };

    public static AlertaRelacionada AlertaRelacionada(DbDataReader r) => new()
    {
        AlertaId = r.Entero("AlertaId"),
        BobinaId = r.EnteroNulo("BobinaId"),
        IdBobi = r.EnteroNulo("IdBobi"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        EsCritico = r.Booleano("EsCritico"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        FechaDeteccion = r.Fecha("FechaDeteccion")
    };

    public static TotalesAlertas TotalesAlertas(DbDataReader r) => new()
    {
        TotalAlertas = r.Entero("TotalAlertas"),
        Pendientes = r.Entero("Pendientes"),
        Atendidas = r.Entero("Atendidas"),
        Criticas = r.Entero("Criticas"),
        CriticasPendientes = r.Entero("CriticasPendientes"),
        PendienteMasAntigua = r.FechaNula("PendienteMasAntigua")
    };

    public static AlertasPorParametro AlertasPorParametro(DbDataReader r) => new()
    {
        ParametroId = r.Entero("ParametroId"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        Total = r.Entero("Total"),
        Pendientes = r.Entero("Pendientes")
    };

    public static AlertasPorMaquina AlertasPorMaquina(DbDataReader r) => new()
    {
        MaquinaId = r.Entero("MaquinaId"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        AreaNombre = r.Texto("AreaNombre"),
        Total = r.Entero("Total"),
        Pendientes = r.Entero("Pendientes")
    };

    public static AlertasPorTurno AlertasPorTurno(DbDataReader r) => new()
    {
        TurnoId = r.Entero("TurnoId"),
        TurnoNombre = r.Texto("TurnoNombre"),
        Total = r.Entero("Total"),
        Pendientes = r.Entero("Pendientes")
    };

    public static ResultadoAtencion ResultadoAtencion(DbDataReader r) => new()
    {
        AlertasAtendidas = r.Entero("AlertasAtendidas"),
        AlertasYaAtendidas = r.Entero("AlertasYaAtendidas")
    };

    public static TarjetasPanel TarjetasPanel(DbDataReader r) => new()
    {
        AlertasPendientes = r.Entero("AlertasPendientes"),
        AlertasCriticasPendientes = r.Entero("AlertasCriticasPendientes"),
        NoConformidadesAbiertas = r.Entero("NoConformidadesAbiertas"),
        NoConformidadesCriticas = r.Entero("NoConformidadesCriticas"),
        LotesPorCertificar = r.Entero("LotesPorCertificar"),
        RegistrosEnProceso = r.Entero("RegistrosEnProceso")
    };

    /// <summary>
    /// Alerta de la cola del panel. El panel devuelve menos columnas que el
    /// monitor (no necesita lote, área ni datos de atención: son alertas
    /// pendientes por definición), así que tiene su propio mapeo en vez de
    /// reutilizar el del listado.
    /// </summary>
    public static AlertaResumen AlertaUrgente(DbDataReader r) => new()
    {
        AlertaId = r.Entero("AlertaId"),
        RegistroId = r.EnteroNulo("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        NumeroOP = r.Texto("NumeroOP"),
        BobinaId = r.EnteroNulo("BobinaId"),
        IdBobi = r.EnteroNulo("IdBobi"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        TurnoNombre = r.Texto("TurnoNombre"),
        FechaDeteccion = r.Fecha("FechaDeteccion"),
        HorasSinAtender = r.Entero("HorasSinAtender")
    };

    public static NoConformidadPendiente NoConformidadPendiente(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo"),
        Descripcion = r.Texto("Descripcion"),
        SeveridadNombre = r.Texto("SeveridadNombre"),
        EstadoNombre = r.Texto("EstadoNombre"),
        AreaNombre = r.Texto("AreaNombre"),
        TipoDefectoNombre = r.Texto("TipoDefectoNombre"),
        ResponsableNombre = r.TextoNulo("ResponsableNombre"),
        FechaRegistro = r.Fecha("FechaRegistro"),
        DiasAbierta = r.Entero("DiasAbierta")
    };

    public static LotePorCertificar LotePorCertificar(DbDataReader r) => new()
    {
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
        NumeroOP = r.Texto("NumeroOP"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        TotalBobinas = r.Entero("TotalBobinas"),
        PesoTotal = r.DecimalNulo("PesoTotal")
    };

    /* ---- No conformidades ---- */

    public static NoConformidadResumen NoConformidadResumen(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo"),
        RegistroId = r.EnteroNulo("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        OrdenId = r.EnteroNulo("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        TipoDefectoId = r.Entero("TipoDefectoId"),
        TipoDefectoNombre = r.Texto("TipoDefectoNombre"),
        SeveridadId = r.Entero("SeveridadId"),
        SeveridadNombre = r.Texto("SeveridadNombre"),
        EstadoActualId = r.Entero("EstadoActualId"),
        EstadoNombre = r.Texto("EstadoNombre"),
        EstadoOrden = r.Entero("EstadoOrden"),
        EsFinal = r.Booleano("EsFinal"),
        Descripcion = r.Texto("Descripcion"),
        ResponsableId = r.EnteroNulo("ResponsableId"),
        ResponsableNombre = r.TextoNulo("ResponsableNombre"),
        RegistradaPorId = r.Entero("RegistradaPorId"),
        RegistradaPorNombre = r.Texto("RegistradaPorNombre"),
        FechaRegistro = r.Fecha("FechaRegistro"),
        DiasAbierta = r.Entero("DiasAbierta"),
        TieneCausaRaiz = r.Booleano("TieneCausaRaiz"),
        TieneAccionCorrectiva = r.Booleano("TieneAccionCorrectiva"),
        TotalEvidencias = r.Entero("TotalEvidencias"),
        BobinasAfectadas = r.Entero("BobinasAfectadas"),
        AlertasVinculadas = r.Entero("AlertasVinculadas")
    };

    public static NoConformidadDetalle NoConformidadDetalle(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo"),
        RegistroId = r.EnteroNulo("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        FechaRegistroInspeccion = r.SoloFechaNula("FechaRegistroInspeccion"),
        OrdenId = r.EnteroNulo("OrdenResueltaId"),
        NumeroOP = r.Texto("NumeroOP"),
        ClienteId = r.EnteroNulo("ClienteId"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoId = r.EnteroNulo("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        AreaId = r.Entero("AreaId"),
        AreaNombre = r.Texto("AreaNombre"),
        TipoDefectoId = r.Entero("TipoDefectoId"),
        TipoDefectoNombre = r.Texto("TipoDefectoNombre"),
        SeveridadId = r.Entero("SeveridadId"),
        SeveridadNombre = r.Texto("SeveridadNombre"),
        EstadoActualId = r.Entero("EstadoActualId"),
        EstadoNombre = r.Texto("EstadoNombre"),
        EstadoOrden = r.Entero("EstadoOrden"),
        EsFinal = r.Booleano("EsFinal"),
        Descripcion = r.Texto("Descripcion"),
        CausaRaiz = r.TextoNulo("CausaRaiz"),
        AccionCorrectiva = r.TextoNulo("AccionCorrectiva"),
        ResponsableId = r.EnteroNulo("ResponsableId"),
        ResponsableNombre = r.TextoNulo("ResponsableNombre"),
        RegistradaPorId = r.Entero("RegistradaPorId"),
        RegistradaPorNombre = r.Texto("RegistradaPorNombre"),
        FechaRegistro = r.Fecha("FechaRegistro"),
        DiasAbierta = r.Entero("DiasAbierta"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        TurnoNombre = r.Texto("TurnoNombre"),
        OperadorNombre = r.Texto("OperadorNombre")
    };

    public static MovimientoEstadoNc MovimientoEstado(DbDataReader r) => new()
    {
        HistorialId = r.Entero("HistorialId"),
        EstadoId = r.Entero("EstadoId"),
        EstadoNombre = r.Texto("EstadoNombre"),
        EsFinal = r.Booleano("EsFinal"),
        UsuarioId = r.Entero("UsuarioId"),
        UsuarioNombre = r.Texto("UsuarioNombre"),
        FechaCambio = r.Fecha("FechaCambio"),
        Observacion = r.TextoNulo("Observacion")
    };

    public static EvidenciaNc Evidencia(DbDataReader r) => new()
    {
        EvidenciaId = r.Entero("EvidenciaId"),
        NombreArchivo = r.Texto("NombreArchivo"),
        Ruta = r.TextoNulo("Ruta"),
        FechaCarga = r.Fecha("FechaCarga"),
        TamanoBytes = r.EnteroNulo("TamanoBytes"),
        TieneArchivo = r.Booleano("TieneArchivo")
    };

    public static ArchivoEvidencia ArchivoEvidencia(DbDataReader r) => new()
    {
        EvidenciaId = r.Entero("EvidenciaId"),
        NoConformidadId = r.Entero("NoConformidadId"),
        NoConformidadCodigo = r.Texto("NoConformidadCodigo"),
        NombreArchivo = r.Texto("NombreArchivo"),
        Ruta = r.TextoNulo("Ruta"),
        Contenido = r.Binario("Archivo"),
        FechaCarga = r.Fecha("FechaCarga"),
        TamanoBytes = r.EnteroNulo("TamanoBytes")
    };

    public static BobinaAfectada BobinaAfectada(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        IdBobi = r.Entero("IdBobi"),
        Peso = r.DecimalNulo("Peso"),
        Metros = r.DecimalNulo("Metros"),
        EsConforme = r.Booleano("EsConforme"),
        LoteId = r.EnteroNulo("LoteId"),
        CodigoLote = r.TextoNulo("CodigoLote")
    };

    public static AlertaDeNoConformidad AlertaDeNoConformidad(DbDataReader r) => new()
    {
        AlertaId = r.Entero("AlertaId"),
        RegistroId = r.EnteroNulo("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        BobinaId = r.EnteroNulo("BobinaId"),
        IdBobi = r.EnteroNulo("IdBobi"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        ValorRegistrado = r.DecimalNulo("ValorRegistrado"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        FechaDeteccion = r.Fecha("FechaDeteccion"),
        Atendida = r.Booleano("Atendida")
    };

    public static EstadoNcDisponible EstadoDisponible(DbDataReader r) => new()
    {
        EstadoId = r.Entero("EstadoId"),
        Nombre = r.Texto("Nombre"),
        Orden = r.Entero("Orden"),
        EsFinal = r.Booleano("EsFinal")
    };

    public static NoConformidadCreada NoConformidadCreada(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo")
    };

    /* ---- Catálogos del formulario de no conformidades ---- */

    public static SeveridadCatalogo Severidad(DbDataReader r) => new()
    {
        SeveridadId = r.Entero("SeveridadId"),
        Nombre = r.Texto("Nombre")
    };

    public static EstadoNcCatalogo EstadoNc(DbDataReader r) => new()
    {
        EstadoId = r.Entero("EstadoId"),
        Nombre = r.Texto("Nombre"),
        Orden = r.Entero("Orden"),
        EsFinal = r.Booleano("EsFinal")
    };

    public static TipoDefectoCatalogo TipoDefecto(DbDataReader r) => new()
    {
        TipoDefectoId = r.Entero("TipoDefectoId"),
        Nombre = r.Texto("Nombre"),
        AreaId = r.EnteroNulo("AreaId"),
        AreaNombre = r.TextoNulo("AreaNombre")
    };

    public static AreaCatalogo Area(DbDataReader r) => new()
    {
        AreaId = r.Entero("AreaId"),
        Nombre = r.Texto("Nombre")
    };

    public static ResponsableCatalogo Responsable(DbDataReader r) => new()
    {
        UsuarioId = r.Entero("UsuarioId"),
        Codigo = r.Texto("Codigo"),
        NombreCompleto = r.Texto("NombreCompleto"),
        RolNombre = r.Texto("RolNombre"),
        AreaId = r.EnteroNulo("AreaId")
    };
}
