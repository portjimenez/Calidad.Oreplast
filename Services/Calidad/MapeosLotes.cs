using System.Data.Common;
using calidad_app.Data.Sp;
using calidad_app.Models.Calidad;

namespace calidad_app.Services.Calidad;

/// <summary>
/// Mapeos de lotes, liberación y certificados.
///
/// Van en un archivo aparte de <see cref="MapeosCalidad"/> por tamaño: son tres
/// bloques con sus propias tablas y un solo archivo con todo el módulo sería
/// incómodo de leer. El criterio del mapeo es el mismo: explícito, columna por
/// columna.
/// </summary>
internal static class MapeosLotes
{
    /* ---- Lotes ---- */

    public static LoteResumen LoteResumen(DbDataReader r) => new()
    {
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
        OrdenId = r.Entero("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        EstadoOrden = r.Texto("EstadoOrden"),
        ClienteId = r.Entero("ClienteId"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoId = r.Entero("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        Estado = r.Texto("Estado"),
        FechaCreacion = r.Fecha("FechaCreacion"),
        TotalBobinas = r.Entero("TotalBobinas"),
        PesoTotal = r.DecimalNulo("PesoTotal"),
        BobinasNoConformes = r.Entero("BobinasNoConformes"),
        NoConformidadesAbiertas = r.Entero("NoConformidadesAbiertas"),
        CertificadoId = r.EnteroNulo("CertificadoId"),
        CertificadoCodigo = r.TextoNulo("CertificadoCodigo"),
        CertificadoFecha = r.FechaNula("CertificadoFecha"),
        TieneCertificado = r.Booleano("TieneCertificado")
    };

    public static LoteDetalle LoteDetalle(DbDataReader r) => new()
    {
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
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
        Estructura = r.TextoNulo("Estructura"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        Estado = r.Texto("Estado"),
        FechaCreacion = r.Fecha("FechaCreacion"),
        TotalBobinas = r.Entero("TotalBobinas"),
        PesoTotal = r.DecimalNulo("PesoTotal"),
        MetrosTotal = r.DecimalNulo("MetrosTotal"),
        BobinasNoConformes = r.Entero("BobinasNoConformes"),
        CertificadoId = r.EnteroNulo("CertificadoId"),
        CertificadoCodigo = r.TextoNulo("CertificadoCodigo"),
        CertificadoFecha = r.FechaNula("CertificadoFecha"),
        TieneCertificado = r.Booleano("TieneCertificado")
    };

    public static BobinaDeLote BobinaDeLote(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        IdBobi = r.Entero("IdBobi"),
        Peso = r.DecimalNulo("Peso"),
        Metros = r.DecimalNulo("Metros"),
        Ancho = r.DecimalNulo("Ancho"),
        Fuelle = r.DecimalNulo("Fuelle"),
        Calibre = r.DecimalNulo("Calibre"),
        Ok = r.Booleano("Ok"),
        Confirmada = r.Booleano("Confirmada"),
        FechaConfirmacion = r.FechaNula("FechaConfirmacion"),
        EsConforme = r.Booleano("EsConforme"),
        NoConformidadId = r.EnteroNulo("NoConformidadId"),
        NoConformidadCodigo = r.TextoNulo("NoConformidadCodigo"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        TurnoNombre = r.Texto("TurnoNombre"),
        OperadorNombre = r.Texto("OperadorNombre")
    };

    public static BobinaDisponible BobinaDisponible(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        IdBobi = r.Entero("IdBobi"),
        Peso = r.DecimalNulo("Peso"),
        Ok = r.Booleano("Ok"),
        EsConforme = r.Booleano("EsConforme"),
        FechaConfirmacion = r.FechaNula("FechaConfirmacion")
    };

    public static RegistroDeLote RegistroDeLote(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        Fecha = r.SoloFecha("Fecha"),
        Estado = r.Texto("Estado"),
        Bloqueado = r.Booleano("Bloqueado"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        MaquinaNombre = r.Texto("MaquinaNombre"),
        TurnoNombre = r.Texto("TurnoNombre"),
        OperadorNombre = r.Texto("OperadorNombre"),
        LiberacionId = r.EnteroNulo("LiberacionId"),
        FechaLiberacion = r.FechaNula("FechaLiberacion"),
        LiberadoPorNombre = r.TextoNulo("LiberadoPorNombre")
    };

    public static NoConformidadDeLote NoConformidadDeLote(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo"),
        Descripcion = r.Texto("Descripcion"),
        SeveridadNombre = r.Texto("SeveridadNombre"),
        EstadoNombre = r.Texto("EstadoNombre"),
        EsFinal = r.Booleano("EsFinal"),
        FechaRegistro = r.Fecha("FechaRegistro")
    };

    public static LoteCreado LoteCreado(DbDataReader r) => new()
    {
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote")
    };

    /* ---- Liberación ---- */

    public static EstadoLiberacion EstadoLiberacion(DbDataReader r) => new()
    {
        RegistroId = r.Entero("RegistroId"),
        IdRegistro = r.Texto("IdRegistro"),
        Estado = r.Texto("Estado"),
        Bloqueado = r.Booleano("Bloqueado"),
        Fecha = r.SoloFecha("Fecha"),
        OrdenId = r.Entero("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        EstadoOrden = r.Texto("EstadoOrden"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        MaquinaCodigo = r.Texto("MaquinaCodigo"),
        TurnoNombre = r.Texto("TurnoNombre"),
        OperadorNombre = r.Texto("OperadorNombre"),
        LoteId = r.EnteroNulo("LoteId"),
        CodigoLote = r.TextoNulo("CodigoLote"),
        EstadoLote = r.TextoNulo("EstadoLote"),
        TotalBobinas = r.Entero("TotalBobinas"),
        PesoTotal = r.DecimalNulo("PesoTotal"),
        BobinasNoConformes = r.Entero("BobinasNoConformes")
    };

    public static FirmaLiberacion Firma(DbDataReader r) => new()
    {
        Tipo = r.Texto("Tipo"),
        LiberacionId = r.EnteroNulo("LiberacionId"),
        Firmada = r.Booleano("Firmada"),
        CalidadVerificada = r.BooleanoNulo("CalidadVerificada"),
        InocuidadVerificada = r.BooleanoNulo("InocuidadVerificada"),
        LiberadoPorId = r.EnteroNulo("LiberadoPorId"),
        CodigoQuienLibera = r.TextoNulo("CodigoQuienLibera"),
        LiberadoPorNombre = r.TextoNulo("LiberadoPorNombre"),
        FechaLiberacion = r.FechaNula("FechaLiberacion"),
        LoteId = r.EnteroNulo("LoteId"),
        CodigoLote = r.TextoNulo("CodigoLote"),
        TieneFirma = r.Booleano("TieneFirma")
    };

    public static CierreOrden CierreOrden(DbDataReader r) => new()
    {
        CierreId = r.Entero("CierreId"),
        RegistroId = r.Entero("RegistroId"),
        Comentarios = r.TextoNulo("Comentarios"),
        KgProductoNoConforme = r.DecimalNulo("KgProductoNoConforme"),
        RazonNoConforme = r.TextoNulo("RazonNoConforme")
    };

    public static ResultadoLiberacion ResultadoLiberacion(DbDataReader r) => new()
    {
        LiberacionId = r.Entero("LiberacionId"),
        Tipo = r.Texto("Tipo"),
        LoteId = r.EnteroNulo("LoteId"),
        OrdenCerrada = r.Booleano("OrdenCerrada"),
        AdvertenciasAlLiberar = r.Entero("AdvertenciasAlLiberar")
    };

    /* ---- Certificados ---- */

    public static CertificadoResumen CertificadoResumen(DbDataReader r) => new()
    {
        CertificadoId = r.Entero("CertificadoId"),
        CertificadoCodigo = r.Texto("CertificadoCodigo"),
        FechaEmision = r.Fecha("FechaEmision"),
        Observaciones = r.TextoNulo("Observaciones"),
        EmitidoPorId = r.EnteroNulo("EmitidoPorId"),
        EmitidoPorNombre = r.TextoNulo("EmitidoPorNombre"),
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        OrdenId = r.Entero("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        ClienteId = r.Entero("ClienteId"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoId = r.Entero("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        TotalBobinas = r.Entero("TotalBobinas"),
        PesoConforme = r.DecimalNulo("PesoConforme")
    };

    public static CertificadoDetalle CertificadoDetalle(DbDataReader r) => new()
    {
        CertificadoId = r.Entero("CertificadoId"),
        CertificadoCodigo = r.Texto("CertificadoCodigo"),
        FechaEmision = r.Fecha("FechaEmision"),
        Observaciones = r.TextoNulo("Observaciones"),
        EmitidoPorId = r.EnteroNulo("EmitidoPorId"),
        EmitidoPorNombre = r.TextoNulo("EmitidoPorNombre"),
        EmitidoPorCodigo = r.TextoNulo("EmitidoPorCodigo"),
        LoteId = r.Entero("LoteId"),
        CodigoLote = r.Texto("CodigoLote"),
        FechaProduccion = r.SoloFechaNula("FechaProduccion"),
        EstadoLote = r.Texto("EstadoLote"),
        OrdenId = r.Entero("OrdenId"),
        NumeroOP = r.Texto("NumeroOP"),
        KgProgramados = r.DecimalNulo("KgProgramados"),
        ClienteId = r.Entero("ClienteId"),
        ClienteCodigo = r.Texto("ClienteCodigo"),
        ClienteNombre = r.Texto("ClienteNombre"),
        ProductoId = r.Entero("ProductoId"),
        ProductoCodigo = r.Texto("ProductoCodigo"),
        ProductoNombre = r.Texto("ProductoNombre"),
        Estructura = r.TextoNulo("Estructura"),
        FichaId = r.EnteroNulo("FichaId"),
        FichaVersion = r.TextoNulo("FichaVersion"),
        FichaVigenteDesde = r.SoloFechaNula("FichaVigenteDesde"),
        TotalBobinas = r.Entero("TotalBobinas"),
        BobinasConformes = r.Entero("BobinasConformes"),
        BobinasSegregadas = r.Entero("BobinasSegregadas"),
        PesoTotal = r.DecimalNulo("PesoTotal"),
        PesoConforme = r.DecimalNulo("PesoConforme"),
        MetrosTotal = r.DecimalNulo("MetrosTotal")
    };

    public static ResultadoParametro ResultadoParametro(DbDataReader r) => new()
    {
        ParametroId = r.Entero("ParametroId"),
        ParametroCodigo = r.Texto("ParametroCodigo"),
        ParametroNombre = r.Texto("ParametroNombre"),
        Unidad = r.TextoNulo("Unidad"),
        EsCritico = r.Booleano("EsCritico"),
        ValorObjetivo = r.DecimalNulo("ValorObjetivo"),
        LimiteInferior = r.DecimalNulo("LimiteInferior"),
        LimiteSuperior = r.DecimalNulo("LimiteSuperior"),
        Mediciones = r.Entero("Mediciones"),
        ValorMinimo = r.DecimalNulo("ValorMinimo"),
        ValorMaximo = r.DecimalNulo("ValorMaximo"),
        ValorPromedio = r.DecimalNulo("ValorPromedio"),
        FueraDeRango = r.Entero("FueraDeRango"),
        Cumple = r.BooleanoNulo("Cumple")
    };

    public static BobinaCertificada BobinaCertificada(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        IdBobi = r.Entero("IdBobi"),
        IdRegistro = r.Texto("IdRegistro"),
        Peso = r.DecimalNulo("Peso"),
        Metros = r.DecimalNulo("Metros"),
        Ancho = r.DecimalNulo("Ancho"),
        Fuelle = r.DecimalNulo("Fuelle"),
        Calibre = r.DecimalNulo("Calibre"),
        FechaConfirmacion = r.FechaNula("FechaConfirmacion")
    };

    public static BobinaSegregada BobinaSegregada(DbDataReader r) => new()
    {
        BobinaId = r.Entero("BobinaId"),
        IdBobi = r.Entero("IdBobi"),
        IdRegistro = r.Texto("IdRegistro"),
        Peso = r.DecimalNulo("Peso"),
        Metros = r.DecimalNulo("Metros"),
        NoConformidadCodigo = r.TextoNulo("NoConformidadCodigo"),
        NoConformidadDescripcion = r.TextoNulo("NoConformidadDescripcion"),
        NoConformidadEstado = r.TextoNulo("NoConformidadEstado")
    };

    public static NoConformidadCertificada NoConformidadCertificada(DbDataReader r) => new()
    {
        NoConformidadId = r.Entero("NoConformidadId"),
        Codigo = r.Texto("Codigo"),
        Descripcion = r.Texto("Descripcion"),
        CausaRaiz = r.TextoNulo("CausaRaiz"),
        AccionCorrectiva = r.TextoNulo("AccionCorrectiva"),
        SeveridadNombre = r.Texto("SeveridadNombre"),
        EstadoNombre = r.Texto("EstadoNombre"),
        FechaRegistro = r.Fecha("FechaRegistro")
    };

    public static CertificadoEmitido CertificadoEmitido(DbDataReader r) => new()
    {
        CertificadoId = r.Entero("CertificadoId"),
        Codigo = r.Texto("Codigo"),
        LoteId = r.Entero("LoteId")
    };
}
