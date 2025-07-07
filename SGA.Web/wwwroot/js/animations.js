// Script para animaciones y mejoras visuales
document.addEventListener("DOMContentLoaded", function () {
  // Agregar clase de animación a elementos principales
  const elementsToAnimate = document.querySelectorAll(
    ".register-card, .login-card, .dashboard-card, .card, h1, h2, h3"
  );
  elementsToAnimate.forEach((element, index) => {
    setTimeout(() => {
      element.classList.add("animate__animated", "animate__fadeInUp");
    }, index * 100);
  });

  // Mejorar las transiciones en los formularios
  const inputElements = document.querySelectorAll("input, select, textarea");
  inputElements.forEach((input) => {
    input.addEventListener("focus", function () {
      this.closest(".register-form-group, .login-form-group")?.classList.add(
        "focused"
      );
    });

    input.addEventListener("blur", function () {
      if (this.value === "") {
        this.closest(
          ".register-form-group, .login-form-group"
        )?.classList.remove("focused");
      }
    });
  });

  // Efecto de hover para botones
  const buttons = document.querySelectorAll("button");
  buttons.forEach((button) => {
    button.addEventListener("mouseenter", function () {
      this.style.transform = "translateY(-2px)";
      this.style.boxShadow = "0 5px 15px rgba(138, 21, 56, 0.3)";
    });

    button.addEventListener("mouseleave", function () {
      this.style.transform = "";
      this.style.boxShadow = "";
    });
  });
});

// Función para inicializar animaciones en elementos específicos
function initializeAnimations() {
  console.log("Inicializando animaciones del dashboard...");
  setTimeout(() => {
    animateCounters();
    addHoverEffects();
    initializeTooltips();
    addPulseEffects();
  }, 300); // Pequeño delay para asegurar que el DOM está completamente cargado
}

// Función para animar contadores
function animateCounters() {
  document.querySelectorAll(".indicator-value").forEach((counter) => {
    const target = parseFloat(counter.textContent);
    const duration = 1500;
    const startTime = performance.now();

    function updateCounter(currentTime) {
      const elapsedTime = currentTime - startTime;
      const progress = Math.min(elapsedTime / duration, 1);
      const value = Math.floor(progress * target);

      if (counter.textContent.includes("%")) {
        counter.textContent = value + "%";
      } else {
        counter.textContent = value;
      }

      if (progress < 1) {
        requestAnimationFrame(updateCounter);
      }
    }

    requestAnimationFrame(updateCounter);
  });
}

// Función para añadir efectos hover
function addHoverEffects() {
  document.querySelectorAll(".indicator-card").forEach((card) => {
    card.addEventListener("mouseenter", () => {
      card.querySelector(".indicator-progress-bar").style.opacity = "0.8";
      card.style.transform = "translateY(-8px) scale(1.02)";
      card.style.boxShadow = "0 15px 30px rgba(138, 21, 56, 0.2)";
    });

    card.addEventListener("mouseleave", () => {
      card.querySelector(".indicator-progress-bar").style.opacity = "1";
      card.style.transform = "";
      card.style.boxShadow = "";
    });
  });
}

// Función para inicializar tooltips
function initializeTooltips() {
  if (typeof bootstrap !== "undefined" && bootstrap.Tooltip) {
    const tooltipTriggerList = [].slice.call(
      document.querySelectorAll('[data-bs-toggle="tooltip"]')
    );
    tooltipTriggerList.map(function (tooltipTriggerEl) {
      return new bootstrap.Tooltip(tooltipTriggerEl);
    });
  }
}

// Función para mostrar toast de notificación
function showToast(message, type = "success") {
  if (typeof Toastify !== "undefined") {
    Toastify({
      text: message,
      duration: 3000,
      close: true,
      gravity: "top",
      position: "right",
      backgroundColor: type === "success" ? "#28a745" : "#dc3545",
      stopOnFocus: true,
    }).showToast();
  } else {
    console.log(message);
  }
}

// Función para añadir efectos de pulso
function addPulseEffects() {
  // Añadir efectos de onda a las tarjetas de estado
  document.querySelectorAll(".status-badge").forEach((badge) => {
    const ripple = document.createElement("span");
    ripple.classList.add("ripple-effect");
    badge.appendChild(ripple);
  });

  // Añadir efectos de sombra interactivos a botones
  document.querySelectorAll(".btn").forEach((btn) => {
    btn.addEventListener("mousemove", (e) => {
      const rect = btn.getBoundingClientRect();
      const x = e.clientX - rect.left;
      const y = e.clientY - rect.top;

      btn.style.background = `radial-gradient(circle at ${x}px ${y}px, rgba(255,255,255,0.2) 0%, rgba(0,0,0,0) 60%), ${
        getComputedStyle(btn).background
      }`;
    });

    btn.addEventListener("mouseleave", () => {
      btn.style.background = "";
    });
  });
}

// Para Blazor, exportamos las funciones
window.showToast = showToast;
window.animateCounters = animateCounters;
window.initializeAnimations = initializeAnimations;

// Función para agregar atajos de teclado
function addKeyboardShortcuts(dotNetHelper) {
  document.addEventListener("keydown", function (event) {
    // Verificar si se presionó Ctrl+0
    if (event.ctrlKey && event.key === "0") {
      event.preventDefault(); // Prevenir el comportamiento por defecto
      dotNetHelper.invokeMethodAsync("ReiniciarCardsVisualmente");
    }
  });
}

// Agregar la función al window
window.addKeyboardShortcuts = addKeyboardShortcuts;
