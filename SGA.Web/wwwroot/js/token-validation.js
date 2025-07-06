// Script para manejar la expiración de tokens JWT
window.tokenValidation = {
  // Función para parsear un JWT y obtener su expiración
  parseJwt: function (token) {
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split("")
          .map(function (c) {
            return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
          })
          .join("")
      );

      return JSON.parse(jsonPayload);
    } catch (e) {
      console.error("Error parsing JWT:", e);
      return null;
    }
  },

  // Función para verificar si un token ha expirado
  isTokenExpired: function (token) {
    if (!token) return true;

    const payload = this.parseJwt(token);
    if (!payload || !payload.exp) return true;

    // Convertir exp (Unix timestamp) a milisegundos y comparar con la fecha actual
    const expirationTime = payload.exp * 1000;
    const currentTime = Date.now();

    return currentTime >= expirationTime;
  },

  // Función para obtener el tiempo restante hasta la expiración en minutos
  getTimeUntilExpiration: function (token) {
    if (!token) return -1;

    const payload = this.parseJwt(token);
    if (!payload || !payload.exp) return -1;

    const expirationTime = payload.exp * 1000;
    const currentTime = Date.now();
    const timeRemaining = expirationTime - currentTime;

    return Math.floor(timeRemaining / (1000 * 60)); // Convertir a minutos
  },

  // Función para mostrar una notificación personalizada de expiración
  showExpirationNotification: function (message, redirectDelay = 5000) {
    // Crear el modal de notificación
    const modalHtml = `
            <div id="token-expiration-modal" class="modal fade show" tabindex="-1" 
                 style="display: block; background-color: rgba(0,0,0,0.5); z-index: 9999;">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header" style="background: linear-gradient(135deg, #8a1538 0%, #a91d47 100%); color: white; border-bottom: none;">
                            <h5 class="modal-title">
                                <i class="fas fa-exclamation-triangle me-2"></i>
                                Sesión Expirada
                            </h5>
                        </div>
                        <div class="modal-body text-center">
                            <div class="mb-3">
                                <i class="fas fa-clock fa-3x text-warning mb-3"></i>
                            </div>
                            <p class="mb-2"><strong>${message}</strong></p>
                            <p class="text-muted">Será redirigido automáticamente al login en <span id="countdown">${Math.ceil(
                              redirectDelay / 1000
                            )}</span> segundos.</p>
                            <div class="mt-3">
                                <button type="button" class="btn btn-primary" onclick="window.tokenValidation.redirectToLogin()">
                                    <i class="fas fa-sign-in-alt me-1"></i>
                                    Ir al Login Ahora
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

    // Agregar el modal al DOM
    document.body.insertAdjacentHTML("beforeend", modalHtml);

    // Iniciar cuenta regresiva
    let countdown = Math.ceil(redirectDelay / 1000);
    const countdownElement = document.getElementById("countdown");

    const countdownInterval = setInterval(() => {
      countdown--;
      if (countdownElement) {
        countdownElement.textContent = countdown;
      }

      if (countdown <= 0) {
        clearInterval(countdownInterval);
        this.redirectToLogin();
      }
    }, 1000);

    // Auto-redirigir después del delay
    setTimeout(() => {
      this.redirectToLogin();
    }, redirectDelay);
  },

  // Función para redirigir al login
  redirectToLogin: function () {
    // Remover el modal si existe
    const modal = document.getElementById("token-expiration-modal");
    if (modal) {
      modal.remove();
    }

    // Limpiar localStorage
    localStorage.removeItem("authToken");

    // Redirigir al login
    window.location.href = "/login";
  },

  // Función para establecer un timer de verificación del token
  startTokenMonitoring: function (intervalMinutes = 2) {
    setInterval(() => {
      const token = localStorage.getItem("authToken");
      if (token && this.isTokenExpired(token)) {
        console.log("Token expirado detectado por el monitor del cliente");
        this.showExpirationNotification(
          "Su sesión ha expirado por motivos de seguridad.",
          5000
        );
      }
    }, intervalMinutes * 60 * 1000);
  },

  // Función para verificar el token antes de hacer peticiones importantes
  checkTokenBeforeRequest: function () {
    const token = localStorage.getItem("authToken");
    if (!token || this.isTokenExpired(token)) {
      this.showExpirationNotification(
        "Su sesión ha expirado. Por favor, inicie sesión nuevamente.",
        3000
      );
      return false;
    }
    return true;
  },
};

// Inicializar el monitoreo cuando se carga la página
document.addEventListener("DOMContentLoaded", function () {
  // Solo iniciar el monitoreo si hay un token
  const token = localStorage.getItem("authToken");
  if (token && !window.tokenValidation.isTokenExpired(token)) {
    window.tokenValidation.startTokenMonitoring(2); // Verificar cada 2 minutos
    console.log("Token monitoring iniciado");
  }
});
