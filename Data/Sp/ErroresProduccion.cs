namespace calidad_app.Data.Sp;

/// <summary>
/// Mensajes de los errores que lanzan los procedimientos del módulo de
/// producción (rangos 50300-50359).
///
/// Sigue la misma idea que <see cref="ErroresInspeccion"/>,
/// <see cref="ErroresCalidad"/> y <see cref="ErroresCatalogos"/>: el mensaje
/// que ve el usuario se redacta aquí, en español y con tildes, y se busca por
/// número, de modo que cambiar la redacción no obliga a tocar la base.
/// </summary>
internal static class ErroresProduccion
{
    internal static readonly Dictionary<int, string> Mensajes = new()
    {
        // Órdenes de producción
        [50300] = "No tiene autorización para administrar órdenes de producción.",
        [50301] = "El número de orden es obligatorio.",
        [50302] = "La orden de producción indicada no existe.",
        [50303] = "El cliente indicado no existe o está inactivo.",
        [50304] = "El producto indicado no existe o está inactivo.",
        [50305] = "Ya existe otra orden con ese número.",
        [50306] = "Los kilos programados no pueden ser negativos.",
        [50307] = "La orden está cerrada y ya no admite cambios.",
        [50308] = "No se puede cambiar el producto o el cliente de una orden que ya tiene registros de inspección.",
        [50310] = "El estado indicado no es válido para una orden de producción.",
        [50311] = "La orden ya se encuentra en ese estado.",
        [50312] = "No se puede cerrar la orden: tiene registros de inspección abiertos.",
        [50313] = "Para reabrir una orden cerrada debe indicarse el motivo.",
        [50314] = "No se puede cerrar la orden: tiene lotes que Calidad todavía no ha liberado.",

        // Asignación operador - máquina - turno
        [50320] = "No tiene autorización para asignar operadores.",
        [50321] = "La asignación indicada no existe.",
        [50322] = "El operador indicado no existe o está inactivo.",
        [50323] = "El usuario asignado no puede registrar inspecciones en planta.",
        [50324] = "La máquina indicada no existe o está inactiva.",
        [50325] = "El turno indicado no existe.",
        [50326] = "Esa máquina ya tiene un operador asignado en ese turno y fecha.",
        [50327] = "El operador ya está asignado a otra máquina en ese turno y fecha.",
        [50328] = "No se puede cambiar la asignación: ya hay registros de inspección capturados en esa máquina, turno y fecha.",
        [50329] = "No se puede eliminar la asignación: ya hay registros de inspección capturados en esa máquina, turno y fecha.",
        [50330] = "La fecha de origen y la de destino deben ser distintas.",
        [50331] = "La fecha de origen no tiene ninguna asignación que copiar.",
        [50332] = "La fecha de destino ya tiene asignaciones. Indique que desea reemplazarlas.",
        [50333] = "Debe indicarse la fecha de la asignación.",

        // Control de desperdicio
        [50340] = "El criterio de agrupación no es válido."
    };
}
