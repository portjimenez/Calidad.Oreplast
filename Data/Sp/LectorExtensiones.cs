using System.Data.Common;

namespace calidad_app.Data.Sp;

/// <summary>
/// Lectura tipada y tolerante a nulos de un DbDataReader, por nombre de columna.
/// Los procedimientos del módulo devuelven varios conjuntos de resultados y EF
/// Core solo sabe leer el primero, así que esas consultas bajan a ADO.NET; estas
/// extensiones evitan repetir GetOrdinal + IsDBNull en cada mapeo.
/// </summary>
public static class LectorExtensiones
{
    public static int Entero(this DbDataReader lector, string columna) =>
        lector.GetInt32(lector.GetOrdinal(columna));

    public static int? EnteroNulo(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? null : lector.GetInt32(i);
    }

    public static string Texto(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? string.Empty : lector.GetString(i);
    }

    public static string? TextoNulo(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? null : lector.GetString(i);
    }

    public static decimal Decimal(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? 0m : lector.GetDecimal(i);
    }

    public static decimal? DecimalNulo(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? null : lector.GetDecimal(i);
    }

    public static bool Booleano(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return !lector.IsDBNull(i) && lector.GetBoolean(i);
    }

    public static bool? BooleanoNulo(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? null : lector.GetBoolean(i);
    }

    public static DateTime Fecha(this DbDataReader lector, string columna) =>
        lector.GetDateTime(lector.GetOrdinal(columna));

    public static DateTime? FechaNula(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? null : lector.GetDateTime(i);
    }

    /// <summary>Columnas DATE de SQL Server: llegan como DateTime y se reducen a DateOnly.</summary>
    public static DateOnly SoloFecha(this DbDataReader lector, string columna) =>
        DateOnly.FromDateTime(lector.GetDateTime(lector.GetOrdinal(columna)));

    public static DateOnly? SoloFechaNula(this DbDataReader lector, string columna)
    {
        var i = lector.GetOrdinal(columna);
        return lector.IsDBNull(i) ? null : DateOnly.FromDateTime(lector.GetDateTime(i));
    }

    /// <summary>Recorre el conjunto de resultados actual y mapea cada fila.</summary>
    public static async Task<List<T>> LeerListaAsync<T>(
        this DbDataReader lector, Func<DbDataReader, T> mapear, CancellationToken ct = default)
    {
        var filas = new List<T>();
        while (await lector.ReadAsync(ct))
        {
            filas.Add(mapear(lector));
        }

        return filas;
    }

    /// <summary>Primera fila del conjunto actual, o null si viene vacío.</summary>
    public static async Task<T?> LeerUnoAsync<T>(
        this DbDataReader lector, Func<DbDataReader, T> mapear, CancellationToken ct = default)
        where T : class
    {
        return await lector.ReadAsync(ct) ? mapear(lector) : null;
    }
}
