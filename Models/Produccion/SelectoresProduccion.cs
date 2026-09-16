using calidad_app.Models.Catalogos;

namespace calidad_app.Models.Produccion;

/// <summary>
/// Listas que alimentan los filtros y formularios de las tres pantallas del
/// módulo. Se cargan de una sola llamada al abrir la pantalla.
///
/// A diferencia de los selectores de catálogos, aquí solo vienen elementos
/// ACTIVOS: estas pantallas programan y miden producción, y ofrecer una
/// máquina retirada o un operador dado de baja solo lleva a un guardado
/// rechazado. La excepción es <c>incluirInactivos</c>, que se usa al abrir un
/// registro viejo cuyo ámbito ya se retiró para no perderlo en silencio.
///
/// Reutiliza los tipos de opción del módulo de catálogos (área, línea, máquina,
/// producto) en lugar de declarar copias: son las mismas entidades y duplicar
/// la clase solo obligaría a mantener dos mapeos en paralelo.
/// </summary>
public class SelectoresProduccion
{
    /// <summary>Estados de la orden, leídos de la base y no escritos en la pantalla.</summary>
    public List<OpcionClave> EstadosOrden { get; set; } = [];

    public List<ClienteOpcion> Clientes { get; set; } = [];
    public List<AreaOpcion> Areas { get; set; } = [];
    public List<LineaOpcion> Lineas { get; set; } = [];
    public List<MaquinaOpcion> Maquinas { get; set; } = [];
    public List<TurnoOpcion> Turnos { get; set; } = [];

    /// <summary>Usuarios que pueden capturar en planta (permiso REGISTRAR_INSPECCION).</summary>
    public List<OperadorOpcion> Operadores { get; set; } = [];

    public List<RazonOpcion> Razones { get; set; } = [];

    /// <summary>
    /// Solo los que coincidieron con la búsqueda: son más de trescientos y
    /// crecen con el ERP, así que no caben en un combo.
    /// </summary>
    public List<ProductoOpcion> Productos { get; set; } = [];

    public IEnumerable<LineaOpcion> LineasDe(int areaId) =>
        Lineas.Where(l => l.AreaId == areaId);

    public IEnumerable<MaquinaOpcion> MaquinasDe(int lineaId) =>
        Maquinas.Where(m => m.LineaId == lineaId);
}

public class ClienteOpcion
{
    public int ClienteId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }

    public string Etiqueta => Nombre;
}

public class TurnoOpcion
{
    public int TurnoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class OperadorOpcion
{
    public int UsuarioId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public bool Activo { get; set; }

    public string Etiqueta => $"{Codigo} - {NombreCompleto}";
}

public class RazonOpcion
{
    public int RazonId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
