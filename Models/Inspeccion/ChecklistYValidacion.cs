namespace calidad_app.Models.Inspeccion;

/// <summary>Un ítem de checklist con la respuesta guardada, si la hay.</summary>
public class ItemChecklist
{
    public int ItemId { get; set; }
    public string ItemCodigo { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public int Orden { get; set; }
    public string? Seccion { get; set; }
    public int? RegChkId { get; set; }
    public int? BobinaId { get; set; }
    public bool Cumple { get; set; }
    public bool Respondido { get; set; }
}

/// <summary>Avance de una sección de checklist, para la barra de progreso.</summary>
public class AvanceChecklist
{
    public int ItemsTotal { get; set; }
    public int ItemsRespondidos { get; set; }
    public int ItemsCumplidos { get; set; }
    public bool Completo { get; set; }

    public int PorcentajeCumplido =>
        ItemsTotal == 0 ? 0 : (int)Math.Round(ItemsCumplidos * 100.0 / ItemsTotal);
}

/// <summary>Checklist completo de una sección.</summary>
public class Checklist
{
    public List<ItemChecklist> Items { get; set; } = [];
    public AvanceChecklist Avance { get; set; } = new();
}

/// <summary>Respuesta que la pantalla envía para guardar.</summary>
public record RespuestaChecklist(int ItemId, bool Cumple);

/// <summary>Secciones de checklist admitidas.</summary>
public static class SeccionChecklist
{
    public const string DespejeLinea = "DespejeLinea";
    public const string CierreOrden = "CierreOrden";
    public const string CalidadBobina = "CalidadBobina";
}

/// <summary>
/// Diagnóstico previo a la liberación: qué falta y qué impide liberar.
/// Es de solo lectura, no cambia nada en la base.
/// </summary>
public class ValidacionCompletitud
{
    public int RegistroId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool ListoParaLiberar { get; set; }
    public int TotalBloqueantes { get; set; }
    public int TotalAdvertencias { get; set; }
    public List<HallazgoValidacion> Hallazgos { get; set; } = [];

    public IEnumerable<HallazgoValidacion> Bloqueantes =>
        Hallazgos.Where(h => h.EsBloqueante);

    public IEnumerable<HallazgoValidacion> Advertencias =>
        Hallazgos.Where(h => !h.EsBloqueante);
}

public class HallazgoValidacion
{
    public string Codigo { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty;
    public string Seccion { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public int? Cantidad { get; set; }

    public bool EsBloqueante => Severidad == "Bloqueante";
}
