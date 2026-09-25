using System.Globalization;

namespace calidad_app.Models.Reportes;

/// <summary>
/// Un punto de la serie de tiempo: un día, una semana o un mes del periodo.
///
/// La tendencia devuelve TODOS los periodos del rango, incluidos aquellos en
/// que la planta no corrió. Esos llegan con las medidas en cero y los
/// porcentajes en null, y así deben dibujarse: un hueco en la línea, no un
/// punto en cero que se leería como "ese día no hubo desperdicio".
/// </summary>
public class PuntoTendencia
{
    /// <summary>Fecha de inicio del periodo en formato ISO: identifica y ordena.</summary>
    public string Clave { get; set; } = string.Empty;

    public DateOnly Inicio { get; set; }
    public DateOnly Fin { get; set; }

    public int Registros { get; set; }
    public int Ordenes { get; set; }
    public int Maquinas { get; set; }
    public int Bobinas { get; set; }
    public int BobinasConformes { get; set; }

    public decimal KgProducidos { get; set; }
    public decimal KgDesperdicio { get; set; }
    public decimal KgProcesado { get; set; }

    public int Mediciones { get; set; }
    public int MedicionesEnRango { get; set; }

    public int Alertas { get; set; }
    public int AlertasCriticas { get; set; }
    public int NoConformidades { get; set; }
    public int TiempoMuertoMin { get; set; }

    public decimal? PorcentajeDesperdicio { get; set; }
    public decimal? PorcentajeConformes { get; set; }
    public decimal? PorcentajeFichaTecnica { get; set; }

    public bool SinProduccion => Registros == 0;

    /// <summary>
    /// Texto del punto en el eje. Se arma aquí y no en la base porque lleva
    /// tildes y nombres de mes en español, que los literales SQL del proyecto
    /// no pueden llevar.
    /// </summary>
    public string Etiqueta(string granularidad) => granularidad switch
    {
        GranularidadTendencia.Mes =>
            CultureInfo.GetCultureInfo("es-GT").TextInfo.ToTitleCase(
                Inicio.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("es-GT"))),
        GranularidadTendencia.Semana => $"Semana del {Inicio:dd/MM}",
        _ => Inicio.ToString("dd/MM")
    };
}

/// <summary>
/// Las tres escalas de la tendencia, tal como las valida el procedimiento.
/// El jefe de producción mira el día; el gerente, la semana o el mes.
/// </summary>
public static class GranularidadTendencia
{
    public const string Dia = "Dia";
    public const string Semana = "Semana";
    public const string Mes = "Mes";

    public static readonly (string Clave, string Nombre)[] Opciones =
    [
        (Dia, "Por día"),
        (Semana, "Por semana"),
        (Mes, "Por mes")
    ];
}

/// <summary>
/// Los indicadores del periodo sumados por una dimensión (área, línea,
/// máquina, turno, producto, cliente u operador).
///
/// La forma es la misma para las siete, de modo que la pantalla dibuja la
/// misma tabla sea cual sea el corte. Los porcentajes vienen calculados sobre
/// los totales del grupo y no son el promedio de los porcentajes de cada
/// corrida.
/// </summary>
public class GrupoIndicador
{
    /// <summary>Null cuando el grupo es "sin línea asignada".</summary>
    public int? ClaveId { get; set; }

    public string ClaveTexto { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;

    public int Registros { get; set; }
    public int Ordenes { get; set; }
    public int Bobinas { get; set; }
    public int BobinasConformes { get; set; }

    public decimal KgProducidos { get; set; }
    public decimal KgDesperdicio { get; set; }
    public decimal KgProcesado { get; set; }

    public int Mediciones { get; set; }
    public int MedicionesEnRango { get; set; }

    public int Alertas { get; set; }
    public int AlertasCriticas { get; set; }
    public int NoConformidades { get; set; }
    public int TiempoMuertoMin { get; set; }

    public decimal? PorcentajeDesperdicio { get; set; }
    public decimal? PorcentajeConformes { get; set; }
    public decimal? PorcentajeFichaTecnica { get; set; }
}

/// <summary>Las siete dimensiones por las que se puede comparar el periodo.</summary>
public static class AgrupacionIndicadores
{
    public const string Area = "Area";
    public const string Linea = "Linea";
    public const string Maquina = "Maquina";
    public const string Turno = "Turno";
    public const string Producto = "Producto";
    public const string Cliente = "Cliente";
    public const string Operador = "Operador";

    public static readonly (string Clave, string Nombre)[] Opciones =
    [
        (Maquina, "Por máquina"),
        (Linea, "Por línea"),
        (Area, "Por área"),
        (Turno, "Por turno"),
        (Producto, "Por producto"),
        (Cliente, "Por cliente"),
        (Operador, "Por operador")
    ];

    public static string Nombre(string clave) =>
        Opciones.FirstOrDefault(o => o.Clave == clave).Nombre ?? "Grupo";
}

/// <summary>
/// Una causa del análisis de Pareto.
///
/// <see cref="Eventos"/> y <see cref="Valor"/> no siempre coinciden: diez
/// paros de dos minutos no pesan lo mismo que uno de veinte, y el Pareto de
/// paros se ordena por minutos, no por cantidad de paros.
/// </summary>
public class CausaPareto
{
    public int? ClaveId { get; set; }
    public string Etiqueta { get; set; } = string.Empty;

    /// <summary>Contexto de la causa: el área del defecto, la unidad del parámetro.</summary>
    public string? Detalle { get; set; }

    public int Eventos { get; set; }

    /// <summary>El peso con el que la causa entra al Pareto.</summary>
    public decimal Valor { get; set; }

    /// <summary>Qué mide <see cref="Valor"/>: NC, mediciones o minutos.</summary>
    public string Unidad { get; set; } = string.Empty;

    public decimal? Porcentaje { get; set; }

    /// <summary>
    /// Responde la pregunta del Pareto: cuántas causas hay que atacar para
    /// eliminar el 80 % del problema.
    /// </summary>
    public decimal? PorcentajeAcumulado { get; set; }

    public string ValorTexto => Unidad switch
    {
        "min" => $"{Valor:0} min",
        "NC" => $"{Valor:0} NC",
        _ => $"{Valor:0}"
    };
}

/// <summary>
/// Los tres orígenes del análisis de causas: las tres preguntas que se hacen
/// al ver un indicador malo.
/// </summary>
public static class OrigenPareto
{
    public const string Defecto = "Defecto";
    public const string Parametro = "Parametro";
    public const string Paro = "Paro";

    public static readonly (string Clave, string Nombre, string Pregunta)[] Opciones =
    [
        (Parametro, "Parámetros fuera de ficha", "¿Qué variable se sale de la ficha técnica?"),
        (Defecto, "Tipos de defecto", "¿Por qué se levantan no conformidades?"),
        (Paro, "Razones de paro", "¿Por qué se detiene la máquina?")
    ];

    public static string Pregunta(string clave) =>
        Opciones.FirstOrDefault(o => o.Clave == clave).Pregunta ?? string.Empty;
}
