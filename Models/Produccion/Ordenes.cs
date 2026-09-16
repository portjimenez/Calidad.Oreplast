namespace calidad_app.Models.Produccion;

/// <summary>
/// Filtros de la pantalla de órdenes. Todos opcionales: en null no filtran, de
/// modo que la misma consulta responde "las órdenes abiertas", "las de este
/// cliente" y "las que se crearon esta semana".
/// </summary>
public class FiltroOrdenes
{
    public string? Busqueda { get; set; }

    /// <summary>Abierta | EnProceso | Cerrada. Ver <see cref="EstadosOrden"/>.</summary>
    public string? Estado { get; set; }

    public int? ClienteId { get; set; }
    public int? ProductoId { get; set; }
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }

    /// <summary>Solo las órdenes que tienen registros de inspección sin cerrar.</summary>
    public bool SoloConPendientes { get; set; }
}

/// <summary>
/// Una orden en la tabla de seguimiento, con su avance ya calculado.
///
/// El avance no es una columna de la base: se calcula sumando el peso de las
/// bobinas confirmadas de todos los registros de la orden. Solo las
/// confirmadas, porque una bobina sin confirmar todavía puede corregirse o
/// borrarse y sumarla inflaría el avance con producto que aún no existe.
/// </summary>
public class OrdenResumen
{
    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public string Estado { get; set; } = EstadosOrden.Abierta;
    public DateTime FechaCreacion { get; set; }
    public decimal? KgProgramados { get; set; }

    public int ClienteId { get; set; }
    public string ClienteCodigo { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;

    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;

    public int Registros { get; set; }
    public int RegistrosAbiertos { get; set; }
    public DateOnly? PrimerRegistro { get; set; }
    public DateOnly? UltimoRegistro { get; set; }

    public decimal KgProducidos { get; set; }
    public int Bobinas { get; set; }
    public int BobinasNoConformes { get; set; }

    public int Lotes { get; set; }
    public int LotesSinLiberar { get; set; }
    public int NoConformidadesAbiertas { get; set; }

    /// <summary>
    /// Puede pasar de 100 %: la planta a veces produce de más para cubrir el
    /// desperdicio del cliente, y recortarlo daría una lectura falsa del
    /// cumplimiento. Null cuando la orden no trae kilos programados.
    /// </summary>
    public decimal? PorcentajeAvance { get; set; }

    public bool TieneRegistros { get; set; }

    /// <summary>
    /// La base solo acepta el cierre manual cuando no queda ningún registro
    /// abierto ni ningún lote sin liberar. Viene resuelto para que la pantalla
    /// oculte el botón en lugar de llevar al usuario a un error.
    /// </summary>
    public bool PuedeCerrarse { get; set; }

    public bool ProductoConFicha { get; set; }

    /// <summary>
    /// Producto y cliente solo se pueden cambiar mientras la orden no tenga
    /// registros: los parámetros medidos se comparan contra la ficha técnica
    /// del producto, y cambiarlo después dejaría las bobinas ya producidas
    /// atribuidas a un producto que nunca se corrió.
    /// </summary>
    public bool AdmiteCambioDeProducto => !TieneRegistros && Estado != EstadosOrden.Cerrada;

    public string Producto => $"{ProductoCodigo} - {ProductoNombre}";
}

/// <summary>Indicadores del encabezado de la pantalla de órdenes.</summary>
/// <remarks>
/// Se pide aparte de la lista y no se deriva de las filas devueltas porque la
/// lista viene recortada por su máximo: si el resumen saliera de ahí, diría
/// "12 órdenes abiertas" cuando en realidad hay 300 y solo se trajeron 200.
/// </remarks>
public class ResumenOrdenes
{
    public int Total { get; set; }
    public int Abiertas { get; set; }
    public int EnProceso { get; set; }
    public int Cerradas { get; set; }
    public decimal KgProgramados { get; set; }
    public decimal KgProducidos { get; set; }
    public int RegistrosAbiertos { get; set; }
    public int LotesSinLiberar { get; set; }
    public int OrdenesConNoConformidades { get; set; }
    public decimal? PorcentajeAvance { get; set; }
}

/// <summary>
/// Expediente completo de una orden: la cadena de trazabilidad hacia abajo
/// (orden → registros → bobinas → lotes → certificado) más sus no
/// conformidades. Es la vista desde la orden; el módulo 3 recorre la misma
/// cadena en sentido contrario, partiendo del lote.
/// </summary>
public class OrdenDetalle : OrdenResumen
{
    public string? Estructura { get; set; }
    public int? FichaId { get; set; }

    public decimal KgDesperdicio { get; set; }
    public decimal KgDuro { get; set; }
    public decimal KgRefill { get; set; }
    public int TiempoMuertoMin { get; set; }

    /// <summary>
    /// Los nombres llevan sufijo porque las propiedades heredadas
    /// <c>Registros</c>, <c>Lotes</c> y <c>Bobinas</c> son los CONTADORES que
    /// pinta la tabla maestra: aquí conviven con las listas del expediente.
    /// </summary>
    public List<RegistroDeOrden> RegistrosDeInspeccion { get; set; } = [];
    public List<LoteDeOrden> LotesDeLaOrden { get; set; } = [];
    public List<BobinaDeOrden> BobinasDeLaOrden { get; set; } = [];
    public List<NoConformidadDeOrden> NoConformidades { get; set; } = [];
}

public class RegistroDeOrden
{
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool Bloqueado { get; set; }
    public DateTime? FechaHoraInicio { get; set; }

    public int TurnoId { get; set; }
    public string TurnoNombre { get; set; } = string.Empty;

    public int MaquinaId { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public int? LineaId { get; set; }
    public string? LineaCodigo { get; set; }
    public string? LineaNombre { get; set; }

    public int OperadorId { get; set; }
    public string OperadorCodigo { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;

    public int Bobinas { get; set; }
    public decimal KgProducidos { get; set; }
    public decimal? KgDesperdicioSetup { get; set; }
    public decimal? KgDesperdicioProduccion { get; set; }

    public bool DespejeFirmado { get; set; }
    public DateTime? DespejeFecha { get; set; }
    public bool CierreFirmado { get; set; }
    public DateTime? CierreFecha { get; set; }
}

public class LoteDeOrden
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public DateOnly? FechaProduccion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }

    public int TotalBobinas { get; set; }
    public decimal PesoTotal { get; set; }
    public int BobinasNoConformes { get; set; }

    public int? CertificadoId { get; set; }
    public string? CertificadoCodigo { get; set; }
    public DateTime? CertificadoFecha { get; set; }
    public bool TieneCertificado { get; set; }
}

/// <summary>
/// El nivel más fino de la trazabilidad: de aquí sale qué bobina produjo cada
/// operador, en qué máquina y en qué fecha, y a qué lote se entregó.
/// </summary>
public class BobinaDeOrden
{
    public int BobinaId { get; set; }
    public int IdBobi { get; set; }
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;

    public int? LoteId { get; set; }
    public string? CodigoLote { get; set; }

    public decimal? Peso { get; set; }
    public decimal? Metros { get; set; }
    public bool Ok { get; set; }
    public bool Confirmada { get; set; }
    public bool Bloqueada { get; set; }
    public bool EsConforme { get; set; }

    public int? NoConformidadId { get; set; }
    public string? NoConformidadCodigo { get; set; }
}

public class NoConformidadDeOrden
{
    public int NoConformidadId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public int? RegistroId { get; set; }
    public string? IdRegistro { get; set; }
    public string TipoDefecto { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool EsFinal { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public string RegistradaPor { get; set; } = string.Empty;
    public string? Responsable { get; set; }
}

/// <summary>
/// Datos que la pantalla envía al guardar una orden. Se separa de
/// <see cref="OrdenResumen"/> porque lo que se escribe son cinco campos; el
/// resto de esa clase son cálculos que la base devuelve y que el formulario
/// nunca debe poder mandar de vuelta.
/// </summary>
public class OrdenEdicion
{
    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int ProductoId { get; set; }
    public decimal? KgProgramados { get; set; }

    public static OrdenEdicion Desde(OrdenResumen orden) => new()
    {
        OrdenId = orden.OrdenId,
        NumeroOP = orden.NumeroOP,
        ClienteId = orden.ClienteId,
        ProductoId = orden.ProductoId,
        KgProgramados = orden.KgProgramados
    };
}

/// <summary>
/// Los tres estados que admite la restricción CHECK de
/// <c>prod.OrdenProduccion</c>, escritos una sola vez para que ningún
/// componente los repita como texto suelto.
/// </summary>
public static class EstadosOrden
{
    public const string Abierta = "Abierta";
    public const string EnProceso = "EnProceso";
    public const string Cerrada = "Cerrada";
}
