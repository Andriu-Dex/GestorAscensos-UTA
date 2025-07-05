/**
 * Sistema de Toast para Notificaciones
 * Implementación moderna y profesional de toasts
 */

window.notificationSystem = {
  // Configuración por defecto
  defaultConfig: {
    duration: 5000,
    position: "top-right",
    showProgress: true,
    allowClose: true,
  },

  // Contador para IDs únicos
  toastCounter: 0,

  // Container principal
  container: null,

  /**
   * Inicializa el sistema de toasts
   */
  init() {
    if (!this.container) {
      this.createContainer();
    }
  },

  /**
   * Crea el container principal para los toasts
   */
  createContainer() {
    this.container = document.createElement("div");
    this.container.id = "toast-container";
    this.container.className = "toast-container";
    document.body.appendChild(this.container);

    // Agregar estilos CSS si no existen
    if (!document.getElementById("toast-styles")) {
      this.injectStyles();
    }
  },

  /**
   * Inyecta los estilos CSS necesarios
   */
  injectStyles() {
    const style = document.createElement("style");
    style.id = "toast-styles";
    style.textContent = `
            .toast-container {
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                pointer-events: none;
            }

            .toast {
                background: white;
                border-radius: 12px;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12);
                margin-bottom: 12px;
                min-width: 320px;
                max-width: 400px;
                opacity: 0;
                transform: translateX(100%);
                transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
                pointer-events: auto;
                border: 1px solid rgba(0, 0, 0, 0.08);
                overflow: hidden;
                position: relative;
            }

            .toast.show {
                opacity: 1;
                transform: translateX(0);
            }

            .toast.hide {
                opacity: 0;
                transform: translateX(100%);
            }

            .toast-header {
                display: flex;
                align-items: center;
                padding: 12px 16px 8px 16px;
                border-bottom: 1px solid rgba(0, 0, 0, 0.05);
            }

            .toast-icon {
                width: 24px;
                height: 24px;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                margin-right: 12px;
                font-size: 12px;
                color: white;
                font-weight: bold;
            }

            .toast-icon.success { background: #28a745; }
            .toast-icon.danger { background: #dc3545; }
            .toast-icon.warning { background: #ffc107; color: #212529; }
            .toast-icon.info { background: #17a2b8; }
            .toast-icon.primary { background: #8a1538; }

            .toast-title {
                font-weight: 600;
                font-size: 14px;
                color: #2c3e50;
                flex: 1;
                margin: 0;
            }

            .toast-close {
                background: none;
                border: none;
                color: #6c757d;
                cursor: pointer;
                padding: 4px;
                border-radius: 4px;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: all 0.2s ease;
                margin-left: 8px;
            }

            .toast-close:hover {
                background: rgba(108, 117, 125, 0.1);
                color: #495057;
            }

            .toast-body {
                padding: 8px 16px 12px 52px;
                font-size: 13px;
                color: #495057;
                line-height: 1.4;
            }

            .toast-progress {
                position: absolute;
                bottom: 0;
                left: 0;
                height: 3px;
                background: rgba(138, 21, 56, 0.2);
                width: 100%;
                overflow: hidden;
            }

            .toast-progress-bar {
                height: 100%;
                background: #8a1538;
                width: 100%;
                transform-origin: left;
                animation: toast-progress linear;
            }

            @keyframes toast-progress {
                from { transform: scaleX(1); }
                to { transform: scaleX(0); }
            }

            @media (max-width: 768px) {
                .toast-container {
                    top: 10px;
                    right: 10px;
                    left: 10px;
                }

                .toast {
                    min-width: auto;
                    max-width: none;
                }
            }
        `;
    document.head.appendChild(style);
  },

  /**
   * Muestra un toast
   */
  show(title, message, type = "info", options = {}) {
    this.init();

    const config = { ...this.defaultConfig, ...options };
    const toastId = `toast-${++this.toastCounter}`;

    const toast = document.createElement("div");
    toast.className = "toast";
    toast.id = toastId;

    const iconMap = {
      success: "✓",
      danger: "✕",
      warning: "⚠",
      info: "ℹ",
      primary: "●",
    };

    toast.innerHTML = `
            <div class="toast-header">
                <div class="toast-icon ${type}">
                    ${iconMap[type] || iconMap.info}
                </div>
                <h6 class="toast-title">${this.escapeHtml(title)}</h6>
                ${
                  config.allowClose
                    ? '<button class="toast-close" onclick="notificationSystem.hide(\'' +
                      toastId +
                      "')\">×</button>"
                    : ""
                }
            </div>
            <div class="toast-body">
                ${this.escapeHtml(message)}
            </div>
            ${
              config.showProgress
                ? `
                <div class="toast-progress">
                    <div class="toast-progress-bar" style="animation-duration: ${config.duration}ms;"></div>
                </div>
            `
                : ""
            }
        `;

    this.container.appendChild(toast);

    // Mostrar el toast
    setTimeout(() => {
      toast.classList.add("show");
    }, 100);

    // Auto-hide después del tiempo especificado
    if (config.duration > 0) {
      setTimeout(() => {
        this.hide(toastId);
      }, config.duration);
    }

    return toastId;
  },

  /**
   * Oculta un toast específico
   */
  hide(toastId) {
    const toast = document.getElementById(toastId);
    if (toast) {
      toast.classList.add("hide");
      setTimeout(() => {
        if (toast.parentNode) {
          toast.parentNode.removeChild(toast);
        }
      }, 300);
    }
  },

  /**
   * Oculta todos los toasts
   */
  hideAll() {
    const toasts = this.container.querySelectorAll(".toast");
    toasts.forEach((toast) => {
      this.hide(toast.id);
    });
  },

  /**
   * Escapa HTML para prevenir XSS
   */
  escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
  },
};

// Función global para compatibilidad con Blazor
window.showToast = function (title, message, type, options) {
  return window.notificationSystem.show(title, message, type, options);
};

window.hideToast = function (toastId) {
  return window.notificationSystem.hide(toastId);
};

window.hideAllToasts = function () {
  return window.notificationSystem.hideAll();
};

// Auto-inicializar
document.addEventListener("DOMContentLoaded", function () {
  window.notificationSystem.init();
});

/**
 * Sistema de sonidos para notificaciones
 */
window.playNotificationSound = function () {
  try {
    // Crear un AudioContext si no existe
    if (!window.audioContext) {
      window.audioContext = new (window.AudioContext ||
        window.webkitAudioContext)();
    }

    // Crear un sonido de notificación usando osciladores
    const oscillator = window.audioContext.createOscillator();
    const gainNode = window.audioContext.createGain();

    oscillator.connect(gainNode);
    gainNode.connect(window.audioContext.destination);

    // Configurar el sonido
    oscillator.frequency.setValueAtTime(800, window.audioContext.currentTime);
    oscillator.frequency.setValueAtTime(
      600,
      window.audioContext.currentTime + 0.1
    );

    // Configurar el volumen con fade out
    gainNode.gain.setValueAtTime(0.3, window.audioContext.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(
      0.01,
      window.audioContext.currentTime + 0.3
    );

    // Reproducir el sonido
    oscillator.start(window.audioContext.currentTime);
    oscillator.stop(window.audioContext.currentTime + 0.3);
  } catch (error) {
    console.log("No se pudo reproducir el sonido de notificación:", error);
  }
};
