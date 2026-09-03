namespace calidad_app.Data.Sp;

/// <summary>
/// Mensajes de los errores que lanzan los procedimientos del módulo de calidad
/// (rangos 50100-50166).
///
/// Van en un catálogo aparte del de inspección por la misma razón por la que
/// los procedimientos están en carpetas separadas: cada módulo mantiene sus
/// mensajes sin tocar los del otro. <see cref="ErroresSp"/> los junta al
/// traducir.
/// </summary>
internal static class ErroresCalidad
{
    internal static readonly Dictionary<int, string> Mensajes = new()
    {
        // Lotes de producción
        [50100] = "El lote indicado no existe.",
        [50101] = "La orden de producción indicada no existe.",
        [50102] = "No se puede abrir un lote sobre una orden cerrada.",
        [50103] = "Ya existe un lote con ese código.",
        [50104] = "Alguna bobina no existe o pertenece a una orden distinta a la del lote.",
        [50105] = "El lote ya fue liberado o cerrado y no admite cambios en sus bobinas.",
        [50106] = "La lista de bobinas no tiene un formato válido.",
        [50107] = "Solo se pueden agrupar en un lote las bobinas ya confirmadas.",
        [50108] = "Alguna bobina ya pertenece a otro lote.",

        // Alertas de proceso
        [50110] = "La alerta indicada no existe.",
        [50111] = "Debe indicarse al menos una alerta para atender.",
        [50112] = "Escriba la observación con la que se atiende la alerta.",
        [50113] = "La no conformidad indicada no existe.",
        [50114] = "La lista de alertas no tiene un formato válido.",

        // No conformidades
        [50120] = "La no conformidad indicada no existe.",
        [50121] = "La descripción de la no conformidad es obligatoria.",
        [50122] = "El tipo de defecto no existe o está inactivo.",
        [50123] = "La severidad indicada no existe.",
        [50124] = "El responsable indicado no existe o está inactivo.",
        [50125] = "El registro de inspección indicado no existe.",
        [50126] = "Debe indicarse el área de la no conformidad.",
        [50127] = "La orden de producción indicada no existe.",
        [50128] = "No está definido el estado inicial Registrada en el catálogo.",
        [50129] = "La no conformidad ya está cerrada o anulada y no admite cambios.",
        [50130] = "El estado indicado no existe.",
        [50131] = "La no conformidad ya se encuentra en ese estado.",
        [50132] = "La transición de estado solicitada no está permitida.",
        [50133] = "No se puede cerrar la no conformidad sin registrar la causa raíz.",
        [50134] = "No se puede cerrar la no conformidad sin registrar la acción correctiva.",
        [50135] = "Para anular la no conformidad debe indicarse el motivo.",
        [50136] = "Alguna de las bobinas indicadas no existe.",
        [50137] = "Alguna bobina ya está vinculada a otra no conformidad.",
        [50138] = "Debe indicarse el nombre del archivo de evidencia.",
        [50139] = "La evidencia debe traer el archivo o la ruta donde está almacenado.",
        [50140] = "La evidencia indicada no existe.",
        [50141] = "No se pueden eliminar evidencias de una no conformidad cerrada o anulada.",

        // Liberación y cierre de orden
        [50142] = "El tipo de liberación debe ser Despeje de línea o Cierre de orden.",
        [50143] = "El usuario no tiene autorización para liberar producto.",
        [50144] = "Esa liberación ya fue firmada para este registro.",
        [50145] = "Deben confirmarse las verificaciones de calidad e inocuidad para liberar.",
        [50146] = "No se puede cerrar la orden sin haber firmado antes el despeje de línea.",
        [50147] = "El lote indicado no pertenece a la orden de este registro.",
        // 50148 llega con el detalle de lo que falta dentro del propio mensaje,
        // así que se deja pasar el texto del procedimiento sin traducir.
        [50149] = "Los kilos de producto no conforme no pueden ser negativos.",
        [50150] = "Los kilos no conformes no pueden superar el peso producido en el registro.",
        [50151] = "Debe indicarse la razón del producto no conforme.",

        // Certificados de calidad
        [50160] = "El usuario no tiene autorización para emitir certificados de calidad.",
        [50161] = "El lote ya tiene un certificado de calidad emitido.",
        [50162] = "Solo se puede certificar un lote liberado por Ingeniería de Calidad.",
        [50163] = "El lote no tiene bobinas asignadas.",
        [50164] = "El lote tiene no conformidades abiertas: deben resolverse antes de certificar.",
        [50165] = "Debe indicarse el certificado o el lote a consultar.",
        [50166] = "El certificado indicado no existe."
    };
}
