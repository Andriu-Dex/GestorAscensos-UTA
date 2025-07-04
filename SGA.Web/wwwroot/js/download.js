// Script para descargar archivos (principalmente reportes PDF)
window.downloadFileFromStream = async (fileName, contentBytes) => {
  try {
    // Crear un blob con los bytes recibidos del servidor
    const blob = new Blob([new Uint8Array(contentBytes)], {
      type: "application/octet-stream",
    });

    // Verificar si el navegador soporta la File System Access API
    if ("showSaveFilePicker" in window) {
      try {
        // Usar la API moderna para mostrar el explorador de archivos
        const fileHandle = await window.showSaveFilePicker({
          suggestedName: fileName,
          types: [
            {
              description: "Archivos PDF",
              accept: { "application/pdf": [".pdf"] },
            },
          ],
        });

        // Escribir el archivo
        const writable = await fileHandle.createWritable();
        await writable.write(blob);
        await writable.close();

        console.log("Archivo guardado exitosamente");
      } catch (err) {
        // Si el usuario cancela o hay error, usar método fallback
        if (err.name !== "AbortError") {
          console.log("Usando método fallback debido a:", err);
          downloadFileDirectly(blob, fileName);
        }
      }
    } else {
      // Fallback para navegadores que no soportan File System Access API
      downloadFileDirectly(blob, fileName);
    }
  } catch (error) {
    console.error("Error al descargar archivo:", error);
  }
};

// Función para descargar archivos usando base64 con explorador de archivos
window.downloadFileFromBase64 = async (
  base64String,
  fileName,
  contentType = "application/pdf"
) => {
  try {
    // Limpiar el string base64
    let cleanBase64 = base64String;

    // Si el string contiene el prefijo data:, removerlo
    if (cleanBase64.startsWith("data:")) {
      const commaIndex = cleanBase64.indexOf(",");
      if (commaIndex !== -1) {
        cleanBase64 = cleanBase64.substring(commaIndex + 1);
      }
    }

    // Limpiar caracteres de nueva línea y espacios
    cleanBase64 = cleanBase64.replace(/\s/g, "");

    // Verificar que el string base64 sea válido
    if (!cleanBase64 || cleanBase64.length % 4 !== 0) {
      console.error("String base64 inválido");
      return;
    }

    // Convertir base64 a bytes
    const byteCharacters = atob(cleanBase64);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);

    // Crear blob
    const blob = new Blob([byteArray], { type: contentType });

    // Verificar si el navegador soporta la File System Access API
    if ("showSaveFilePicker" in window) {
      try {
        // Usar la API moderna para mostrar el explorador de archivos
        const fileHandle = await window.showSaveFilePicker({
          suggestedName: fileName,
          types: [
            {
              description: "Archivos PDF",
              accept: { "application/pdf": [".pdf"] },
            },
          ],
        });

        // Escribir el archivo
        const writable = await fileHandle.createWritable();
        await writable.write(blob);
        await writable.close();

        console.log("Archivo guardado exitosamente");
      } catch (err) {
        // Si el usuario cancela o hay error, usar método fallback
        if (err.name !== "AbortError") {
          console.log("Usando método fallback debido a:", err);
          downloadFileDirectly(blob, fileName);
        }
      }
    } else {
      // Fallback para navegadores que no soportan File System Access API
      downloadFileDirectly(blob, fileName);
    }
  } catch (error) {
    console.error("Error al descargar archivo:", error);
  }
};

// Función auxiliar para descarga directa (fallback)
function downloadFileDirectly(blob, fileName) {
  // Crear URL para el blob
  const url = URL.createObjectURL(blob);

  // Crear elemento <a> temporal para descargar
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;

  // Agregar al DOM, hacer clic y limpiar
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
}

// Función legacy para compatibilidad
window.downloadFile = (
  fileName,
  base64String,
  contentType = "application/pdf"
) => {
  window.downloadFileFromBase64(base64String, fileName, contentType);
};

// Función para visualizar PDF en un modal
window.showPdfInModal = (base64String, title = "Visualizar PDF") => {
  try {
    // Limpiar el string base64
    let cleanBase64 = base64String;

    // Si el string contiene el prefijo data:, removerlo
    if (cleanBase64.startsWith("data:")) {
      const commaIndex = cleanBase64.indexOf(",");
      if (commaIndex !== -1) {
        cleanBase64 = cleanBase64.substring(commaIndex + 1);
      }
    }

    // Limpiar caracteres de nueva línea y espacios
    cleanBase64 = cleanBase64.replace(/\s/g, "");

    // Verificar que el string base64 sea válido
    if (!cleanBase64 || cleanBase64.length % 4 !== 0) {
      console.error("String base64 inválido para visualización");
      throw new Error("El archivo no está correctamente codificado");
    }

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
    const byteCharacters = atob(cleanBase64);
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
  } catch (error) {
    console.error("Error al mostrar PDF en modal:", error);
  }
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

// Función para descargar desde data URL
window.downloadFromDataUrl = (dataUrl, fileName) => {
  try {
    // Crear un elemento <a> temporal para iniciar la descarga
    const link = document.createElement("a");
    link.href = dataUrl;
    link.download = fileName;

    // Agregar el elemento al DOM (no visible)
    document.body.appendChild(link);

    // Iniciar la descarga
    link.click();

    // Limpiar - remover el elemento
    document.body.removeChild(link);
  } catch (error) {
    console.error("Error al descargar desde data URL:", error);
    if (typeof showToast === "function") {
      showToast("Error al descargar el archivo: " + error.message, "error");
    }
  }
};

// Función para mostrar vista previa de reportes HTML en modal
window.showReportPreview = (
  htmlContent,
  title = "Vista Previa del Reporte"
) => {
  try {
    // Crear un div para el modal
    const modalDiv = document.createElement("div");
    modalDiv.className = "report-preview-modal";
    modalDiv.style.position = "fixed";
    modalDiv.style.top = "0";
    modalDiv.style.left = "0";
    modalDiv.style.width = "100%";
    modalDiv.style.height = "100%";
    modalDiv.style.backgroundColor = "rgba(0, 0, 0, 0.8)";
    modalDiv.style.zIndex = "9999";
    modalDiv.style.display = "flex";
    modalDiv.style.justifyContent = "center";
    modalDiv.style.alignItems = "center";

    // Crear el contenido del modal
    const modalContent = document.createElement("div");
    modalContent.style.backgroundColor = "white";
    modalContent.style.width = "90%";
    modalContent.style.height = "90%";
    modalContent.style.borderRadius = "8px";
    modalContent.style.display = "flex";
    modalContent.style.flexDirection = "column";
    modalContent.style.overflow = "hidden";
    modalContent.style.boxShadow = "0 4px 20px rgba(0, 0, 0, 0.3)";

    // Crear la barra de título
    const titleBar = document.createElement("div");
    titleBar.style.backgroundColor = "#8a1538";
    titleBar.style.color = "white";
    titleBar.style.padding = "15px 20px";
    titleBar.style.display = "flex";
    titleBar.style.justifyContent = "space-between";
    titleBar.style.alignItems = "center";
    titleBar.style.fontSize = "18px";
    titleBar.style.fontWeight = "600";

    const titleText = document.createElement("span");
    titleText.textContent = title;
    titleBar.appendChild(titleText);

    // Crear botón de cerrar
    const closeBtn = document.createElement("button");
    closeBtn.innerHTML = "&times;";
    closeBtn.style.background = "none";
    closeBtn.style.border = "none";
    closeBtn.style.color = "white";
    closeBtn.style.fontSize = "24px";
    closeBtn.style.cursor = "pointer";
    closeBtn.style.padding = "0";
    closeBtn.style.width = "30px";
    closeBtn.style.height = "30px";
    closeBtn.style.display = "flex";
    closeBtn.style.alignItems = "center";
    closeBtn.style.justifyContent = "center";
    closeBtn.style.borderRadius = "50%";
    closeBtn.style.transition = "background-color 0.2s";

    closeBtn.addEventListener("mouseenter", () => {
      closeBtn.style.backgroundColor = "rgba(255, 255, 255, 0.2)";
    });

    closeBtn.addEventListener("mouseleave", () => {
      closeBtn.style.backgroundColor = "transparent";
    });

    closeBtn.addEventListener("click", () => {
      document.body.removeChild(modalDiv);
    });

    titleBar.appendChild(closeBtn);

    // Crear contenedor del contenido HTML
    const contentContainer = document.createElement("div");
    contentContainer.style.flex = "1";
    contentContainer.style.overflow = "auto";
    contentContainer.style.padding = "20px";

    // Insertar el contenido HTML
    contentContainer.innerHTML = htmlContent;

    // Aplicar estilos básicos al contenido
    const style = document.createElement("style");
    style.textContent = `
      .report-preview-modal h1, .report-preview-modal h2, .report-preview-modal h3 {
        color: #8a1538;
        margin-bottom: 15px;
      }
      .report-preview-modal table {
        width: 100%;
        border-collapse: collapse;
        margin-bottom: 20px;
      }
      .report-preview-modal th, .report-preview-modal td {
        border: 1px solid #ddd;
        padding: 8px;
        text-align: left;
      }
      .report-preview-modal th {
        background-color: #f8f9fa;
        font-weight: 600;
      }
      .report-preview-modal .badge {
        padding: 4px 8px;
        border-radius: 4px;
        font-size: 12px;
        font-weight: 600;
      }
      .report-preview-modal .badge.bg-success {
        background-color: #198754 !important;
        color: white;
      }
      .report-preview-modal .badge.bg-warning {
        background-color: #ffc107 !important;
        color: black;
      }
      .report-preview-modal .badge.bg-danger {
        background-color: #dc3545 !important;
        color: white;
      }
    `;
    document.head.appendChild(style);

    // Ensamblar el modal
    modalContent.appendChild(titleBar);
    modalContent.appendChild(contentContainer);
    modalDiv.appendChild(modalContent);

    // Cerrar modal al hacer clic en el fondo
    modalDiv.addEventListener("click", (e) => {
      if (e.target === modalDiv) {
        document.body.removeChild(modalDiv);
      }
    });

    // Cerrar modal con Escape
    document.addEventListener("keydown", function escapeHandler(e) {
      if (e.key === "Escape") {
        document.body.removeChild(modalDiv);
        document.removeEventListener("keydown", escapeHandler);
      }
    });

    // Agregar el modal al DOM
    document.body.appendChild(modalDiv);

    // Agregar animación de entrada
    modalDiv.style.opacity = "0";
    setTimeout(() => {
      modalDiv.style.transition = "opacity 0.3s ease";
      modalDiv.style.opacity = "1";
    }, 10);
  } catch (error) {
    console.error("Error al mostrar vista previa del reporte:", error);
    if (typeof showToast === "function") {
      showToast("Error al mostrar la vista previa: " + error.message, "error");
    }
  }
};
