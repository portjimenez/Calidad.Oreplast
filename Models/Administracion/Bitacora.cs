namespace calidad_app.Models.Administracion;

/// <summary>
/// Filtros de la bitácora. El rango de fechas es obligatorio (hasta un año):
/// es el único filtro que aprovecha el índice de la tabla.
/// </summary>
public class FiltroBitacora
{
    public DateOnly FechaDesde { get; set; } = DateOnly.FromDateTime(DateTime.Today).AddDays(-6);
    public DateOnly FechaHasta { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public int? UsuarioId { get; set; }
    public string? Accion { get; set; }
    public string? Entidad { get; set; }

    /// <summary>Texto libre: detalle, entidad afectada, IP, nombre o cuenta del usuario.</summary>
    public string? Busqueda { get; set; }

    /// <summary>
    /// Por omisión se ocultan los "Inicio de sesión exitoso": se escribe uno en
    /// cada carga de página y tapan lo que el auditor viene a buscar. Los accesos
    /// denegados se muestran siempre.
    /// </summary>
    public bool OcultarAccesosExitosos { get; set; } = true;
}

/// <summary>Una fila de aud.Bitacora, con el usuario ya resuelto.</summary>
public class EventoBitacora
{
    public long BitacoraId { get; set; }
    public DateTime FechaHora { get; set; }

    /// <summary>Null en los intentos de acceso con una cuenta no registrada.</summary>
    public int? UsuarioId { get; set; }
    public string? UsuarioCodigo { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? UsuarioDominio { get; set; }

    public string Accion { get; set; } = string.Empty;
    public string? Entidad { get; set; }
    public string? EntidadId { get; set; }
    public string? Detalle { get; set; }
    public string? DireccionIp { get; set; }

    /// <summary>
    /// seg.usp_Usuario_ValidarAcceso distingue los casos solo por el texto del
    /// detalle; los rechazos empiezan con "Intento de acceso".
    /// </summary>
    public bool EsAccesoDenegado =>
        Accion == "Acceso" && Detalle?.StartsWith("Intento de acceso", StringComparison.Ordinal) == true;

    public string AccionVisible => EsAccesoDenegado
        ? "Acceso denegado"
        : TextosAdministracion.Accion(Accion);
}

/// <summary>
/// Una página de la bitácora. Se pagina en la base, así que el total viene
/// aparte: es el que alimenta los controles, no el tamaño de la lista.
/// </summary>
public class PaginaBitacora
{
    public int Total { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 50;
    public List<EventoBitacora> Eventos { get; set; } = [];

    public int TotalPaginas => Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));

    public string Rango
    {
        get
        {
            if (Total == 0)
            {
                return "Sin eventos";
            }

            var desde = ((Pagina - 1) * TamanoPagina) + 1;
            var hasta = Math.Min(Pagina * TamanoPagina, Total);
            return $"{desde:N0}–{hasta:N0} de {Total:N0}";
        }
    }
}

/// <summary>Indicadores del encabezado de la bitácora para el rango consultado.</summary>
public class ResumenBitacora
{
    public int TotalEventos { get; set; }
    public int AccesosExitosos { get; set; }
    public int AccesosDenegados { get; set; }

    /// <summary>Todo lo que no es un acceso: altas, cambios, liberaciones, descargas…</summary>
    public int Operaciones { get; set; }

    public int UsuariosDistintos { get; set; }

    public List<ConteoAccion> PorAccion { get; set; } = [];
}

public record ConteoAccion(string Accion, int Total)
{
    public string AccionVisible => TextosAdministracion.Accion(Accion);
}
