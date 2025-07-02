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
window.downloadFile = (
  fileName,
  base64String,
  contentType = "application/pdf"
) => {
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

// Función para visualizar PDF en un modal
window.showPdfInModal = (base64String, title = "Visualizar PDF") => {
  // Crear un div para el modal
  const modalDiv = document.createElement("div");
  modalDiv.className = "pdf-modal";
  modalDiv.style.position = "fixed";
  modalDiv.style.top = "0";
  modalDiv.style.left = "0";
  modalDiv.style.width = "100%";
  modalDiv.style.height = "100%";
  modalDiv.style.backgroundColor = "rgba(0,0,0,0.7)";
  modalDiv.style.zIndex = "10000";
  modalDiv.style.display = "flex";
  modalDiv.style.flexDirection = "column";
  modalDiv.style.justifyContent = "center";
  modalDiv.style.alignItems = "center";

  // Crear header del modal
  const modalHeader = document.createElement("div");
  modalHeader.style.backgroundColor = "#8a1538";
  modalHeader.style.color = "white";
  modalHeader.style.width = "90%";
  modalHeader.style.padding = "10px";
  modalHeader.style.display = "flex";
  modalHeader.style.justifyContent = "space-between";
  modalHeader.style.alignItems = "center";
  modalHeader.style.borderTopLeftRadius = "5px";
  modalHeader.style.borderTopRightRadius = "5px";

  // Título del modal
  const modalTitle = document.createElement("h5");
  modalTitle.innerText = title;
  modalTitle.style.margin = "0";
  modalHeader.appendChild(modalTitle);

  // Botón de cerrar
  const closeButton = document.createElement("button");
  closeButton.innerHTML = "&times;";
  closeButton.style.backgroundColor = "transparent";
  closeButton.style.border = "none";
  closeButton.style.color = "white";
  closeButton.style.fontSize = "1.5rem";
  closeButton.style.cursor = "pointer";
  closeButton.onclick = () => {
    document.body.removeChild(modalDiv);
  };
  modalHeader.appendChild(closeButton);

  // Crear contenedor del PDF
  const modalContent = document.createElement("div");
  modalContent.style.backgroundColor = "white";
  modalContent.style.width = "90%";
  modalContent.style.height = "80%";
  modalContent.style.padding = "0";
  modalContent.style.overflow = "hidden";

  // Agregar el objeto PDF
  const pdfObject = document.createElement("object");
  pdfObject.type = "application/pdf";
  pdfObject.style.width = "100%";
  pdfObject.style.height = "100%";

  // Crear blob del PDF a partir de base64
  const byteCharacters = atob(base64String);
  const byteArray = new Uint8Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteArray[i] = byteCharacters.charCodeAt(i);
  }
  const blob = new Blob([byteArray], { type: "application/pdf" });
  const url = URL.createObjectURL(blob);

  pdfObject.data = url;
  modalContent.appendChild(pdfObject);

  // Agregar los elementos al modal
  modalDiv.appendChild(modalHeader);
  modalDiv.appendChild(modalContent);

  // Agregar el modal al body
  document.body.appendChild(modalDiv);

  // Limpieza cuando se cierra el modal
  return () => {
    URL.revokeObjectURL(url);
  };
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
