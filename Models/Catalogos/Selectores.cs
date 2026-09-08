namespace calidad_app.Models.Catalogos;

/// <summary>
/// Listas que alimentan los combos de los formularios de catálogos: el área de
/// una máquina, la línea de una máquina, el ámbito de una meta. Se cargan de
/// una sola llamada al abrir la pantalla.
///
/// Incluyen los elementos INACTIVOS, cada uno con su bandera. Es la diferencia
/// con los catálogos que usa la captura de inspección: allí ofrecer una
/// máquina retirada sería un error, pero aquí se administra el catálogo, y al
/// abrir una máquina vieja que apunta a una línea ya retirada el combo tiene
/// que poder mostrarla, o la pantalla la perdería en silencio al guardar.
/// </summary>
public class SelectoresCatalogo
{
    public List<AreaOpcion> Areas { get; set; } = [];
    public List<LineaOpcion> Lineas { get; set; } = [];
    public List<MaquinaOpcion> Maquinas { get; set; } = [];

    /// <summary>
    /// Solo los que coincidieron con la búsqueda: son más de trescientos y
    /// crecen con el ERP, así que no caben en un combo.
    /// </summary>
    public List<ProductoOpcion> Productos { get; set; } = [];

    /// <summary>Conceptos de meta, tal como los admite la base.</summary>
    public List<OpcionClave> ConceptosMeta { get; set; } = [];

    /// <summary>Tipos de ítem de verificación, tal como los admite la base.</summary>
    public List<OpcionClave> TiposItemChecklist { get; set; } = [];

    /// <summary>Líneas activas del área indicada: las que puede tomar una máquina de esa área.</summary>
    public IEnumerable<LineaOpcion> LineasDe(int areaId) =>
        Lineas.Where(l => l.AreaId == areaId && l.Activo);

    public IEnumerable<MaquinaOpcion> MaquinasDe(int lineaId) =>
        Maquinas.Where(m => m.LineaId == lineaId);
}

public class AreaOpcion
{
    public int AreaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class LineaOpcion
{
    public int LineaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public bool Activo { get; set; }

    public string Etiqueta => $"{Codigo} - {Nombre}";
}

public class MaquinaOpcion
{
    public int MaquinaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public int? LineaId { get; set; }
    public string? LineaNombre { get; set; }
    public bool Activo { get; set; }

    public string Etiqueta => $"{Codigo} - {Nombre}";
}

public class ProductoOpcion
{
    public int ProductoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }

    public string Etiqueta => $"{Codigo} - {Nombre}";
}

/// <summary>
/// Opción de una lista cerrada que vive en la base (los conceptos de meta, los
/// tipos de ítem). Se leen de allí y no se escriben en la pantalla para que
/// ambos lados sigan diciendo lo mismo si la lista cambia.
/// </summary>
public class OpcionClave
{
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
}
