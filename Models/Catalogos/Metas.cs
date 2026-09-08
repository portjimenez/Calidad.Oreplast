namespace calidad_app.Models.Catalogos;

/// <summary>
/// Meta de producción: el valor contra el que se compara lo que la planta
/// realmente gastó y produjo.
///
/// El ámbito es flexible. Una meta puede ser general (toda la planta) o
/// acotarse a una línea, a una máquina, a un producto o a una combinación.
/// Como pueden convivir varias que apliquen al mismo caso,
/// <see cref="Especificidad"/> dice cuántas dimensiones fija cada una: la
/// regla al medir es que gana la más específica.
///
/// Las metas no se corrigen, se suceden: al guardar una nueva para el mismo
/// concepto y ámbito hay que declarar que reemplaza a la vigente, y la
/// anterior queda inactiva pero se conserva. El desperdicio del mes pasado se
/// sigue midiendo contra la meta que estaba vigente entonces.
/// </summary>
public class MetaProduccion
{
    public int MetaId { get; set; }

    /// <summary>Desperdicio | MaterialDuro | Refill | Produccion.</summary>
    public string Concepto { get; set; } = ConceptosMeta.Desperdicio;

    public decimal ValorMeta { get; set; }

    /// <summary>kg, % o m, según el concepto.</summary>
    public string? Unidad { get; set; }

    public DateOnly VigenteDesde { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public bool Activo { get; set; } = true;

    public int? LineaId { get; set; }
    public string? LineaCodigo { get; set; }
    public string? LineaNombre { get; set; }

    public int? MaquinaId { get; set; }
    public string? MaquinaCodigo { get; set; }
    public string? MaquinaNombre { get; set; }

    public int? ProductoId { get; set; }
    public string? ProductoCodigo { get; set; }
    public string? ProductoNombre { get; set; }

    /// <summary>Cuántas dimensiones del ámbito fija la meta (0 a 3).</summary>
    public int Especificidad { get; set; }

    /// <summary>Su fecha de vigencia ya llegó.</summary>
    public bool Vigente { get; set; }

    /// <summary>Descripción del ámbito, ya resuelta para la tabla.</summary>
    public string Ambito => Especificidad == 0
        ? "Toda la planta"
        : string.Join(" · ", new[] { LineaNombre, MaquinaCodigo, ProductoCodigo }
            .Where(parte => !string.IsNullOrWhiteSpace(parte)));

    public string ValorConUnidad =>
        Unidad is null ? ValorMeta.ToString("0.###") : $"{ValorMeta:0.###} {Unidad}";
}

/// <summary>
/// Los cuatro conceptos que admite la restricción CHECK de
/// <c>cat.MetaProduccion</c>, escritos una sola vez para que ningún componente
/// los repita como texto suelto.
/// </summary>
public static class ConceptosMeta
{
    public const string Desperdicio = "Desperdicio";
    public const string MaterialDuro = "MaterialDuro";
    public const string Refill = "Refill";
    public const string Produccion = "Produccion";
}

/// <summary>
/// Los tres tipos que admite la restricción CHECK de <c>cat.ItemChecklist</c>.
/// </summary>
public static class TiposItemChecklist
{
    public const string DespejeLinea = "DespejeLinea";
    public const string CierreOrden = "CierreOrden";
    public const string CalidadBobina = "CalidadBobina";
}
