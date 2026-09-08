namespace calidad_app.Models.Catalogos;

/// <summary>
/// Material de la mezcla (sección MAT# del registro de inspección).
///
/// <see cref="Codigo"/> es el código Oreplast del material: la columna de la
/// base se llama CodigoOreplast, pero el procedimiento la devuelve como Codigo
/// para que la aplicación nombre igual al código de todos los catálogos.
/// </summary>
public class MaterialCatalogo
{
    public int MaterialId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public int Usos { get; set; }
    public DateOnly? UltimoUso { get; set; }
    public bool EnUso { get; set; }

    public string Etiqueta => $"{Codigo} - {Nombre}";
}

/// <summary>
/// Parámetro medible del proceso (calibre, ancho, velocidad de motor...).
///
/// Es el catálogo del que dependen las fichas técnicas: cada tolerancia apunta
/// a un parámetro de aquí, y cada alerta del módulo de inspección nace de
/// comparar una medición contra esa tolerancia. Por eso el listado trae
/// <see cref="FichasActivas"/>: un parámetro con fichas vigentes detrás es la
/// definición contra la que se está liberando producto hoy.
/// </summary>
public class ParametroCatalogo
{
    public int ParametroId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Unidad { get; set; }

    /// <summary>Null significa que el parámetro aplica a todas las áreas.</summary>
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }

    public bool EsCritico { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;

    public int Tolerancias { get; set; }
    public int FichasActivas { get; set; }
    public int Mediciones { get; set; }
    public int Alertas { get; set; }
    public bool EnUso { get; set; }

    public string Etiqueta => Unidad is null ? Nombre : $"{Nombre} ({Unidad})";

    /// <summary>
    /// Con fichas técnicas activas detrás no se retira: la tolerancia se
    /// seguiría aplicando sin poder verse ni mantenerse desde el catálogo.
    /// </summary>
    public bool SePuedeDesactivar => FichasActivas == 0;

    /// <summary>
    /// El código es como nombra al parámetro el formato ya firmado, así que
    /// queda fijo en cuanto hay mediciones.
    /// </summary>
    public bool PuedeCambiarCodigo => Mediciones == 0;
}

/// <summary>
/// Ítem de verificación: una casilla del despeje de línea, del cierre de orden
/// o del control de calidad por bobina. Es, literalmente, una línea del
/// formato que el operador firma.
/// </summary>
public class ItemChecklistCatalogo
{
    public int ItemId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;

    /// <summary>DespejeLinea | CierreOrden | CalidadBobina.</summary>
    public string Tipo { get; set; } = TiposItemChecklist.DespejeLinea;

    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }

    /// <summary>Orden en que aparece en el formulario, no alfabético.</summary>
    public int Orden { get; set; }

    public bool Activo { get; set; } = true;

    public int Respuestas { get; set; }
    public bool EnUso { get; set; }

    /// <summary>
    /// Un ítem ya respondido no cambia de tipo: sus respuestas quedarían
    /// colgando de una sección a la que ya no pertenece.
    /// </summary>
    public bool PuedeCambiarTipo => Respuestas == 0;
}

/// <summary>
/// Tipo de defecto: la clasificación con la que se levanta una no conformidad,
/// y lo que hace posible el análisis de tendencias.
/// </summary>
public class TipoDefectoCatalogo
{
    public int TipoDefectoId { get; set; }
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Null es un defecto transversal, que puede ocurrir en cualquier área.</summary>
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }

    public bool Activo { get; set; } = true;

    public int NoConformidades { get; set; }
    public int Abiertas { get; set; }
    public bool EnUso { get; set; }
}

/// <summary>
/// Razón de tiempo muerto: por qué se detuvo la máquina. Se elige en las
/// secciones Setup y Producción del registro.
/// </summary>
public class RazonTiempoMuertoCatalogo
{
    public int RazonId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    /// <summary>Un paro en el montaje es un problema de preparación...</summary>
    public int UsosEnSetup { get; set; }

    /// <summary>...y uno en la corrida, un problema de proceso. Por eso van separados.</summary>
    public int UsosEnProduccion { get; set; }

    public bool EnUso { get; set; }
}
