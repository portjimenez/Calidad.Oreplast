// Impresión tabular de cualquier pantalla (botón Imprimir de la barra de herramientas).
//
// Imprimir la pantalla tal como se ve equivale a una captura: tarjetas, columnas
// lado a lado, chips y texto apilado dentro de las celdas. Aquí se arma, a partir
// de lo que la pantalla muestra, un documento aparte con los datos en tablas con
// cuadrícula (al estilo de una hoja de Excel), se imprime y se descarta.
//
// - Los datos ya están en la pantalla con sus filtros aplicados: no se consulta al
//   servidor y lo impreso coincide con lo que el usuario ve.
// - El documento se agrega a <body>, fuera de lo que dibuja Blazor, y se retira al
//   cerrar el diálogo: no se toca el DOM que Blazor administra.
// - Se recorre el contenido en orden y se reconoce:
//     encabezados (h1-h4)   -> título de la sección que sigue
//     tarjetas (.tarjeta, .kpi) -> tabla "Indicador | Valor | Detalle"
//     tablas                -> la misma tabla, con el texto de cada celda en una línea
//     campos de formulario  -> tabla "Campo | Valor" (solo si no hubo nada de lo anterior)
// - Se omiten botones, gráficas (SVG), lo oculto y lo marcado con data-imprimir="no".
// - Un certificado abierto se imprime como siempre: ya es un documento con su
//   propio formato (css/impresion.css, sección Certificado).

const ID_DOCUMENTO = "impresion-tabular";
const SEPARADOR = " · ";

export function imprimir() {
    retirarDocumento();

    const contenido = document.querySelector(".app-contenido");

    if (!contenido || document.getElementById("certificado-imprimible")) {
        lanzarImpresion();
        return;
    }

    const lectura = leerPantalla(contenido);
    const bloques = lectura.tablas.length > 0
        ? lectura.tablas
        : lectura.campos;

    // Sin nada que convertir en tabla se imprime la pantalla como se ve.
    if (bloques.length === 0) {
        lanzarImpresion();
        return;
    }

    document.body.appendChild(construirDocumento(lectura.titulo, lectura.subtitulo, bloques));
    window.addEventListener("afterprint", retirarDocumento, { once: true });
    lanzarImpresion();
}

// window.print() detiene el script hasta que se cierra el diálogo, y la llamada
// desde Blazor Server caduca al minuto: con el diálogo abierto más tiempo, la
// barra recibiría un error. Se difiere para que la llamada regrese enseguida.
function lanzarImpresion() {
    setTimeout(() => window.print(), 0);
}

function retirarDocumento() {
    document.getElementById(ID_DOCUMENTO)?.remove();
}

// ---------------------------------------------------------------- Lectura

function leerPantalla(contenido) {
    const lectura = { titulo: "", subtitulo: "", tablas: [], campos: [] };

    let encabezado = null;      // último encabezado visto, aún sin usar
    let tarjetas = null;        // bloque de tarjetas en curso
    let padreTarjetas = null;
    let campos = null;          // bloque de campos en curso

    const recorrer = (elemento) => {
        for (const hijo of elemento.children) {
            if (debeOmitirse(hijo)) {
                continue;
            }

            if (/^H[1-4]$/.test(hijo.tagName) || hijo.tagName === "LEGEND") {
                const texto = normalizar(hijo.textContent);

                if (hijo.tagName === "H1" && !lectura.titulo) {
                    lectura.titulo = texto;
                    const siguiente = hijo.nextElementSibling;
                    if (siguiente?.tagName === "P" && !debeOmitirse(siguiente)) {
                        lectura.subtitulo = normalizar(siguiente.textContent);
                    }
                } else if (texto) {
                    encabezado = texto;
                    campos = null;
                }
                continue;
            }

            if (hijo.matches(".tarjeta, .kpi")) {
                if (!tarjetas || padreTarjetas !== hijo.parentElement) {
                    tarjetas = {
                        titulo: tomar() ?? "Resumen",
                        encabezados: ["Indicador", "Valor", "Detalle"],
                        filas: [],
                        numericas: [false, true, false]
                    };
                    padreTarjetas = hijo.parentElement;
                    lectura.tablas.push(tarjetas);
                }
                tarjetas.filas.push(leerTarjeta(hijo));
                continue;
            }

            if (hijo.tagName === "TABLE") {
                const tabla = leerTabla(hijo);
                if (tabla.filas.length > 0 || tabla.encabezados.length > 0) {
                    tabla.titulo = normalizar(hijo.querySelector(":scope > caption")?.textContent) || tomar();
                    lectura.tablas.push(tabla);
                }
                continue;
            }

            if (hijo.tagName === "LABEL") {
                const campo = leerCampo(hijo);
                if (campo) {
                    if (!campos) {
                        campos = {
                            titulo: tomar(),
                            encabezados: ["Campo", "Valor"],
                            filas: [],
                            numericas: [false, false]
                        };
                        lectura.campos.push(campos);
                    }
                    campos.filas.push(campo);
                    continue;
                }
            }

            recorrer(hijo);
        }
    };

    // El encabezado se usa una sola vez: la segunda tabla bajo el mismo título
    // sale sin título en vez de repetirlo.
    const tomar = () => {
        const texto = encabezado;
        encabezado = null;
        return texto;
    };

    recorrer(contenido);

    if (!lectura.titulo) {
        lectura.titulo = document.title;
    }

    return lectura;
}

function debeOmitirse(elemento) {
    if (elemento.matches("button, svg, script, style, template, nav, .visually-hidden, .sr-only, [data-imprimir='no']")) {
        return true;
    }

    return typeof elemento.checkVisibility === "function"
        ? !elemento.checkVisibility()
        : elemento.offsetParent === null && getComputedStyle(elemento).position !== "fixed";
}

function leerTarjeta(tarjeta) {
    const titulo = tarjeta.querySelector("[class*='__titulo']");
    const valor = tarjeta.querySelector("[class*='__valor']");
    const resto = [...tarjeta.children]
        .filter(hijo => hijo !== titulo && hijo !== valor && !debeOmitirse(hijo))
        .map(textoDe)
        .filter(texto => texto);

    return [
        { texto: textoDe(titulo) },
        { texto: textoDe(valor) },
        { texto: resto.join(SEPARADOR) }
    ];
}

function leerTabla(tabla) {
    const filasVisibles = [...tabla.rows].filter(fila => !debeOmitirse(fila));
    const enEncabezado = fila => fila.parentElement?.tagName === "THEAD";

    let filasEncabezado = filasVisibles.filter(enEncabezado);
    let filasCuerpo = filasVisibles.filter(fila => !enEncabezado(fila));

    // Tabla sin <thead>: la primera fila hace de encabezado si es toda de <th>.
    if (filasEncabezado.length === 0 && filasCuerpo.length > 0
        && [...filasCuerpo[0].cells].every(celda => celda.tagName === "TH")) {
        filasEncabezado = [filasCuerpo[0]];
        filasCuerpo = filasCuerpo.slice(1);
    }

    const leerFila = fila => [...fila.cells].map(celda => ({
        texto: debeOmitirse(celda) ? "" : textoDe(celda),
        colspan: celda.colSpan,
        rowspan: celda.rowSpan,
        numerica: getComputedStyle(celda).textAlign === "right"
    }));

    const resultado = {
        encabezadosMultiples: filasEncabezado.map(leerFila),
        filas: filasCuerpo.map(leerFila)
    };

    quitarColumnasVacias(resultado);
    resultado.encabezados = resultado.encabezadosMultiples.length === 1
        ? resultado.encabezadosMultiples[0]
        : [];

    return resultado;
}

// Las columnas de acciones (solo botones) quedan vacías en papel. Se quitan
// cuando la tabla es una cuadrícula simple; con celdas combinadas no se puede
// saber con certeza a qué columna pertenece cada una, y se deja intacta.
function quitarColumnasVacias(tabla) {
    const todas = [...tabla.encabezadosMultiples, ...tabla.filas];
    if (todas.length === 0 || todas.some(fila => fila.some(c => c.colspan > 1 || c.rowspan > 1))) {
        return;
    }

    const columnas = Math.max(...todas.map(fila => fila.length));
    const vacias = [];
    for (let i = 0; i < columnas; i++) {
        if (todas.every(fila => !fila[i]?.texto)) {
            vacias.push(i);
        }
    }

    if (vacias.length > 0 && vacias.length < columnas) {
        const filtrar = fila => fila.filter((_, i) => !vacias.includes(i));
        tabla.encabezadosMultiples = tabla.encabezadosMultiples.map(filtrar);
        tabla.filas = tabla.filas.map(filtrar);
    }
}

function leerCampo(etiqueta) {
    const control = etiqueta.querySelector("input, select, textarea")
        ?? (etiqueta.htmlFor ? document.getElementById(etiqueta.htmlFor) : null);

    // Un valor calculado se muestra como texto dentro de la etiqueta.
    const calculado = control ? null : etiqueta.querySelector("[class*='valor']");

    if ((!control && !calculado) || control?.type === "hidden") {
        return null;
    }

    let nombre = "";
    for (const nodo of etiqueta.childNodes) {
        if (nodo.nodeType === Node.TEXT_NODE) {
            nombre += nodo.textContent;
        } else if (nodo !== control && nodo !== calculado && nodo.nodeType === Node.ELEMENT_NODE
            && !nodo.contains(control) && !nodo.contains(calculado) && !debeOmitirse(nodo)) {
            nombre += " " + nodo.textContent;
        }
    }

    const valor = control ? valorDeControl(control) : textoDe(calculado);
    return [{ texto: normalizar(nombre) }, { texto: valor }];
}

function valorDeControl(control) {
    if (control.tagName === "SELECT") {
        return normalizar([...control.selectedOptions].map(o => o.text).join(", "));
    }
    if (control.type === "checkbox" || control.type === "radio") {
        return control.checked ? "Sí" : "No";
    }
    return normalizar(control.value);
}

// Texto de una celda en una sola línea. Lo que en pantalla va en su propio
// renglón (el dato secundario debajo del principal, un chip junto al texto) se
// une con " · "; lo que va en línea (negritas, enlaces) sigue siendo parte del
// mismo trozo de texto.
function textoDe(elemento) {
    if (!elemento) {
        return "";
    }

    const trozos = [];
    let actual = "";

    const cerrar = () => {
        const texto = normalizar(actual);
        if (texto) {
            trozos.push(texto);
        }
        actual = "";
    };

    const recorrer = (nodo) => {
        for (const hijo of nodo.childNodes) {
            if (hijo.nodeType === Node.TEXT_NODE) {
                actual += hijo.textContent;
                continue;
            }
            if (hijo.nodeType !== Node.ELEMENT_NODE || debeOmitirse(hijo)) {
                continue;
            }
            if (hijo.matches("input, select, textarea")) {
                actual += " " + valorDeControl(hijo) + " ";
                continue;
            }
            if (esTrozoPropio(hijo)) {
                cerrar();
                recorrer(hijo);
                cerrar();
            } else {
                recorrer(hijo);
            }
        }
    };

    recorrer(elemento);
    cerrar();

    return trozos.join(SEPARADOR);
}

function esTrozoPropio(elemento) {
    if (elemento.tagName === "BR" || elemento.matches("[class*='secundario'], [class*='chip']")) {
        return true;
    }
    const display = getComputedStyle(elemento).display;
    return display !== "inline" && display !== "contents";
}

function normalizar(texto) {
    return (texto ?? "").replace(/\s+/g, " ").trim();
}

// ---------------------------------------------------------------- Documento

function construirDocumento(titulo, subtitulo, bloques) {
    const documento = crear("div");
    documento.id = ID_DOCUMENTO;

    const cabecera = crear("header", "it-cabecera");
    cabecera.append(
        crear("div", "it-empresa", "Oreplast S.A. · Sistema de Control de Calidad"),
        crear("h1", "it-titulo", titulo));
    if (subtitulo) {
        cabecera.append(crear("p", "it-subtitulo", subtitulo));
    }
    cabecera.append(crear("p", "it-fecha", "Impreso el " + new Date().toLocaleString("es-GT", {
        day: "2-digit", month: "2-digit", year: "numeric", hour: "2-digit", minute: "2-digit"
    })));
    documento.append(cabecera);

    for (const bloque of bloques) {
        const seccion = crear("section", "it-bloque");
        if (bloque.titulo) {
            seccion.append(crear("h2", "it-bloque__titulo", bloque.titulo));
        }
        seccion.append(construirTabla(bloque));
        documento.append(seccion);
    }

    return documento;
}

function construirTabla(bloque) {
    const tabla = crear("table", "it-tabla");

    // Tarjetas y campos traen encabezados de texto; las tablas leídas de la
    // pantalla traen celdas (con alineación y celdas combinadas).
    const filasEncabezado = bloque.encabezadosMultiples
        ?? [bloque.encabezados.map((texto, i) => ({ texto, numerica: bloque.numericas[i] }))];

    if (filasEncabezado.length > 0) {
        const thead = crear("thead");
        for (const fila of filasEncabezado) {
            thead.append(construirFila(fila, "th"));
        }
        tabla.append(thead);
    }

    const tbody = crear("tbody");
    for (const fila of bloque.filas) {
        const celdas = bloque.numericas
            ? fila.map((celda, i) => ({ ...celda, numerica: bloque.numericas[i] }))
            : fila;
        tbody.append(construirFila(celdas, "td"));
    }
    tabla.append(tbody);

    return tabla;
}

function construirFila(celdas, etiqueta) {
    const tr = crear("tr");
    for (const celda of celdas) {
        const elemento = crear(etiqueta, celda.numerica ? "it-num" : null, celda.texto);
        if (celda.colspan > 1) {
            elemento.colSpan = celda.colspan;
        }
        if (celda.rowspan > 1) {
            elemento.rowSpan = celda.rowspan;
        }
        tr.append(elemento);
    }
    return tr;
}

// textContent y no innerHTML: lo que se copia de la pantalla es texto, nunca marcado.
function crear(etiqueta, clase, texto) {
    const elemento = document.createElement(etiqueta);
    if (clase) {
        elemento.className = clase;
    }
    if (texto) {
        elemento.textContent = texto;
    }
    return elemento;
}
