namespace calidad_app.Models.Inspeccion;

/// <summary>
/// Sección "Especificaciones del proceso" (relación 1:1 con el registro).
/// El procedimiento siempre devuelve una fila: cuando aún no se ha capturado,
/// EspecId viene en null y el formulario se pinta vacío.
/// </summary>
public class EspecificacionProceso
{
    public int RegistroId { get; set; }
    public int? EspecId { get; set; }
    public decimal? KgAProducir { get; set; }
    public decimal? AnchoProduccionMm { get; set; }
    public decimal? CalibreProduccionMic { get; set; }
    public decimal? Fuelle { get; set; }
    public string? Color { get; set; }
    public bool? Tratado { get; set; }
    public string? TipoBobina { get; set; }
    public string? TipoMaterial { get; set; }
    public string? TipoSello { get; set; }
    public string? Estructura { get; set; }
    public int? BobinasPorEmbobinador { get; set; }
    public bool? Abierta { get; set; }
    public bool? Impresa { get; set; }
    public string? UsoFinal { get; set; }
    public bool? Rotulado { get; set; }
    public decimal? MetrosAproxBobina { get; set; }
    public decimal? KgAproxBobina { get; set; }
    public decimal? AnchoProductoMm { get; set; }
    public decimal? LargoProductoMm { get; set; }
    public decimal? CalibreProductoMic { get; set; }
    public decimal? AnchoExtrusionMm { get; set; }
    public decimal? CalibreExtrusionMic { get; set; }
    public decimal? AlturaImpresion { get; set; }
    public string? Observaciones { get; set; }
    public bool Bloqueado { get; set; }
    public string Estado { get; set; } = string.Empty;

    public bool Capturada => EspecId is not null;
}

/// <summary>Una línea de la mezcla de materiales (sección MAT#).</summary>
public class MaterialMezcla
{
    public int? RegMaterialId { get; set; }
    public int RegistroId { get; set; }
    public int NumeroMat { get; set; }
    public int MaterialId { get; set; }
    public string MaterialCodigo { get; set; } = string.Empty;
    public string MaterialNombre { get; set; } = string.Empty;
    public bool MaterialActivo { get; set; }
    public string? CodigoFabricante { get; set; }
    public string? Lote { get; set; }
    public decimal Porcentaje { get; set; }
}

/// <summary>
/// Mezcla completa con su total. La base calcula si suma 100 % para que la
/// regla sea la misma la evalúe la pantalla o la validación previa a liberar.
/// </summary>
public class MezclaMateriales
{
    public List<MaterialMezcla> Materiales { get; set; } = [];
    public decimal PorcentajeTotal { get; set; }
    public bool SumaCompleta { get; set; }
    public int TotalMateriales { get; set; }
}

/// <summary>Sección "Setup" (montaje de la máquina antes de producir).</summary>
public class SetupRegistro
{
    public int RegistroId { get; set; }
    public int? SetupId { get; set; }
    public DateOnly? FechaSetup { get; set; }
    public int? OperadorSetupId { get; set; }
    public string? OperadorSetupNombre { get; set; }
    public DateTime? FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public decimal? HorasSetup { get; set; }
    public int? TiempoMuertoMin { get; set; }
    public int? RazonId { get; set; }
    public string? RazonNombre { get; set; }
    public decimal? KgDesperdicio { get; set; }
    public decimal? KgDuro { get; set; }
    public bool Bloqueado { get; set; }
    public string Estado { get; set; } = string.Empty;

    public bool Capturado => SetupId is not null;
}

/// <summary>Sección "Producción" (la corrida propiamente dicha).</summary>
public class ProduccionRegistro
{
    public int RegistroId { get; set; }
    public int? ProduccionId { get; set; }
    public DateOnly? FechaProduccion { get; set; }
    public int? OperadorId { get; set; }
    public string? OperadorNombre { get; set; }
    public DateTime? FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public decimal? HorasProduccion { get; set; }
    public int? TiempoMuertoMin { get; set; }
    public int? RazonId { get; set; }
    public string? RazonNombre { get; set; }
    public decimal? KgDesperdicio { get; set; }
    public decimal? KgRefill { get; set; }
    /// <summary>Kilogramos acumulados en las bobinas ya confirmadas.</summary>
    public decimal KgProducidos { get; set; }

    /// <summary>
    /// Cuántas bobinas están confirmadas (no cuántas hay). Las que siguen en
    /// captura no cuentan como producción: el operador aún puede corregirlas.
    /// </summary>
    public int TotalBobinas { get; set; }
    public bool Bloqueado { get; set; }
    public string Estado { get; set; } = string.Empty;

    public bool Capturada => ProduccionId is not null;
}

/// <summary>Línea de mezcla que la pantalla envía al guardar.</summary>
public record MaterialEntrada(
    int NumeroMat,
    int MaterialId,
    string? CodigoFabricante,
    string? Lote,
    decimal Porcentaje);

/// <summary>Datos mínimos para abrir un registro de inspección.</summary>
public record NuevoRegistro(
    int OrdenId,
    int TipoRegistroId,
    DateOnly Fecha,
    int TurnoId,
    int OperadorId,
    int MaquinaId,
    string Clasificacion = "P",
    DateTime? FechaHoraInicio = null);
