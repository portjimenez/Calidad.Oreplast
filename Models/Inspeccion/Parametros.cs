namespace calidad_app.Models.Inspeccion;

/// <summary>
/// Un parámetro del formulario de inspección: la definición del catálogo, el
/// valor capturado (si lo hay) y los límites de la ficha técnica.
/// </summary>
public class ParametroMedicion
{
    public int ParametroId { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }
    public int Orden { get; set; }
    public int? RegParamId { get; set; }
    public int? BobinaId { get; set; }
    public decimal? ValorRegistrado { get; set; }
    /// <summary>true dentro, false fuera, null no evaluable (sin valor, sin ficha o sin límites).</summary>
    public bool? DentroDeRango { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public decimal? ValorObjetivo { get; set; }
    public decimal? LimiteInferior { get; set; }
    public decimal? LimiteSuperior { get; set; }
    public bool TieneTolerancia { get; set; }

    public bool Medido => ValorRegistrado is not null;
    public bool FueraDeRango => DentroDeRango == false;

    /// <summary>Texto del rango para mostrar junto al campo, por ejemplo "114.3 – 139.7 mic".</summary>
    public string RangoTexto => (LimiteInferior, LimiteSuperior) switch
    {
        (null, null) => "Sin límites definidos",
        (not null, null) => $"≥ {LimiteInferior:0.##} {Unidad}".TrimEnd(),
        (null, not null) => $"≤ {LimiteSuperior:0.##} {Unidad}".TrimEnd(),
        _ => $"{LimiteInferior:0.##} – {LimiteSuperior:0.##} {Unidad}".TrimEnd()
    };
}

/// <summary>Encabezado de la ficha técnica que aplica a un registro.</summary>
public class FichaAplicable
{
    public bool TieneFicha { get; set; }
    public int? FichaId { get; set; }
    public string? FichaVersion { get; set; }
    public DateOnly? FichaVigenteDesde { get; set; }
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public List<ParametroMedicion> Parametros { get; set; } = [];
}

/// <summary>Recuento de la evaluación contra la ficha técnica.</summary>
public class EvaluacionResumen
{
    public bool TieneFicha { get; set; }
    public int? FichaId { get; set; }
    public int ValoresEvaluados { get; set; }
    public int FueraDeRango { get; set; }
    public int DentroDeRango { get; set; }
    public int NoEvaluables { get; set; }
    public int AlertasGeneradas { get; set; }
    public List<DesviacionParametro> Desviaciones { get; set; } = [];

    public bool HayDesviaciones => FueraDeRango > 0;
}

/// <summary>Un valor que quedó fuera de la tolerancia de la ficha.</summary>
public class DesviacionParametro
{
    public int? RegParamId { get; set; }
    public int? BobinaId { get; set; }
    public int? IdBobi { get; set; }
    public int ParametroId { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }
    public decimal? ValorRegistrado { get; set; }
    public decimal? ValorObjetivo { get; set; }
    public decimal? LimiteInferior { get; set; }
    public decimal? LimiteSuperior { get; set; }
    /// <summary>"Alto" o "Bajo".</summary>
    public string Desviacion { get; set; } = string.Empty;
}

/// <summary>Valor que la pantalla envía para guardar.</summary>
public record ValorParametro(int ParametroId, decimal? Valor);

/// <summary>Nivel al que se captura una medición.</summary>
public static class AmbitoParametro
{
    public const string Registro = "Registro";
    public const string Bobina = "Bobina";
}
