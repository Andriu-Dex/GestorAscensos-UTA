// Script para descargar archivos (principalmente reportes PDF)
window.downloadFileFromStream = (fileName, contentBytes) => {
  // Crear un blob con los bytes recibidos del servidor
  const blob = new Blob([new Uint8Array(contentBytes)], {
    type: "application/octet-stream",
  });

  // Crear URL para el blob
  const url = URL.createObjectURL(blob);

  // Crear un elemento <a> temporal para iniciar la descarga
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;

  // Agregar el elemento al DOM (no visible)
  document.body.appendChild(link);

  // Iniciar la descarga
  link.click();

  // Limpiar - remover el elemento <a> y revocar la URL
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
};
