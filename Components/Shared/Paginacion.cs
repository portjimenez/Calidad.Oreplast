namespace calidad_app.Components.Shared;

/// <summary>
/// Paginación en cliente de las tablas maestras (10 filas por página).
///
/// Es en cliente y no en la base a propósito: los procedimientos de listado ya
/// acotan el resultado y ninguna de estas consultas devuelve más de unas decenas
/// de filas, así que paginar en el servidor obligaría a una consulta por página
/// sin ganar nada. Si alguna lista llegara a crecer, el cambio queda contenido
/// aquí y en su procedimiento, no en las pantallas.
///
/// La clase vive aparte de <see cref="Paginador"/> para que la lógica (qué filas
/// tocan, en qué página está el elemento abierto) se pruebe y se corrija en un
/// solo lugar, y el componente se ocupe únicamente de dibujar los controles.
/// </summary>
public sealed class Paginacion<T>
{
    /// <summary>Filas visibles por página en el maestro.</summary>
    public const int TamanoPagina = 10;

    private List<T> _elementos = [];

    /// <summary>Página actual, siempre entre 1 y <see cref="TotalPaginas"/>.</summary>
    public int Pagina { get; private set; } = 1;

    /// <summary>Total de filas de la lista completa, no de la página.</summary>
    public int Total => _elementos.Count;

    /// <summary>Nunca es cero: una lista vacía sigue siendo "página 1 de 1".</summary>
    public int TotalPaginas => Math.Max(1, (int)Math.Ceiling(Total / (double)TamanoPagina));

    /// <summary>Las filas que toca dibujar en la página actual.</summary>
    public IEnumerable<T> Elementos =>
        _elementos.Skip((Pagina - 1) * TamanoPagina).Take(TamanoPagina);

    /// <summary>Texto del rango mostrado, por ejemplo "11–14 de 14".</summary>
    public string Rango
    {
        get
        {
            if (Total == 0)
            {
                return "Sin filas";
            }

            var desde = ((Pagina - 1) * TamanoPagina) + 1;
            var hasta = Math.Min(Pagina * TamanoPagina, Total);
            return $"{desde}–{hasta} de {Total}";
        }
    }

    /// <summary>
    /// Carga una lista nueva y vuelve a la primera página. Es lo que corresponde
    /// cuando cambia un filtro o una búsqueda: la página 3 del resultado anterior
    /// no significa nada en el resultado nuevo.
    /// </summary>
    public void Reiniciar(List<T>? elementos)
    {
        _elementos = elementos ?? [];
        Pagina = 1;
    }

    /// <summary>
    /// Recarga la lista conservando la posición del usuario. Si se le indica cuál
    /// es el elemento abierto en el detalle, se coloca en la página donde quedó:
    /// después de guardar algo, la fila que se estaba viendo tiene que seguir a la
    /// vista aunque el orden de la lista haya cambiado.
    /// </summary>
    public void Actualizar(List<T>? elementos, Predicate<T>? seleccionado = null)
    {
        _elementos = elementos ?? [];

        if (seleccionado is not null)
        {
            var indice = _elementos.FindIndex(seleccionado);
            if (indice >= 0)
            {
                Pagina = (indice / TamanoPagina) + 1;
                return;
            }
        }

        Pagina = Math.Clamp(Pagina, 1, TotalPaginas);
    }

    public void IrAPagina(int pagina) => Pagina = Math.Clamp(pagina, 1, TotalPaginas);
}
