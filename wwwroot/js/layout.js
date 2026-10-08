// Funciones del layout principal (MainLayout).

// Lleva al principio el área de contenido. Blazor regresa arriba la ventana al
// navegar, pero en esta aplicación la ventana no se desplaza (el menú es fijo):
// el que se desplaza es el contenido, y ese hay que regresarlo a mano.
export function irArriba(elemento) {
    if (elemento) {
        elemento.scrollTop = 0;
    }
}
