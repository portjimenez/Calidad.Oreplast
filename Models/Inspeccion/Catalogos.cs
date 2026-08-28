namespace calidad_app.Models.Inspeccion;

/// <summary>Material de la mezcla, tal como se elige en la sección MAT#.</summary>
public class MaterialCatalogo
{
    public int MaterialId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    public string Etiqueta => $"{Codigo} — {Nombre}";
}

/// <summary>Razón de tiempo muerto de las secciones Setup y Producción.</summary>
public class RazonTiempoMuerto
{
    public int RazonId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>Operador que puede figurar como responsable de un setup o una corrida.</summary>
public class OperadorCatalogo
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;

    public string Etiqueta => $"{Codigo} — {NombreCompleto}";
}

/// <summary>
/// Catálogos de apoyo del registro. Se cargan de una sola vez al abrir el
/// registro: son listas cortas y estables durante la captura.
/// </summary>
public class CatalogosInspeccion
{
    public List<MaterialCatalogo> Materiales { get; set; } = [];
    public List<RazonTiempoMuerto> Razones { get; set; } = [];
    public List<OperadorCatalogo> Operadores { get; set; } = [];
    public List<TurnoCatalogo> Turnos { get; set; } = [];
    public List<MaquinaCatalogo> Maquinas { get; set; } = [];
    public List<TipoRegistroCatalogo> TiposRegistro { get; set; } = [];

    /// <summary>Máquinas activas del área indicada.</summary>
    public IEnumerable<MaquinaCatalogo> MaquinasDe(int areaId) =>
        Maquinas.Where(m => m.AreaId == areaId);
}

/// <summary>Turno de trabajo.</summary>
public class TurnoCatalogo
{
    public int TurnoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>Máquina activa, con el área a la que pertenece.</summary>
public class MaquinaCatalogo
{
    public int MaquinaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;

    public string Etiqueta => $"{Codigo} — {Nombre}";
}

/// <summary>Tipo de registro (formato) por área, por ejemplo REEX en extrusión.</summary>
public class TipoRegistroCatalogo
{
    public int TipoRegistroId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
}

/// <summary>Orden encontrada al buscar sobre cuál abrir un registro.</summary>
public class OrdenBusqueda
{
    public int OrdenId { get; set; }
    public string NumeroOP { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal? KgProgramados { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public int RegistrosExistentes { get; set; }

    /// <summary>Si el producto no tiene ficha, los parámetros no podrán compararse.</summary>
    public bool ProductoConFicha { get; set; }
}
