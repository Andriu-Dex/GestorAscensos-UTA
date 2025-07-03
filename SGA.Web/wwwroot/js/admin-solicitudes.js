// JavaScript para la funcionalidad de documentos en AdminSolicitudes

window.downloadDocument = (url, filename) => {
  try {
    // Crear un enlace temporal para la descarga
    const link = document.createElement("a");
    link.href = url;
    link.download = filename || "documento.pdf";

    // Agregar el enlace al DOM temporalmente
    document.body.appendChild(link);

    // Hacer clic en el enlace para iniciar la descarga
    link.click();

    // Remover el enlace del DOM
    document.body.removeChild(link);

    console.log(`Descarga iniciada: ${filename}`);
  } catch (error) {
    console.error("Error al descargar documento:", error);
    alert("Error al descargar el documento. Por favor, intente nuevamente.");
  }
};

// Función para cerrar modales con tecla Escape
document.addEventListener("keydown", function (event) {
  if (event.key === "Escape") {
    // Buscar si hay algún modal abierto
    const modals = document.querySelectorAll(".modal.show");
    if (modals.length > 0) {
      // Enviar evento para cerrar el modal más reciente
      const lastModal = modals[modals.length - 1];
      const closeButton = lastModal.querySelector(".btn-close");
      if (closeButton) {
        closeButton.click();
      }
    }
  }
});

// Función para mejorar la experiencia del usuario con los modales
window.initializeModalFocus = () => {
  // Enfocar el primer elemento focusable en modales abiertos
  const openModals = document.querySelectorAll(".modal.show");
  openModals.forEach((modal) => {
    const focusableElements = modal.querySelectorAll(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    if (focusableElements.length > 0) {
      focusableElements[0].focus();
    }
  });
};

// Función para mostrar tooltips informativos
window.showTooltip = (element, message) => {
  if (element && message) {
    element.setAttribute("title", message);
    element.setAttribute("data-bs-toggle", "tooltip");
    element.setAttribute("data-bs-placement", "top");

    // Inicializar tooltip de Bootstrap si está disponible
    if (typeof bootstrap !== "undefined" && bootstrap.Tooltip) {
      new bootstrap.Tooltip(element);
    }
  }
};

// Función para formatear fechas de manera consistente
window.formatDate = (dateString) => {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString("es-ES", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch (error) {
    console.error("Error al formatear fecha:", error);
    return dateString;
  }
};

// Función para validar archivos PDF antes de la descarga
window.validatePdfUrl = (url) => {
  return url && (url.includes("pdf") || url.includes("application/pdf"));
};

console.log("AdminSolicitudes JavaScript cargado correctamente");
