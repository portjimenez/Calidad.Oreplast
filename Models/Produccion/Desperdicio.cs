using calidad_app.Models.Catalogos;

namespace calidad_app.Models.Produccion;

/// <summary>
/// Filtros del control de desperdicio. El rango de fechas es obligatorio (se
/// mide un periodo, no "todo"); el resto acota el ámbito y en null no filtra.
/// </summary>
public class FiltroDesperdicio
{
    public DateOnly FechaDesde { get; set; } =
        DateOnly.FromDateTime(DateTime.Today.AddDays(-30));

    public DateOnly FechaHasta { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public int? AreaId { get; set; }
    public int? LineaId { get; set; }
    public int? MaquinaId { get; set; }
    public int? TurnoId { get; set; }
    public int? ProductoId { get; set; }
    public int? OrdenId { get; set; }
}

/// <summary>
/// Totales del periodo y su comparación contra las metas.
///
/// El porcentaje se mide sobre el MATERIAL PROCESADO (lo producido más lo
/// desperdiciado), no sobre lo producido. Si se dividiera entre lo producido,
/// una corrida que desperdicia más de lo que saca daría más de 100 %, un
/// número que no significa nada y que no se puede comparar entre máquinas. Así
/// el indicador siempre va de 0 a 100 y se lee directo: "de cada 100 kg que
/// entraron a la máquina, 7 se perdieron".
/// </summary>
public class ResumenDesperdicio
{
    public DateOnly FechaDesde { get; set; }
    public DateOnly FechaHasta { get; set; }

    public int Registros { get; set; }
    public int Ordenes { get; set; }
    public int Maquinas { get; set; }
    public int Bobinas { get; set; }
    public int BobinasNoConformes { get; set; }

    public decimal KgProducidos { get; set; }
    public decimal KgDesperdicioSetup { get; set; }
    public decimal KgDesperdicioProduccion { get; set; }
    public decimal KgDesperdicio { get; set; }
    public decimal KgDuro { get; set; }
    public decimal KgRefill { get; set; }

    /// <summary>Lo producido más lo desperdiciado: el material que pasó por la máquina.</summary>
    public decimal KgProcesado { get; set; }

    public decimal? PorcentajeDesperdicio { get; set; }
    public decimal? PorcentajeDuro { get; set; }
    public decimal? PorcentajeRefill { get; set; }

    public int TiempoMuertoMin { get; set; }
    public decimal HorasSetup { get; set; }
    public decimal HorasProduccion { get; set; }

    /// <summary>Una fila por concepto de meta, con el valor real y si se cumple.</summary>
    public List<MetaComparada> Metas { get; set; } = [];

    public decimal HorasTiempoMuerto => Math.Round(TiempoMuertoMin / 60m, 2);
}

/// <summary>
/// Un concepto de meta medido contra lo que realmente pasó.
///
/// La meta que se aplica es la del ámbito consultado y se resuelve con la
/// regla del módulo 4: gana la más específica. La unidad decide contra qué se
/// compara (una meta en % contra el porcentaje, una en kg contra los kilos), y
/// <see cref="EsMaximo"/> en qué dirección se cumple: desperdicio, material
/// duro y refill son techos; producción es un piso.
/// </summary>
public class MetaComparada
{
    public string Concepto { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool EsMaximo { get; set; }

    public decimal? ValorReal { get; set; }

    public int? MetaId { get; set; }
    public decimal? ValorMeta { get; set; }
    public string? Unidad { get; set; }
    public DateOnly? MetaVigenteDesde { get; set; }

    public decimal? Desviacion { get; set; }

    /// <summary>Null cuando no hay meta definida para el ámbito: no se juzga lo que no se pactó.</summary>
    public bool? Cumple { get; set; }

    public bool TieneMeta => MetaId is not null;

    /// <summary>
    /// Nombre en español correcto. Los literales de los procedimientos van sin
    /// acentos (convención del proyecto para evitar problemas de codificación en
    /// SQL Server), así que el texto que ve el usuario se redacta aquí, igual
    /// que se hace con los mensajes de error. El nombre que devuelve la base
    /// queda de respaldo por si se agrega un concepto y todavía no está aquí.
    /// </summary>
    public string NombreVisible => Concepto switch
    {
        ConceptosMeta.Desperdicio => "Desperdicio",
        ConceptosMeta.MaterialDuro => "Material duro",
        ConceptosMeta.Refill => "Refill",
        ConceptosMeta.Produccion => "Producción",
        _ => Nombre
    };

    public string ValorRealTexto => ValorReal is null
        ? "—"
        : $"{ValorReal:0.##}{(Unidad == "%" ? " %" : " kg")}";

    public string ValorMetaTexto => ValorMeta is null
        ? "Sin meta definida"
        : $"{ValorMeta:0.##} {Unidad}";
}

/// <summary>
/// Una corrida (un registro de inspección) con su desperdicio y SU PROPIA
/// meta.
///
/// La meta se resuelve por registro, a partir de su línea, máquina y producto
/// y de la fecha en que se corrió: una línea puede tener meta de 6 % y otra de
/// 9 %, y una corrida de hace dos meses se juzga con la meta que estaba
/// vigente entonces, no con la que se cargó después.
/// </summary>
public class RegistroDesperdicio
{
    public int RegistroId { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;

    public int TurnoId { get; set; }
    public string TurnoNombre { get; set; } = string.Empty;

    public int MaquinaId { get; set; }
    public string MaquinaCodigo { get; set; } = string.Empty;
    public string MaquinaNombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int? LineaId { get; set; }
    public string? LineaCodigo { get; set; }
    public string? LineaNombre { get; set; }

    public string OperadorNombre { get; set; } = string.Empty;

    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;

    public int Bobinas { get; set; }
    public decimal KgProducidos { get; set; }
    public decimal KgDesperdicioSetup { get; set; }
    public decimal KgDesperdicioProduccion { get; set; }
    public decimal KgDesperdicio { get; set; }
    public decimal KgDuro { get; set; }
    public decimal KgRefill { get; set; }
    public decimal KgProcesado { get; set; }
    public decimal? PorcentajeDesperdicio { get; set; }

    public int TiempoMuertoMin { get; set; }
    public string? RazonSetup { get; set; }
    public string? RazonProduccion { get; set; }
    public decimal? HorasSetup { get; set; }
    public decimal? HorasProduccion { get; set; }

    public int? MetaId { get; set; }
    public decimal? ValorMeta { get; set; }
    public string? MetaUnidad { get; set; }
    public decimal? ValorComparado { get; set; }
    public bool? CumpleMeta { get; set; }

    public string Linea => LineaCodigo ?? "(sin línea)";
}

/// <summary>
/// El desperdicio del periodo sumado por una dimensión (línea, máquina, turno,
/// producto, orden o fecha). La forma es la misma para las seis, de modo que
/// la pantalla dibuja la misma tabla y la misma gráfica sea cual sea el corte.
///
/// El porcentaje se calcula sobre los totales del grupo y NO es el promedio de
/// los porcentajes de cada registro: promediarlos le daría el mismo peso a una
/// corrida de 20 kg que a una de 2000.
/// </summary>
public class GrupoDesperdicio
{
    /// <summary>Null al agrupar por fecha, que es la única dimensión sin identificador.</summary>
    public int? ClaveId { get; set; }

    public string ClaveTexto { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;

    public int Registros { get; set; }
    public int Bobinas { get; set; }
    public decimal KgProducidos { get; set; }
    public decimal KgDesperdicioSetup { get; set; }
    public decimal KgDesperdicioProduccion { get; set; }
    public decimal KgDesperdicio { get; set; }
    public decimal KgDuro { get; set; }
    public decimal KgRefill { get; set; }
    public decimal KgProcesado { get; set; }
    public decimal? PorcentajeDesperdicio { get; set; }
    public int TiempoMuertoMin { get; set; }
}

/// <summary>
/// Minutos de paro del periodo por razón, con su acumulado: un Pareto listo
/// para dibujar.
///
/// El paro de setup y el de producción se cuentan por separado además de
/// sumados porque no significan lo mismo: el de setup es tiempo de
/// preparación, previsible; el de producción es una interrupción de una
/// corrida que ya estaba andando.
/// </summary>
public class ParoPorRazon
{
    /// <summary>Null cuando el operador registró minutos sin elegir razón.</summary>
    public int? RazonId { get; set; }

    public string RazonNombre { get; set; } = string.Empty;

    public int Eventos { get; set; }
    public int EventosSetup { get; set; }
    public int EventosProduccion { get; set; }

    public int MinutosTotal { get; set; }
    public int MinutosSetup { get; set; }
    public int MinutosProduccion { get; set; }

    public int Registros { get; set; }
    public decimal HorasTotal { get; set; }
    public decimal? PorcentajeDelTotal { get; set; }

    /// <summary>
    /// Responde la pregunta del Pareto: cuántas razones hay que atacar para
    /// eliminar el 80 % de los minutos perdidos.
    /// </summary>
    public decimal? PorcentajeAcumulado { get; set; }

    public bool SinRazonIndicada => RazonId is null;
}

/// <summary>
/// Las seis dimensiones por las que se puede agrupar el desperdicio, tal como
/// las valida el procedimiento. Escritas una sola vez para que ningún
/// componente las repita como texto suelto.
/// </summary>
public static class AgrupacionDesperdicio
{
    public const string Linea = "Linea";
    public const string Maquina = "Maquina";
    public const string Turno = "Turno";
    public const string Producto = "Producto";
    public const string Orden = "Orden";
    public const string Fecha = "Fecha";

    public static readonly (string Clave, string Nombre)[] Opciones =
    [
        (Maquina, "Por máquina"),
        (Linea, "Por línea"),
        (Turno, "Por turno"),
        (Producto, "Por producto"),
        (Orden, "Por orden"),
        (Fecha, "Por fecha")
    ];
}
