using Microsoft.Data.SqlClient;

namespace calidad_app.Data.Sp;

/// <summary>
/// Traduce los errores que lanzan los procedimientos almacenados.
///
/// El mapeo se hace por NÚMERO y no por texto: así el mensaje que ve el
/// usuario se redacta aquí, en español y con tildes (los literales de SQL
/// Server del proyecto van sin acentos para evitar problemas de codificación),
/// y cambiar la redacción no obliga a tocar la base.
///
/// Cada módulo mantiene su propio catálogo (<see cref="ErroresInspeccion"/>,
/// <see cref="ErroresCalidad"/>) y aquí se consultan en orden. Si aparece un
/// número sin traducir se usa el mensaje original del procedimiento, que ya es
/// legible: es lo que ocurre a propósito con el error 50148, cuyo texto trae la
/// lista de lo que impide liberar el registro.
/// </summary>
public static class ErroresSp
{
    /// <summary>Primer número reservado para errores lanzados a propósito con THROW.</summary>
    private const int PrimerErrorDeNegocio = 50000;

    /// <summary>
    /// Convierte un error de SQL Server en excepción de dominio cuando lo lanzó
    /// un THROW nuestro. Los errores por debajo de 50000 son fallos reales del
    /// motor (tiempo de espera, conexión, restricciones) y se dejan pasar para
    /// que los trate el manejador de errores de la aplicación.
    /// </summary>
    public static Exception Traducir(SqlException ex)
    {
        if (ex.Number < PrimerErrorDeNegocio)
        {
            return ex;
        }

        var mensaje =
            ErroresInspeccion.Mensajes.TryGetValue(ex.Number, out var inspeccion) ? inspeccion :
            ErroresCalidad.Mensajes.TryGetValue(ex.Number, out var calidad) ? calidad :
            ex.Message;

        return new ReglaNegocioException(mensaje, ex.Number);
    }
}
