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

// Función para descargar archivos usando base64
window.downloadFile = (base64String, fileName, contentType) => {
  // Decodificar base64 a bytes
  const byteCharacters = atob(base64String);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);

  // Crear blob
  const blob = new Blob([byteArray], { type: contentType });

  // Crear URL y descargar
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
};

// Función para abrir PDF en nueva pestaña
window.openPdfInNewTab = (dataUrl) => {
  window.open(dataUrl, "_blank");
};

// Función para abrir PDF en modal (usando un evento personalizado)
window.openPdfModal = (dataUrl) => {
  // Disparar un evento personalizado que será capturado por Blazor
  window.dispatchEvent(
    new CustomEvent("openPdfModal", {
      detail: { pdfUrl: dataUrl },
    })
  );
};
