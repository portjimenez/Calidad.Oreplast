using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace calidad_app.Data.Sp;

/// <summary>
/// Ejecuta procedimientos almacenados por ADO.NET.
///
/// EF Core solo sabe leer el primer conjunto de resultados de una consulta, y
/// buena parte de los procedimientos del módulo de inspección devuelven dos o
/// más (encabezado + lotes, bobinas + acumulados, resumen + hallazgos). Para
/// esos casos hay que llegar al DbDataReader y avanzar con NextResult.
///
/// La conexión se toma del propio DbContext para no tener una segunda fuente de
/// configuración: la cadena de conexión sigue siendo la que se registra en
/// Program.cs.
/// </summary>
public sealed class EjecutorSp(IDbContextFactory<AppDbContext> dbFactory)
{
    /// <summary>
    /// Ejecuta el procedimiento y entrega el lector a <paramref name="leer"/>,
    /// que se encarga de mapear todos los conjuntos de resultados.
    /// </summary>
    public async Task<T> ConsultarAsync<T>(
        string procedimiento,
        Action<DbCommand> parametros,
        Func<DbDataReader, CancellationToken, Task<T>> leer,
        CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var conexion = db.Database.GetDbConnection();

        try
        {
            await db.Database.OpenConnectionAsync(ct);

            await using var comando = CrearComando(conexion, procedimiento, parametros);
            await using var lector = await comando.ExecuteReaderAsync(ct);

            return await leer(lector, ct);
        }
        catch (SqlException ex)
        {
            throw ErroresInspeccion.Traducir(ex);
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }

    /// <summary>Ejecuta un procedimiento cuyo resultado no interesa.</summary>
    public async Task EjecutarAsync(
        string procedimiento,
        Action<DbCommand> parametros,
        CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var conexion = db.Database.GetDbConnection();

        try
        {
            await db.Database.OpenConnectionAsync(ct);

            await using var comando = CrearComando(conexion, procedimiento, parametros);
            await comando.ExecuteNonQueryAsync(ct);
        }
        catch (SqlException ex)
        {
            throw ErroresInspeccion.Traducir(ex);
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }

    private static DbCommand CrearComando(
        DbConnection conexion, string procedimiento, Action<DbCommand> parametros)
    {
        var comando = conexion.CreateCommand();
        comando.CommandText = procedimiento;
        comando.CommandType = CommandType.StoredProcedure;
        parametros(comando);
        return comando;
    }
}

/// <summary>Alta de parámetros con la conversión de tipos que espera SQL Server.</summary>
public static class ComandoExtensiones
{
    /// <summary>
    /// Agrega un parámetro con nombre. Un valor null se envía como DBNull, que
    /// es como los procedimientos representan "no filtrar" o "no cambiar".
    /// </summary>
    public static DbCommand Con(this DbCommand comando, string nombre, object? valor)
    {
        var parametro = comando.CreateParameter();
        parametro.ParameterName = nombre;
        parametro.Value = Normalizar(valor);
        comando.Parameters.Add(parametro);
        return comando;
    }

    private static object Normalizar(object? valor) => valor switch
    {
        null => DBNull.Value,
        // SQL Server no tiene DateOnly: las columnas DATE viajan como DateTime.
        DateOnly fecha => fecha.ToDateTime(TimeOnly.MinValue),
        string texto when texto.Length == 0 => DBNull.Value,
        _ => valor
    };
}
