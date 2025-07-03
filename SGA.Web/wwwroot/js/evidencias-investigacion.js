// Funciones JavaScript para mejorar la experiencia de usuario en evidencias de investigación

window.evidenciasInvestigacionHelpers = {
  // Mejorar la experiencia de carga de archivos
  setupFileUpload: function () {
    document.addEventListener("change", function (e) {
      if (e.target && e.target.classList.contains("file-upload-evidencia")) {
        const file = e.target.files[0];
        if (file) {
          // Validar tamaño del archivo (max 10MB)
          if (file.size > 10 * 1024 * 1024) {
            if (typeof showToast === "function") {
              showToast(
                "El archivo es demasiado grande. El tamaño máximo permitido es 10MB.",
                "error"
              );
            } else {
              console.error(
                "El archivo es demasiado grande. El tamaño máximo permitido es 10MB."
              );
            }
            e.target.value = "";
            return;
          }

          // Validar tipo de archivo
          if (file.type !== "application/pdf") {
            if (typeof showToast === "function") {
              showToast("Solo se permiten archivos PDF.", "error");
            } else {
              console.error("Solo se permiten archivos PDF.");
            }
            e.target.value = "";
            return;
          }

          // Mostrar feedback visual
          const feedback =
            e.target.parentElement.querySelector(".file-feedback");
          if (feedback) {
            feedback.innerHTML = `<i class="bi bi-check-circle text-success"></i> ${
              file.name
            } (${(file.size / 1024 / 1024).toFixed(2)} MB)`;
          }
        }
      }
    });
  },

  // Validar formulario antes de envío
  validateEvidenciaForm: function (evidenciaIndex) {
    const card = document.querySelector(
      `[data-evidencia-index="${evidenciaIndex}"]`
    );
    if (!card) return false;

    const tipoEvidencia = card.querySelector('[name="TipoEvidencia"]');
    const tituloProyecto = card.querySelector('[name="TituloProyecto"]');
    const rolInvestigador = card.querySelector('[name="RolInvestigador"]');
    const fechaInicio = card.querySelector('[name="FechaInicio"]');
    const mesesDuracion = card.querySelector('[name="MesesDuracion"]');

    let isValid = true;
    let messages = [];

    if (!tipoEvidencia?.value) {
      this.showFieldError(
        tipoEvidencia,
        "Debe seleccionar un tipo de evidencia"
      );
      isValid = false;
      messages.push("Tipo de evidencia es requerido");
    }

    if (!tituloProyecto?.value?.trim()) {
      this.showFieldError(
        tituloProyecto,
        "Debe ingresar el título del proyecto"
      );
      isValid = false;
      messages.push("Título del proyecto es requerido");
    }

    if (!rolInvestigador?.value) {
      this.showFieldError(rolInvestigador, "Debe seleccionar un rol");
      isValid = false;
      messages.push("Rol del investigador es requerido");
    }

    if (!fechaInicio?.value) {
      this.showFieldError(fechaInicio, "Debe seleccionar una fecha de inicio");
      isValid = false;
      messages.push("Fecha de inicio es requerida");
    }

    if (!mesesDuracion?.value || parseInt(mesesDuracion.value) < 1) {
      this.showFieldError(mesesDuracion, "La duración debe ser al menos 1 mes");
      isValid = false;
      messages.push("Duración debe ser al menos 1 mes");
    }

    if (!isValid) {
      this.showValidationSummary(messages);
    }

    return isValid;
  },

  // Mostrar error en campo específico
  showFieldError: function (field, message) {
    if (!field) return;

    field.classList.add("is-invalid");

    let feedback = field.parentElement.querySelector(".invalid-feedback");
    if (!feedback) {
      feedback = document.createElement("div");
      feedback.className = "invalid-feedback";
      field.parentElement.appendChild(feedback);
    }
    feedback.textContent = message;

    // Remover error después de 5 segundos
    setTimeout(() => {
      field.classList.remove("is-invalid");
      if (feedback) feedback.remove();
    }, 5000);
  },

  // Mostrar resumen de validación
  showValidationSummary: function (messages) {
    const alertDiv = document.createElement("div");
    alertDiv.className = "alert alert-danger alert-dismissible fade show mt-3";
    alertDiv.innerHTML = `
            <strong>Por favor, corrija los siguientes errores:</strong>
            <ul class="mb-0 mt-2">
                ${messages.map((msg) => `<li>${msg}</li>`).join("")}
            </ul>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

    const modalBody = document.querySelector(".modal-body");
    if (modalBody) {
      // Remover alertas anteriores
      const existingAlerts = modalBody.querySelectorAll(".alert-danger");
      existingAlerts.forEach((alert) => alert.remove());

      modalBody.insertBefore(alertDiv, modalBody.firstChild);

      // Auto-remover después de 10 segundos
      setTimeout(() => {
        if (alertDiv.parentElement) {
          alertDiv.remove();
        }
      }, 10000);
    }
  },

  // Calcular duración automáticamente
  calculateDuration: function (fechaInicio, fechaFin) {
    if (!fechaInicio || !fechaFin) return 0;

    const startDate = new Date(fechaInicio);
    const endDate = new Date(fechaFin);

    if (endDate <= startDate) return 0;

    const diffTime = Math.abs(endDate - startDate);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    const diffMonths = Math.ceil(diffDays / 30);

    return diffMonths;
  },

  // Confirmar eliminación
  confirmDelete: function (titulo) {
    return confirm(
      `¿Está seguro de que desea eliminar la evidencia "${titulo}"?`
    );
  },

  // Mostrar toast personalizado
  showToast: function (message, type = "info") {
    const toastContainer =
      document.querySelector(".toast-container") || this.createToastContainer();

    const toastEl = document.createElement("div");
    toastEl.className = `toast align-items-center text-white bg-${
      type === "error" ? "danger" : type === "success" ? "success" : "info"
    } border-0`;
    toastEl.setAttribute("role", "alert");
    toastEl.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        `;

    toastContainer.appendChild(toastEl);

    // Usar Bootstrap Toast
    const toast = new bootstrap.Toast(toastEl);
    toast.show();

    // Remover después de mostrar
    toastEl.addEventListener("hidden.bs.toast", () => {
      toastEl.remove();
    });
  },

  // Crear contenedor de toasts si no existe
  createToastContainer: function () {
    const container = document.createElement("div");
    container.className = "toast-container position-fixed top-0 end-0 p-3";
    container.style.zIndex = "9999";
    document.body.appendChild(container);
    return container;
  },

  // Mejorar accesibilidad
  setupAccessibility: function () {
    // Agregar navegación por teclado en tablas
    document.addEventListener("keydown", function (e) {
      if (e.target.tagName === "BUTTON" && e.target.closest(".table")) {
        if (e.key === "ArrowDown" || e.key === "ArrowUp") {
          e.preventDefault();
          const buttons = Array.from(
            document.querySelectorAll(".table button")
          );
          const currentIndex = buttons.indexOf(e.target);

          let nextIndex;
          if (e.key === "ArrowDown") {
            nextIndex = Math.min(currentIndex + 1, buttons.length - 1);
          } else {
            nextIndex = Math.max(currentIndex - 1, 0);
          }

          buttons[nextIndex]?.focus();
        }
      }
    });
  },

  // Inicializar todas las mejoras
  init: function () {
    this.setupFileUpload();
    this.setupAccessibility();
    console.log("Evidencias de Investigación JS helpers initialized");
  },
};

// Inicializar cuando el DOM esté listo
if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", () => {
    window.evidenciasInvestigacionHelpers.init();
  });
} else {
  window.evidenciasInvestigacionHelpers.init();
}
