namespace calidad_app.Models.Inspeccion;

/// <summary>Una bobina de la corrida, con el avance de su checklist de calidad.</summary>
public class BobinaResumen
{
    public int BobinaId { get; set; }
    public int RegistroId { get; set; }
    public int IdBobi { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Metros { get; set; }
    public decimal? Ancho { get; set; }
    public decimal? Fuelle { get; set; }
    public decimal? Calibre { get; set; }
    /// <summary>Sin parámetros fuera de rango y con el checklist completo.</summary>
    public bool Ok { get; set; }
    public bool Confirmada { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
    public bool Bloqueada { get; set; }
    /// <summary>Lo determina Ingeniería de Calidad, no la confirmación del operador.</summary>
    public bool EsConforme { get; set; }
    public int? NoConformidadId { get; set; }
    public string? NoConformidadCodigo { get; set; }
    public int? LoteId { get; set; }
    public string? CodigoLote { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int ItemsCumplidos { get; set; }
    public int ItemsRespondidos { get; set; }
    public int ParametrosFueraDeRango { get; set; }

    public bool EsEditable => !Confirmada && !Bloqueada;
}

/// <summary>
/// Acumulado de una medida sobre todas las bobinas del registro. No se almacena:
/// se calcula en cada consulta a partir de prod.Bobina, para que no haya dos
/// versiones del mismo dato cuando se corrige el peso de una bobina.
/// </summary>
public class AcumuladoMedida
{
    public string Medida { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal? Total { get; set; }
    public decimal? Minimo { get; set; }
    public decimal? Maximo { get; set; }
    public decimal? Promedio { get; set; }
    /// <summary>Null cuando hay menos de dos bobinas con dato: no hay dispersión que calcular.</summary>
    public decimal? Desviacion { get; set; }
}

/// <summary>Bobinas del registro junto con sus acumulados.</summary>
public class ProduccionPorBobina
{
    public List<BobinaResumen> Bobinas { get; set; } = [];
    public List<AcumuladoMedida> Acumulados { get; set; } = [];

    public decimal PesoTotal => Bobinas.Sum(b => b.Peso ?? 0m);
    public int Confirmadas => Bobinas.Count(b => b.Confirmada);
}

/// <summary>Datos que la pantalla envía para dar de alta o corregir una bobina.</summary>
public class BobinaEntrada
{
    public int? BobinaId { get; set; }
    public int? IdBobi { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Metros { get; set; }
    public decimal? Ancho { get; set; }
    public decimal? Fuelle { get; set; }
    public decimal? Calibre { get; set; }
    public int? LoteId { get; set; }
}

/// <summary>Resultado de confirmar una bobina.</summary>
public class ConfirmacionBobina
{
    public int BobinaId { get; set; }
    public int IdBobi { get; set; }
    public bool Confirmada { get; set; }
    public bool Ok { get; set; }
    public int ParametrosFueraDeRango { get; set; }
    public int ItemsChecklistPendientes { get; set; }
    public int AlertasGeneradas { get; set; }
    public List<DesviacionParametro> Desviaciones { get; set; } = [];
}
