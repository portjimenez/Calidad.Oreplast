using calidad_app.Models.Reportes;
using ClosedXML.Excel;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace calidad_app.Services.Reportes;

/// <summary>
/// Convierte un reporte ya armado (<see cref="TablaReporte"/>) en un archivo
/// de Excel o de PDF.
///
/// Es uno solo para los cuatro reportes, y por eso recibe la tabla genérica en
/// lugar de las filas tipadas: el exportador sabe de tipos de columna (texto,
/// entero, porcentaje, fecha) pero no sabe qué es una bobina ni una no
/// conformidad. Agregar un quinto reporte no toca este archivo.
///
/// En el Excel los números viajan como NÚMEROS, no como texto: quien recibe el
/// archivo tiene que poder sumar una columna o hacer una tabla dinámica sin
/// limpiar nada primero. El formato (miles, decimales, fecha) se aplica como
/// formato de celda.
/// </summary>
public class ExportadorArchivos
{
    /// <summary>Azul del tema de la aplicación, para el encabezado de las tablas.</summary>
    private static readonly XLColor AzulExcel = XLColor.FromArgb(0x1F, 0x6F, 0xB2);
    private static readonly Color AzulPdf = new(0x1F, 0x6F, 0xB2);

    public const string TipoExcel =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public const string TipoPdf = "application/pdf";

    /// <summary>
    /// PDFsharp, en su compilación portable, no trae fuentes: hay que decirle
    /// que use las de Windows, que es donde corre la aplicación (IIS de la
    /// planta). Se hace una sola vez por proceso.
    /// </summary>
    private static readonly Lazy<bool> FuentesListas = new(() =>
    {
        GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        return true;
    });

    public ArchivoGenerado Generar(TablaReporte tabla, FormatoArchivo formato, string nombreBase)
    {
        var marca = DateTime.Now.ToString("yyyyMMdd-HHmm");

        return formato == FormatoArchivo.Excel
            ? new ArchivoGenerado($"{nombreBase}-{marca}.xlsx", TipoExcel, Excel(tabla))
            : new ArchivoGenerado($"{nombreBase}-{marca}.pdf", TipoPdf, Pdf(tabla));
    }

    /* ------------------------------------------------------------------
       Excel
       ------------------------------------------------------------------ */
    private static byte[] Excel(TablaReporte tabla)
    {
        using var libro = new XLWorkbook();
        var hoja = libro.AddWorksheet(NombreDeHoja(tabla.Titulo));

        var columnas = tabla.Columnas.Count;

        // Encabezado del archivo: título y ámbito, para que la hoja se explique
        // sola cuando circule por correo lejos de la aplicación.
        hoja.Cell(1, 1).Value = tabla.Titulo;
        hoja.Cell(1, 1).Style.Font.Bold = true;
        hoja.Cell(1, 1).Style.Font.FontSize = 14;

        hoja.Cell(2, 1).Value = tabla.Subtitulo;
        hoja.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;

        if (columnas > 1)
        {
            hoja.Range(1, 1, 1, columnas).Merge();
            hoja.Range(2, 1, 2, columnas).Merge();
        }

        const int filaEncabezado = 4;

        for (var c = 0; c < columnas; c++)
        {
            var celda = hoja.Cell(filaEncabezado, c + 1);
            celda.Value = tabla.Columnas[c].Titulo;
            celda.Style.Font.Bold = true;
            celda.Style.Font.FontColor = XLColor.White;
            celda.Style.Fill.BackgroundColor = AzulExcel;
            celda.Style.Alignment.WrapText = true;
        }

        for (var f = 0; f < tabla.Filas.Count; f++)
        {
            var fila = tabla.Filas[f];

            for (var c = 0; c < columnas && c < fila.Length; c++)
            {
                var celda = hoja.Cell(filaEncabezado + 1 + f, c + 1);
                EscribirValor(celda, fila[c]);

                var formato = FormatoCelda.FormatoExcel(tabla.Columnas[c].Tipo);
                if (formato is not null)
                {
                    celda.Style.NumberFormat.Format = formato;
                }
            }
        }

        var ultimaFila = filaEncabezado + tabla.Filas.Count;

        if (tabla.Filas.Count > 0)
        {
            // Autofiltro y paneles congelados: con varios miles de filas, sin
            // esto lo primero que hace quien abre el archivo es ponerlos.
            hoja.Range(filaEncabezado, 1, ultimaFila, columnas).SetAutoFilter();
            hoja.SheetView.FreezeRows(filaEncabezado);
        }

        if (!string.IsNullOrWhiteSpace(tabla.Nota))
        {
            var celdaNota = hoja.Cell(ultimaFila + 2, 1);
            celdaNota.Value = tabla.Nota;
            celdaNota.Style.Font.Italic = true;
            celdaNota.Style.Font.FontColor = XLColor.Gray;
        }

        hoja.Columns().AdjustToContents(5d, 45d);

        using var memoria = new MemoryStream();
        libro.SaveAs(memoria);
        return memoria.ToArray();
    }

    /// <summary>
    /// Escribe el valor conservando su tipo. Un DateOnly se convierte a
    /// DateTime porque Excel no distingue fecha de fecha con hora: la
    /// diferencia la hace el formato de la celda.
    /// </summary>
    private static void EscribirValor(IXLCell celda, object? valor)
    {
        switch (valor)
        {
            case null:
                break;
            case bool booleano:
                celda.Value = booleano ? "Sí" : "No";
                break;
            case DateOnly fecha:
                celda.Value = fecha.ToDateTime(TimeOnly.MinValue);
                break;
            case DateTime fechaHora:
                celda.Value = fechaHora;
                break;
            case decimal numero:
                celda.Value = numero;
                break;
            case int entero:
                celda.Value = entero;
                break;
            default:
                celda.Value = valor.ToString();
                break;
        }
    }

    /// <summary>Excel no admite más de 31 caracteres ni ciertos símbolos en el nombre de la hoja.</summary>
    private static string NombreDeHoja(string titulo)
    {
        var limpio = new string(titulo.Where(c => !"[]:*?/\\".Contains(c)).ToArray());
        return limpio.Length <= 31 ? limpio : limpio[..31];
    }

    /* ------------------------------------------------------------------
       PDF
       ------------------------------------------------------------------ */
    private static byte[] Pdf(TablaReporte tabla)
    {
        _ = FuentesListas.Value;

        var documento = new Document();
        documento.Styles[StyleNames.Normal]!.Font.Name = "Arial";
        documento.Styles[StyleNames.Normal]!.Font.Size = 7;

        var seccion = documento.AddSection();

        // Horizontal: estos reportes tienen muchas columnas y en vertical
        // saldrían ilegibles.
        seccion.PageSetup.Orientation = Orientation.Landscape;
        seccion.PageSetup.PageFormat = PageFormat.A4;
        seccion.PageSetup.LeftMargin = Unit.FromCentimeter(1.2);
        seccion.PageSetup.RightMargin = Unit.FromCentimeter(1.2);
        seccion.PageSetup.TopMargin = Unit.FromCentimeter(1.2);
        seccion.PageSetup.BottomMargin = Unit.FromCentimeter(1.2);

        var titulo = seccion.AddParagraph(tabla.Titulo);
        titulo.Format.Font.Size = 14;
        titulo.Format.Font.Bold = true;
        titulo.Format.Font.Color = AzulPdf;

        var subtitulo = seccion.AddParagraph(tabla.Subtitulo);
        subtitulo.Format.Font.Size = 8;
        subtitulo.Format.Font.Color = Colors.Gray;
        subtitulo.Format.SpaceAfter = Unit.FromCentimeter(0.3);

        AgregarTabla(seccion, tabla);

        if (!string.IsNullOrWhiteSpace(tabla.Nota))
        {
            var nota = seccion.AddParagraph(tabla.Nota);
            nota.Format.Font.Size = 7;
            nota.Format.Font.Italic = true;
            nota.Format.Font.Color = Colors.Gray;
            nota.Format.SpaceBefore = Unit.FromCentimeter(0.3);
        }

        PieDePagina(seccion);

        var renderizador = new PdfDocumentRenderer { Document = documento };
        renderizador.RenderDocument();

        using var memoria = new MemoryStream();
        renderizador.PdfDocument.Save(memoria, false);
        return memoria.ToArray();
    }

    private static void AgregarTabla(Section seccion, TablaReporte tabla)
    {
        if (tabla.Columnas.Count == 0)
        {
            return;
        }

        var migra = seccion.AddTable();
        migra.Borders.Width = 0.25;
        migra.Borders.Color = Colors.LightGray;
        migra.TopPadding = 1.5;
        migra.BottomPadding = 1.5;
        migra.LeftPadding = 2;
        migra.RightPadding = 2;

        // Los anchos declarados por el reporte se escalan al ancho útil de la
        // página, para que la tabla nunca se salga ni deje media hoja vacía.
        var util = seccion.PageSetup.PageWidth.Centimeter
                 - seccion.PageSetup.LeftMargin.Centimeter
                 - seccion.PageSetup.RightMargin.Centimeter;

        var declarado = tabla.Columnas.Sum(c => c.AnchoCm);
        var factor = declarado > 0 ? util / declarado : 1;

        foreach (var columna in tabla.Columnas)
        {
            var nueva = migra.AddColumn(Unit.FromCentimeter(columna.AnchoCm * factor));
            nueva.Format.Alignment = FormatoCelda.AlineadaDerecha(columna.Tipo)
                ? ParagraphAlignment.Right
                : ParagraphAlignment.Left;
        }

        var encabezado = migra.AddRow();
        encabezado.HeadingFormat = true;   // se repite en cada página
        encabezado.Shading.Color = AzulPdf;
        encabezado.Format.Font.Bold = true;
        encabezado.Format.Font.Color = Colors.White;

        for (var c = 0; c < tabla.Columnas.Count; c++)
        {
            encabezado.Cells[c].AddParagraph(tabla.Columnas[c].Titulo);
        }

        for (var f = 0; f < tabla.Filas.Count; f++)
        {
            var fila = migra.AddRow();

            // Filas alternas sombreadas: con doce columnas, seguir una fila
            // entera a lo ancho sin esto es difícil.
            if (f % 2 == 1)
            {
                fila.Shading.Color = new Color(0xF4, 0xF7, 0xFA);
            }

            var valores = tabla.Filas[f];

            for (var c = 0; c < tabla.Columnas.Count && c < valores.Length; c++)
            {
                var texto = FormatoCelda.Texto(valores[c], tabla.Columnas[c].Tipo);
                fila.Cells[c].AddParagraph(texto);
            }
        }
    }

    private static void PieDePagina(Section seccion)
    {
        var pie = seccion.Footers.Primary.AddParagraph();
        pie.Format.Font.Size = 7;
        pie.Format.Font.Color = Colors.Gray;
        pie.Format.Alignment = ParagraphAlignment.Center;
        pie.AddText("Oreplast S.A. · Sistema de Control de Calidad · Generado el ");
        pie.AddDateField("dd/MM/yyyy HH:mm");
        pie.AddText("  ·  Página ");
        pie.AddPageField();
        pie.AddText(" de ");
        pie.AddNumPagesField();
    }
}
