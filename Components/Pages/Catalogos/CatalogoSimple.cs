namespace calidad_app.Components.Pages.Catalogos;

/// <summary>
/// Fila de un catálogo simple, tal como la dibuja
/// <see cref="PestanaCatalogoSimple"/>.
///
/// Cinco de los diez catálogos (áreas, materiales, turnos, tipos de defecto y
/// razones de tiempo muerto) son la misma pantalla: un nombre, a veces un
/// código, a veces un área, y el estado. En lugar de escribir cinco tablas y
/// cinco formularios casi idénticos, la página adapta cada modelo a esta fila
/// y usa un solo componente.
///
/// La conversión de ida y vuelta vive en la página, junto a la llamada al
/// servicio de cada catálogo, para que el componente no tenga que saber con
/// cuál está trabajando.
/// </summary>
public sealed class FilaCatalogo
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Null es "todas las áreas" en los catálogos que lo admiten.</summary>
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }

    public bool Activo { get; set; } = true;

    /// <summary>Cuántas veces se ha usado; es lo que se mira antes de retirarlo.</summary>
    public int Usos { get; set; }

    /// <summary>Segunda línea de la fila (la última vez que se usó, por ejemplo).</summary>
    public string? Detalle { get; set; }

    public FilaCatalogo Copiar() => (FilaCatalogo)MemberwiseClone();
}

/// <summary>
/// Todo lo que <see cref="PestanaCatalogoSimple"/> necesita saber del catálogo
/// que está mostrando: cómo se llama, qué columnas tiene sentido dibujar y qué
/// hacer para cargar, guardar y cambiar de estado.
///
/// Los tres delegados los arma la página, que es quien conoce los servicios.
/// Así el componente queda como lo que es: una tabla y un formulario.
/// </summary>
public sealed class CatalogoSimple
{
    /// <summary>Nombre en plural, para los mensajes de la tabla.</summary>
    public required string Titulo { get; init; }

    /// <summary>Nombre en singular, para el botón de alta y el título del formulario.</summary>
    public required string Singular { get; init; }

    /// <summary>El elemento tiene código propio además del nombre.</summary>
    public bool UsaCodigo { get; init; }

    /// <summary>El elemento se puede acotar a un área.</summary>
    public bool UsaArea { get; init; }

    /// <summary>
    /// Falso solo para turnos: <c>cat.Turno</c> no tiene columna Activo, así que
    /// ese catálogo no ofrece retirar ni reincorporar.
    /// </summary>
    public bool AdmiteBaja { get; init; } = true;

    /// <summary>Encabezado de la columna de uso ("Usos", "No conformidades"...).</summary>
    public string EncabezadoUso { get; init; } = "Usos";

    /// <summary>Explicación al pie del formulario: qué significa retirar este catálogo.</summary>
    public string? Nota { get; init; }

    /// <summary>Carga la lista. Recibe la búsqueda y si se piden solo los activos.</summary>
    public required Func<string?, bool, Task<List<FilaCatalogo>>> Cargar { get; init; }

    /// <summary>Guarda el alta o la modificación y devuelve el id resultante.</summary>
    public required Func<FilaCatalogo, Task<int>> Guardar { get; init; }

    /// <summary>Retira el elemento o lo reincorpora.</summary>
    public required Func<FilaCatalogo, bool, Task> CambiarEstado { get; init; }
}
