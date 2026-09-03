// Descarga de archivos desde Blazor Server.
//
// El binario viaja por el circuito de SignalR como stream y aquí se convierte
// en una descarga del navegador. Se hace así, y no con un endpoint HTTP, porque
// dentro del circuito la identidad del usuario ya está resuelta: el
// procedimiento que entrega el archivo registra en la bitácora quién lo
// descargó, y eso solo es fiable si la llamada sale del componente autorizado.
export async function descargarArchivo(nombreArchivo, streamRef) {
    const buffer = await streamRef.arrayBuffer();
    const url = URL.createObjectURL(new Blob([buffer]));

    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.download = nombreArchivo ?? 'archivo';
    document.body.appendChild(enlace);
    enlace.click();
    document.body.removeChild(enlace);

    // Sin esto el blob queda retenido en memoria mientras dure la pestaña.
    URL.revokeObjectURL(url);
}
