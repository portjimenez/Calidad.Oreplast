namespace calidad_app.Models.Reportes;

/// <summary>
/// Ámbito que se consulta en el tablero: un periodo obligatorio y, sobre él,
/// los cortes que el usuario quiera aplicar. Cada corte en null no filtra.
///
/// Es el mismo filtro para las cinco vistas del tablero (resumen, tendencia,
/// comparativo, calidad y causas) porque todas responden sobre el mismo
/// conjunto de corridas: si cada pantalla aceptara un ámbito distinto, dos
/// gráficas de la misma página podrían estar hablando de cosas diferentes.
/// </summary>
public class FiltroIndicadores
{
    public DateOnly FechaDesde { get; set; } =
        DateOnly.FromDateTime(DateTime.Today.AddDays(-29));

    public DateOnly FechaHasta { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public int? AreaId { get; set; }
    public int? LineaId { get; set; }
    public int? MaquinaId { get; set; }
    public int? TurnoId { get; set; }
    public int? ProductoId { get; set; }
    public int? ClienteId { get; set; }
    public int? OperadorId { get; set; }

    public int Dias => FechaHasta.DayNumber - FechaDesde.DayNumber + 1;

    /// <summary>
    /// Copia del filtro. La usa el centro de reportes para partir del ámbito
    /// que el usuario ya eligió en el tablero sin quedar atado a él.
    /// </summary>
    public FiltroIndicadores Copiar() => (FiltroIndicadores)MemberwiseClone();

    /// <summary>
    /// Descripción del ámbito en una línea, para el encabezado de los archivos
    /// exportados y para la bitácora. Los nombres los pone quien llama (la
    /// pantalla, que es la que tiene los catálogos cargados).
    /// </summary>
    public string DescribirPeriodo() =>
        $"{FechaDesde:dd/MM/yyyy} al {FechaHasta:dd/MM/yyyy}";
}

/// <summary>
/// Lo que ocurrió en el periodo, en crudo. Es el contexto del tablero: los
/// indicadores dicen cómo se comportó la planta, estos números dicen sobre
/// cuánto trabajo se midió.
/// </summary>
public class TotalesPeriodo
{
    public DateOnly FechaDesde { get; set; }
    public DateOnly FechaHasta { get; set; }

    /// <summary>Periodo anterior de igual longitud, contra el que se compara.</summary>
    public DateOnly FechaDesdeAnterior { get; set; }
    public DateOnly FechaHastaAnterior { get; set; }

    public int DiasDelPeriodo { get; set; }

    public int Registros { get; set; }
    public int RegistrosAnterior { get; set; }
    public int Ordenes { get; set; }
    public int Maquinas { get; set; }

    public int Bobinas { get; set; }
    public int BobinasConformes { get; set; }
    public int BobinasNoConformes { get; set; }

    public decimal KgProducidos { get; set; }
    public decimal KgDesperdicio { get; set; }
    public decimal KgDuro { get; set; }
    public decimal KgRefill { get; set; }

    /// <summary>Lo producido más lo desperdiciado: el material que pasó por la máquina.</summary>
    public decimal KgProcesado { get; set; }

    public decimal KgProgramados { get; set; }
    public decimal KgProducidosDeOrdenes { get; set; }

    public int TiempoMuertoMin { get; set; }

    public int Mediciones { get; set; }
    public int MedicionesEnRango { get; set; }
    public int MedicionesFueraRango { get; set; }

    public int Alertas { get; set; }
    public int AlertasCriticas { get; set; }
    public int AlertasPendientes { get; set; }

    public int NoConformidades { get; set; }
    public int NoConformidadesAbiertas { get; set; }
    public int NoConformidadesCerradas { get; set; }

    public int RegistrosLiberados { get; set; }
    public int Lotes { get; set; }
    public int LotesLiberados { get; set; }
    public int LotesCertificados { get; set; }

    public decimal HorasTiempoMuerto => Math.Round(TiempoMuertoMin / 60m, 1);

    public bool HayDatos => Registros > 0;
}

/// <summary>
/// Un indicador del tablero: su valor, el del periodo anterior de igual
/// longitud y la meta cuando existe.
///
/// La base devuelve una fila por indicador con esta misma forma y no una
/// columna por indicador. Así la pantalla dibuja las tarjetas recorriendo la
/// lista, y agregar un indicador es agregar una fila en el procedimiento y su
/// nombre en <see cref="TextosIndicadores"/>, sin tocar el componente.
/// </summary>
public class IndicadorKpi
{
    public string Clave { get; set; } = string.Empty;
    public int Orden { get; set; }

    /// <summary>kg, %, h, dias o conteo. Decide cómo se escribe el valor.</summary>
    public string Unidad { get; set; } = string.Empty;

    /// <summary>
    /// En qué dirección el indicador es bueno. Lo dice la base y no la
    /// pantalla, para que la flecha verde o roja se decida en un solo lugar:
    /// el desperdicio, el paro y las no conformidades mejoran cuando bajan; la
    /// producción y los cumplimientos, cuando suben.
    /// </summary>
    public bool MejorSiSube { get; set; }

    public decimal? Valor { get; set; }
    public decimal? ValorAnterior { get; set; }
    public decimal? Variacion { get; set; }
    public decimal? VariacionPorcentual { get; set; }

    public int? MetaId { get; set; }
    public decimal? ValorMeta { get; set; }
    public string? MetaUnidad { get; set; }

    /// <summary>Null cuando no hay meta pactada para el ámbito: no se juzga lo que no se acordó.</summary>
    public bool? CumpleMeta { get; set; }

    public bool TieneMeta => MetaId is not null;

    public string NombreVisible => TextosIndicadores.Nombre(Clave);

    public string Explicacion => TextosIndicadores.Explicacion(Clave);

    public string ValorTexto => Formatear(Valor);

    public string ValorAnteriorTexto => Formatear(ValorAnterior);

    public string MetaTexto => ValorMeta is null
        ? "Sin meta definida"
        : $"{(MejorSiSube ? "Mínimo" : "Máximo")} {ValorMeta:0.##} {MetaUnidad}";

    /// <summary>
    /// Cómo le fue al indicador contra el periodo anterior. No es lo mismo que
    /// el signo de la variación: que el desperdicio suba es empeorar y que baje
    /// es mejorar, así que la lectura depende de <see cref="MejorSiSube"/>.
    /// </summary>
    public TendenciaIndicador Tendencia
    {
        get
        {
            if (Variacion is not { } variacion || variacion == 0)
            {
                return TendenciaIndicador.SinCambio;
            }

            var sube = variacion > 0;
            return sube == MejorSiSube ? TendenciaIndicador.Mejora : TendenciaIndicador.Empeora;
        }
    }

    /// <summary>Variación en el texto que se pone bajo la tarjeta.</summary>
    public string VariacionTexto
    {
        get
        {
            if (ValorAnterior is null)
            {
                return "Sin dato del periodo anterior";
            }

            if (Variacion is not { } variacion || variacion == 0)
            {
                return "Igual que el periodo anterior";
            }

            var signo = variacion > 0 ? "+" : "−";
            var magnitud = Math.Abs(variacion);
            var porcentual = VariacionPorcentual is { } pct
                ? $" ({signo}{Math.Abs(pct):0.#} %)"
                : string.Empty;

            // La diferencia entre dos porcentajes se dice en PUNTOS, no en por
            // ciento: pasar de 5 % a 7 % es subir dos puntos (y un 40 %), y
            // llamarle "2 %" a eso es el error más común al leer un tablero.
            var cambio = Unidad switch
            {
                "%" => $"{signo}{magnitud:0.##} puntos",
                "kg" => $"{signo}{magnitud:N0} kg",
                "h" => $"{signo}{magnitud:0.#} h",
                "dias" => $"{signo}{magnitud:0.#} días",
                _ => $"{signo}{magnitud:N0}"
            };

            return cambio + porcentual;
        }
    }

    private string Formatear(decimal? valor) => valor switch
    {
        null => "—",
        _ when Unidad == "%" => $"{valor:0.#} %",
        _ when Unidad == "kg" => $"{valor:N0} kg",
        _ when Unidad == "h" => $"{valor:0.#} h",
        _ when Unidad == "dias" => $"{valor:0.#} días",
        _ => $"{valor:N0}"
    };
}

/// <summary>Cómo se lee la variación contra el periodo anterior.</summary>
public enum TendenciaIndicador
{
    SinCambio,
    Mejora,
    Empeora
}

/// <summary>El tablero completo: el contexto del periodo y sus indicadores.</summary>
public class TableroIndicadores
{
    public TotalesPeriodo Totales { get; set; } = new();
    public List<IndicadorKpi> Indicadores { get; set; } = [];

    public IndicadorKpi? Indicador(string clave) =>
        Indicadores.FirstOrDefault(i => i.Clave == clave);
}

/// <summary>
/// Claves de los indicadores, tal como las devuelve cal.usp_Kpi_Resumen.
/// Escritas una sola vez para que ninguna pantalla las repita como texto
/// suelto.
/// </summary>
public static class ClavesIndicador
{
    public const string KgProducidos = "KgProducidos";
    public const string PctDesperdicio = "PctDesperdicio";
    public const string PctFichaTecnica = "PctFichaTecnica";
    public const string PctBobinasConformes = "PctBobinasConformes";
    public const string PctPrograma = "PctPrograma";
    public const string Alertas = "Alertas";
    public const string HorasAtencionAlerta = "HorasAtencionAlerta";
    public const string NoConformidades = "NoConformidades";
    public const string DiasCierreNc = "DiasCierreNc";
    public const string HorasParo = "HorasParo";
    public const string PctCertificados = "PctCertificados";
}

/// <summary>
/// Nombre y explicación de cada indicador, en español y con tildes.
///
/// Los literales de los procedimientos van sin acentos (convención del
/// proyecto para evitar problemas de codificación en SQL Server), así que la
/// base manda la clave y el texto que ve el usuario se redacta aquí, igual que
/// se hace con los mensajes de error. La explicación no es decorativa: es la
/// definición del indicador y es lo que hay que poder defender cuando alguien
/// pregunte de dónde sale el número.
/// </summary>
public static class TextosIndicadores
{
    private static readonly Dictionary<string, (string Nombre, string Explicacion)> Textos = new()
    {
        [ClavesIndicador.KgProducidos] = (
            "Producción",
            "Kilos de bobinas confirmadas en el periodo. Una bobina sin confirmar todavía puede corregirse, así que no se cuenta."),

        [ClavesIndicador.PctDesperdicio] = (
            "Desperdicio",
            "Kilos desperdiciados sobre el material procesado (lo producido más lo desperdiciado). Medido así, el indicador siempre va de 0 a 100 y se puede comparar entre máquinas."),

        [ClavesIndicador.PctFichaTecnica] = (
            "Cumplimiento de ficha",
            "Mediciones de parámetros que quedaron dentro de la tolerancia de la ficha técnica vigente. Es el indicador propio del sistema: mide si el proceso se está corriendo dentro de lo aprobado."),

        [ClavesIndicador.PctBobinasConformes] = (
            "Bobinas conformes",
            "Bobinas confirmadas que no se marcaron como no conformes."),

        [ClavesIndicador.PctPrograma] = (
            "Cumplimiento del programa",
            "Kilos producidos frente a los programados en las órdenes que se trabajaron en el periodo. Se toma la producción completa de esas órdenes, porque lo programado también es de la orden completa."),

        [ClavesIndicador.Alertas] = (
            "Alertas generadas",
            "Desviaciones detectadas contra la ficha técnica durante las corridas del periodo."),

        [ClavesIndicador.HorasAtencionAlerta] = (
            "Atención de alertas",
            "Horas promedio entre que se detecta una desviación y que Calidad la atiende. Solo cuentan las ya atendidas: las pendientes aún no tienen tiempo de atención."),

        [ClavesIndicador.NoConformidades] = (
            "No conformidades",
            "No conformidades levantadas sobre las corridas del periodo."),

        [ClavesIndicador.DiasCierreNc] = (
            "Cierre de no conformidades",
            "Días promedio entre que se registra una no conformidad y que pasa a un estado final."),

        [ClavesIndicador.HorasParo] = (
            "Tiempo muerto",
            "Horas de paro registradas en setup y en producción."),

        [ClavesIndicador.PctCertificados] = (
            "Lotes certificados",
            "De los lotes liberados que salieron de estas corridas, cuántos ya tienen su certificado de calidad emitido.")
    };

    public static string Nombre(string clave) =>
        Textos.TryGetValue(clave, out var texto) ? texto.Nombre : clave;

    public static string Explicacion(string clave) =>
        Textos.TryGetValue(clave, out var texto) ? texto.Explicacion : string.Empty;
}
