namespace calidad_app.Data.Sp;

/// <summary>
/// Mensajes de los errores que lanzan los procedimientos del módulo de
/// reportería e indicadores (rangos 50500-50539).
///
/// Sigue la misma idea que los catálogos de los módulos anteriores: el mensaje
/// que ve el usuario se redacta aquí, en español y con tildes, y se busca por
/// número, de modo que cambiar la redacción no obliga a tocar la base.
/// </summary>
internal static class ErroresReportes
{
    internal static readonly Dictionary<int, string> Mensajes = new()
    {
        // Periodo (cal.usp_Kpi_ValidarPeriodo, compartido por todo el módulo)
        [50500] = "Debe indicarse el rango de fechas del periodo.",
        [50501] = "La fecha inicial no puede ser posterior a la final.",
        [50502] = "El periodo consultado no puede superar dos años.",

        // Opciones de las vistas del tablero
        [50503] = "La escala de la tendencia no es válida.",
        [50504] = "El criterio de agrupación no es válido.",
        [50505] = "El origen del análisis de causas no es válido.",
        [50506] = "El número máximo de causas a mostrar no es válido.",
        [50507] = "El número máximo de filas del reporte no es válido.",

        // Reportes y su descarga
        [50510] = "No tiene autorización para generar reportes.",
        [50511] = "Debe indicarse cuál reporte se generó.",
        [50512] = "Debe indicarse el formato del archivo generado."
    };
}
