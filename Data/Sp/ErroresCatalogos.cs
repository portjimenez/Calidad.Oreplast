namespace calidad_app.Data.Sp;

/// <summary>
/// Mensajes de los errores que lanzan los procedimientos del módulo de
/// catálogos (rangos 50200-50260).
///
/// Sigue la misma idea que <see cref="ErroresInspeccion"/> y
/// <see cref="ErroresCalidad"/>: el mensaje que ve el usuario se redacta aquí,
/// en español y con tildes, y se busca por número, de modo que cambiar la
/// redacción no obliga a tocar la base.
/// </summary>
internal static class ErroresCatalogos
{
    internal static readonly Dictionary<int, string> Mensajes = new()
    {
        // Comunes a todos los catálogos
        [50200] = "No tiene autorización para administrar los catálogos.",
        [50201] = "El nombre es obligatorio.",
        [50202] = "El código es obligatorio.",
        [50203] = "Ya existe otro elemento con ese código.",
        [50204] = "Ya existe otro elemento con ese nombre.",

        // Áreas
        [50210] = "El área indicada no existe.",
        [50211] = "No se puede desactivar el área mientras tenga líneas o máquinas activas.",

        // Líneas de producción
        [50215] = "La línea indicada no existe.",
        [50216] = "El área indicada no existe o está inactiva.",
        [50217] = "No se puede desactivar la línea mientras tenga máquinas activas asignadas.",
        [50218] = "No se puede cambiar de área una línea que ya tiene máquinas asignadas.",

        // Máquinas
        [50220] = "La máquina indicada no existe.",
        [50221] = "El área indicada no existe o está inactiva.",
        [50222] = "No se puede desactivar la máquina: tiene registros de inspección abiertos.",
        [50223] = "La línea indicada no existe, está inactiva o pertenece a otra área.",
        [50224] = "No se puede cambiar de área una máquina que ya tiene registros de inspección.",

        // Turnos
        [50225] = "El turno indicado no existe.",

        // Materiales
        [50230] = "El material indicado no existe.",

        // Parámetros medibles
        [50235] = "El parámetro indicado no existe.",
        [50236] = "El área indicada no existe o está inactiva.",
        [50237] = "No se puede desactivar el parámetro: hay fichas técnicas activas que lo usan.",
        [50238] = "No se puede cambiar el código de un parámetro que ya tiene mediciones registradas.",

        // Metas de producción
        [50240] = "La meta indicada no existe.",
        [50241] = "El concepto de la meta no es válido.",
        [50242] = "El valor de la meta debe ser un número mayor o igual a cero.",
        [50243] = "La máquina indicada no pertenece a la línea indicada.",
        [50244] = "Ya hay una meta activa para ese concepto y ámbito. Indique que reemplaza a la vigente.",
        [50245] = "Alguna referencia del ámbito de la meta (línea, máquina o producto) no existe.",

        // Ítems de verificación
        [50250] = "El ítem de verificación indicado no existe.",
        [50251] = "El tipo de ítem de verificación no es válido.",
        [50252] = "No se puede cambiar el tipo de un ítem que ya tiene respuestas registradas.",

        // Tipos de defecto
        [50255] = "El tipo de defecto indicado no existe.",

        // Razones de tiempo muerto
        [50260] = "La razón de tiempo muerto indicada no existe."
    };
}
