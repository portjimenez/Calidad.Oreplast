
namespace calidad_app.Data.Sp;

/// <summary>
/// Regla de negocio rechazada por la base (un THROW dentro de un procedimiento).
/// No es un fallo del sistema: es el resultado normal de intentar algo que las
/// reglas no permiten, y la pantalla lo muestra tal cual al usuario.
/// </summary>
public class ReglaNegocioException(string mensaje, int numero) : Exception(mensaje)
{
    public int Numero { get; } = numero;
}

/// <summary>
/// Mensajes de los errores que lanzan los procedimientos del módulo de
/// inspección (rangos 50010-50080). Quien los traduce es
/// <see cref="ErroresSp"/>, que consulta este catálogo y el del módulo de
/// calidad.
/// </summary>
internal static class ErroresInspeccion
{
    internal static readonly Dictionary<int, string> Mensajes = new()
    {
        // Alta del registro
        [50010] = "La orden de producción indicada no existe.",
        [50011] = "No se puede abrir un registro sobre una orden cerrada.",
        [50012] = "El tipo de registro indicado no existe.",
        [50013] = "La máquina no existe o está inactiva.",
        [50014] = "La máquina seleccionada no pertenece al área del tipo de registro.",
        [50015] = "El operador no existe o está inactivo.",
        [50016] = "El turno indicado no existe.",
        [50017] = "La clasificación del registro debe ser una letra (por ejemplo P o S).",
        [50018] = "No hay una versión de formato vigente para el tipo de registro en esa fecha.",
        [50019] = "Ya existe un registro abierto para esa orden, máquina, turno y fecha.",

        // Estado del registro
        [50020] = "El registro de inspección indicado no existe.",
        [50021] = "El registro está bloqueado o cerrado y ya no admite cambios.",
        [50022] = "La máquina no existe, está inactiva o no pertenece al área del registro.",

        // Mezcla de materiales
        [50030] = "La lista de materiales no tiene un formato válido.",
        [50031] = "Cada material debe indicar el material y su porcentaje.",
        [50032] = "El porcentaje de cada material debe estar entre 0 y 100.",
        [50033] = "Hay números de material (MAT#) repetidos en la mezcla.",
        [50034] = "La mezcla incluye un material inexistente o inactivo.",
        [50035] = "Los porcentajes de la mezcla deben sumar 100 %.",

        // Setup y producción
        [50040] = "La hora de fin del setup no puede ser anterior a la de inicio.",
        [50041] = "El tiempo muerto no puede ser negativo.",
        [50042] = "La razón de tiempo muerto no existe o está inactiva.",
        [50043] = "La hora de fin de producción no puede ser anterior a la de inicio.",

        // Parámetros
        [50050] = "El ámbito debe ser Registro o Bobina.",
        [50051] = "Para el ámbito Bobina debe indicarse la bobina.",
        [50052] = "La bobina indicada no pertenece a este registro.",
        [50053] = "La bobina está bloqueada y ya no admite cambios.",
        [50054] = "La lista de valores no tiene un formato válido.",
        [50055] = "No se recibió ningún parámetro para guardar.",
        [50056] = "Se envió un parámetro inexistente, inactivo o ajeno al área del registro.",

        // Bobinas
        [50060] = "El peso de la bobina debe ser mayor que cero.",
        [50061] = "Los metros de la bobina no pueden ser negativos.",
        [50062] = "El lote indicado no pertenece a la orden de este registro.",
        [50063] = "La bobina ya fue confirmada; retire la confirmación antes de corregirla.",
        [50064] = "Ya existe una bobina con ese ID BOBI en el registro.",
        [50065] = "La bobina indicada no existe.",
        [50066] = "No se puede confirmar una bobina sin peso registrado.",
        [50067] = "No se puede eliminar una bobina confirmada o bloqueada.",
        [50068] = "La bobina está ligada a una no conformidad y no puede eliminarse.",

        // Checklists
        [50070] = "La lista de respuestas no tiene un formato válido.",
        [50071] = "No se recibió ninguna respuesta para guardar.",
        [50072] = "Se envió un ítem de checklist inexistente, inactivo o ajeno al área.",
        [50080] = "La sección debe ser Despeje de línea o Cierre de orden."
    };
}
