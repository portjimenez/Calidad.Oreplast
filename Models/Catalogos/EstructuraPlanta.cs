namespace calidad_app.Models.Catalogos;

/// <summary>
/// Encabezado de una pestaña de la pantalla de catálogos: cuántos elementos
/// tiene y cuántos siguen activos.
/// </summary>
public class ResumenCatalogo
{
    public int Orden { get; set; }

    /// <summary>Identifica la pestaña ("Maquinas", "Lineas", ...).</summary>
    public string Clave { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Activos { get; set; }
    public int Inactivos { get; set; }

    /// <summary>
    /// Falso solo para turnos: <c>cat.Turno</c> no tiene columna Activo, así
    /// que ese catálogo no ofrece el botón de retirar. La pantalla lo lee de
    /// aquí en lugar de llevar la excepción escrita.
    /// </summary>
    public bool AdmiteBaja { get; set; }
}

/// <summary>
/// Área del proceso productivo (Extrusión, Impresión, Laminación, Corte). Es
/// el catálogo raíz: líneas, máquinas, parámetros, ítems de verificación y
/// tipos de defecto cuelgan de él.
///
/// Los conteos vienen resueltos desde la base porque son lo que permite
/// decidir si un área se puede retirar; sin ellos la pantalla desactivaría a
/// ciegas.
/// </summary>
public class AreaCatalogo
{
    public int AreaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public int Lineas { get; set; }
    public int LineasActivas { get; set; }
    public int Maquinas { get; set; }
    public int MaquinasActivas { get; set; }
    public int Parametros { get; set; }
    public int ItemsChecklist { get; set; }
    public int TiposDefecto { get; set; }
    public int TiposRegistro { get; set; }
    public int Usuarios { get; set; }
    public int NoConformidades { get; set; }

    /// <summary>Hay algo registrado que depende de esta área.</summary>
    public bool EnUso { get; set; }

    /// <summary>
    /// El área solo se puede retirar cuando no le quedan líneas ni máquinas
    /// activas. Es la misma condición que valida la base; aquí sirve para no
    /// ofrecer un botón que va a fallar.
    /// </summary>
    public bool SePuedeDesactivar => LineasActivas == 0 && MaquinasActivas == 0;
}

/// <summary>
/// Línea de producción: agrupa las máquinas que trabajan encadenadas. Es el
/// corte por el que se sigue la planta cuando no interesa máquina por máquina.
/// </summary>
public class LineaCatalogo
{
    public int LineaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public bool AreaActiva { get; set; }
    public bool Activo { get; set; } = true;

    public int Maquinas { get; set; }
    public int MaquinasActivas { get; set; }
    public int Metas { get; set; }
    public bool EnUso { get; set; }

    public string Etiqueta => $"{Codigo} - {Nombre}";

    /// <summary>No se retira una línea que todavía tiene máquinas activas.</summary>
    public bool SePuedeDesactivar => MaquinasActivas == 0;

    /// <summary>
    /// Una línea no cambia de área cuando ya tiene máquinas asignadas: sus
    /// máquinas quedarían apuntando a un área que ya no es la suya.
    /// </summary>
    public bool PuedeCambiarDeArea => Maquinas == 0;
}

/// <summary>
/// Máquina de la planta, con su área y su línea ya resueltas.
///
/// <see cref="RegistrosAbiertos"/> es el dato que decide si se puede retirar
/// hoy: la pantalla de captura solo ofrece máquinas activas, así que
/// desactivarla dejaría esos registros sin poder cerrarse.
/// </summary>
public class MaquinaCatalogo
{
    public int MaquinaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public bool AreaActiva { get; set; }

    /// <summary>Opcional: hay máquinas que no pertenecen a ninguna línea.</summary>
    public int? LineaId { get; set; }
    public string? LineaCodigo { get; set; }
    public string? LineaNombre { get; set; }

    public bool Activo { get; set; } = true;

    public int Registros { get; set; }
    public int RegistrosAbiertos { get; set; }
    public DateOnly? UltimoRegistro { get; set; }
    public int Metas { get; set; }
    public bool EnUso { get; set; }

    public string Etiqueta => $"{Codigo} - {Nombre}";

    public bool SePuedeDesactivar => RegistrosAbiertos == 0;

    /// <summary>
    /// Con registros de inspección hechos, el área queda fija: cambiarla
    /// contaría esa producción en un área donde nunca ocurrió.
    /// </summary>
    public bool PuedeCambiarDeArea => Registros == 0;
}

/// <summary>
/// Turno de trabajo. Único catálogo sin estado: <c>cat.Turno</c> no tiene
/// columna Activo, así que se da de alta y se renombra, pero no se retira.
/// </summary>
public class TurnoCatalogo
{
    public int TurnoId { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public int Registros { get; set; }
    public DateOnly? UltimoRegistro { get; set; }
    public bool EnUso { get; set; }
}
