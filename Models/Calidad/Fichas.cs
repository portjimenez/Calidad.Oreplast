namespace calidad_app.Models.Calidad;

/// <summary>
/// Estados de una versión de ficha técnica. No se guardan: la base los deduce
/// de <c>Activa</c> y <c>VigenteDesde</c> (ver cal.ufn_EstadoFichas).
/// </summary>
public static class EstadosFicha
{
    /// <summary>En preparación: ninguna inspección la lee. Es la única editable.</summary>
    public const string Borrador = "Borrador";

    /// <summary>Publicada con fecha de vigencia futura.</summary>
    public const string Programada = "Programada";

    /// <summary>La que toma hoy un registro de inspección nuevo.</summary>
    public const string Vigente = "Vigente";

    /// <summary>Sustituida por otra más nueva; sus registros la siguen leyendo.</summary>
    public const string Reemplazada = "Reemplazada";
}

/// <summary>Filtros del maestro de fichas técnicas.</summary>
public class FiltroFichas
{
    public string? Busqueda { get; set; }

    /// <summary>
    /// Null = todos; "ConFicha", "SinFicha", "Urgentes" (sin ficha y con órdenes
    /// abiertas) o "ConBorrador".
    /// </summary>
    public string? Estado { get; set; }
}

/// <summary>
/// Un producto en el maestro, con el estado de su ficha. La fila es el
/// PRODUCTO y no la ficha: la pregunta de la pantalla es qué productos tienen
/// ficha y cuáles no, y uno sin ficha no tiene fila en cal.FichaTecnica.
/// </summary>
public class ProductoFicha
{
    public int ProductoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Estructura { get; set; }
    public bool Activo { get; set; }
    public string? ClienteNombre { get; set; }

    public int? FichaVigenteId { get; set; }
    public string? VersionVigente { get; set; }
    public DateOnly? VigenteDesde { get; set; }

    public int? BorradorId { get; set; }
    public string? VersionBorrador { get; set; }

    public string? VersionProgramada { get; set; }
    public DateOnly? ProgramadaDesde { get; set; }

    public int Versiones { get; set; }
    public int OrdenesAbiertas { get; set; }

    /// <summary>Filas que cumplen el filtro, aunque la lista venga recortada.</summary>
    public int TotalFiltrado { get; set; }

    public bool TieneFicha => FichaVigenteId is not null;

    /// <summary>
    /// Sin ficha y con órdenes abiertas: sus mediciones se están capturando como
    /// "no evaluables" y no levantan alertas.
    /// </summary>
    public bool EsUrgente => !TieneFicha && OrdenesAbiertas > 0;
}

/// <summary>
/// Detalle de la ficha técnica de un producto: el producto, todas sus
/// versiones y la tabla de tolerancias de la versión elegida.
/// </summary>
public class FichaProducto
{
    public int ProductoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Estructura { get; set; }
    public bool Activo { get; set; }
    public string? ClienteNombre { get; set; }

    public int OrdenesAbiertas { get; set; }
    public int Registros { get; set; }
    public DateOnly? UltimoRegistro { get; set; }

    /// <summary>
    /// Primera fecha desde la que puede entrar en vigor una versión nueva: hoy,
    /// el día siguiente a la última versión publicada o el día siguiente al
    /// último registro capturado, la mayor de las tres.
    /// </summary>
    public DateOnly FechaMinimaVigencia { get; set; }

    /// <summary>Versión cuyas tolerancias vienen en <see cref="Tolerancias"/>.</summary>
    public int? FichaSeleccionadaId { get; set; }

    public List<VersionFicha> Versiones { get; set; } = [];

    /// <summary>
    /// Todos los parámetros activos, tengan o no tolerancia en la versión
    /// elegida, más los retirados que esa versión sí define (historia).
    /// </summary>
    public List<ToleranciaFicha> Tolerancias { get; set; } = [];

    public VersionFicha? Seleccionada =>
        Versiones.FirstOrDefault(v => v.FichaId == FichaSeleccionadaId);

    public VersionFicha? Borrador =>
        Versiones.FirstOrDefault(v => v.Estado == EstadosFicha.Borrador);

    public VersionFicha? Vigente =>
        Versiones.FirstOrDefault(v => v.Estado == EstadosFicha.Vigente);

    public string Etiqueta => $"{Codigo} · {Nombre}";
}

public class VersionFicha
{
    public int FichaId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateOnly VigenteDesde { get; set; }

    /// <summary>Inicio de la siguiente versión publicada (exclusivo); null si es la última.</summary>
    public DateOnly? VigenteHasta { get; set; }

    public bool Activa { get; set; }
    public string Estado { get; set; } = EstadosFicha.Borrador;
    public int Tolerancias { get; set; }

    /// <summary>Registros de inspección que leen esta versión.</summary>
    public int Registros { get; set; }

    public bool EsBorrador => Estado == EstadosFicha.Borrador;

    /// <summary>
    /// Una versión publicada se puede devolver a borrador solo mientras ningún
    /// registro la use: los que ya la usan la vuelven a leer en cada consulta y
    /// en cada certificado.
    /// </summary>
    public bool SePuedeRetirar => Activa && Registros == 0;
}

/// <summary>
/// Un renglón de la tabla de tolerancias. Los límites son editables en la
/// pantalla cuando la versión es un borrador.
/// </summary>
public class ToleranciaFicha
{
    public int ParametroId { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public bool EsCritico { get; set; }
    public int Orden { get; set; }
    public bool ParametroActivo { get; set; }
    public string? AreaNombre { get; set; }

    /// <summary>Si la versión define este parámetro (la ficha decide qué aplica al producto).</summary>
    public bool Aplica { get; set; }

    public decimal? ValorObjetivo { get; set; }
    public decimal? LimiteInferior { get; set; }
    public decimal? LimiteSuperior { get; set; }
}

/// <summary>Una tolerancia tal como se envía al guardar el borrador.</summary>
public record ToleranciaEdicion(
    int ParametroId, decimal? ValorObjetivo, decimal? LimiteInferior, decimal? LimiteSuperior);
