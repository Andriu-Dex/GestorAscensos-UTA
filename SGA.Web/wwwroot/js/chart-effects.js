// Efectos visuales específicos para gráficos elegantes
// chart-effects.js

// Función para crear efectos de partículas en el fondo
function createParticleEffect(canvasId) {
  const canvas = document.getElementById(canvasId);
  if (!canvas) return;

  const container = canvas.closest(".chart-container-enhanced");
  if (!container) return;

  // Crear canvas para partículas
  const particleCanvas = document.createElement("canvas");
  particleCanvas.style.position = "absolute";
  particleCanvas.style.top = "0";
  particleCanvas.style.left = "0";
  particleCanvas.style.width = "100%";
  particleCanvas.style.height = "100%";
  particleCanvas.style.pointerEvents = "none";
  particleCanvas.style.zIndex = "0";
  particleCanvas.style.opacity = "0.3";

  container.style.position = "relative";
  container.appendChild(particleCanvas);

  const ctx = particleCanvas.getContext("2d");

  // Ajustar tamaño del canvas
  function resizeParticleCanvas() {
    particleCanvas.width = container.offsetWidth;
    particleCanvas.height = container.offsetHeight;
  }

  resizeParticleCanvas();
  window.addEventListener("resize", resizeParticleCanvas);

  // Crear partículas
  const particles = [];
  for (let i = 0; i < 20; i++) {
    particles.push({
      x: Math.random() * particleCanvas.width,
      y: Math.random() * particleCanvas.height,
      size: Math.random() * 3 + 1,
      speedX: (Math.random() - 0.5) * 0.5,
      speedY: (Math.random() - 0.5) * 0.5,
      opacity: Math.random() * 0.5 + 0.1,
      color: `rgba(138, 21, 56, ${Math.random() * 0.3 + 0.1})`,
    });
  }

  // Animar partículas
  function animateParticles() {
    ctx.clearRect(0, 0, particleCanvas.width, particleCanvas.height);

    particles.forEach((particle) => {
      // Actualizar posición
      particle.x += particle.speedX;
      particle.y += particle.speedY;

      // Rebotar en los bordes
      if (particle.x < 0 || particle.x > particleCanvas.width) {
        particle.speedX *= -1;
      }
      if (particle.y < 0 || particle.y > particleCanvas.height) {
        particle.speedY *= -1;
      }

      // Dibujar partícula
      ctx.beginPath();
      ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2);
      ctx.fillStyle = particle.color;
      ctx.fill();

      // Efecto de resplandor
      ctx.beginPath();
      ctx.arc(particle.x, particle.y, particle.size * 2, 0, Math.PI * 2);
      ctx.fillStyle = `rgba(138, 21, 56, ${particle.opacity * 0.1})`;
      ctx.fill();
    });

    requestAnimationFrame(animateParticles);
  }

  animateParticles();
}

// Función para añadir efectos hover al gráfico
function addChartHoverEffects(chartInstance) {
  if (!chartInstance) return;

  const canvas = chartInstance.canvas;
  const container = canvas.closest(".chart-container-enhanced");

  if (!container) return;

  // Efectos de hover en el contenedor
  container.addEventListener("mouseenter", () => {
    container.style.transform = "translateY(-5px)";
    container.style.boxShadow = "0 20px 50px rgba(0, 0, 0, 0.15)";
  });

  container.addEventListener("mouseleave", () => {
    container.style.transform = "translateY(0)";
    container.style.boxShadow = "0 15px 40px rgba(0, 0, 0, 0.1)";
  });

  // Efectos de hover en el canvas
  canvas.addEventListener("mouseenter", () => {
    canvas.style.filter = "drop-shadow(0 12px 28px rgba(138, 21, 56, 0.25))";
  });

  canvas.addEventListener("mouseleave", () => {
    canvas.style.filter = "drop-shadow(0 8px 20px rgba(0, 0, 0, 0.12))";
  });
}

// Función para crear efectos de carga
function createLoadingEffect(containerId) {
  const container = document.getElementById(containerId);
  if (!container) return;

  // Crear elemento de carga
  const loadingEl = document.createElement("div");
  loadingEl.className = "loading-elegant";
  loadingEl.innerHTML = `
        <div class="text-center">
            <div class="spinner-elegant"></div>
            <p class="mt-3 text-muted">Cargando gráfico elegante...</p>
        </div>
    `;

  container.appendChild(loadingEl);

  // Remover después de 3 segundos
  setTimeout(() => {
    if (loadingEl && loadingEl.parentNode) {
      loadingEl.parentNode.removeChild(loadingEl);
    }
  }, 3000);
}

// Función para crear efectos de entrada
function createEntryAnimation(chartInstance) {
  if (!chartInstance) return;

  const canvas = chartInstance.canvas;
  const container = canvas.closest(".chart-container-enhanced");

  if (!container) return;

  // Animación de entrada
  container.style.opacity = "0";
  container.style.transform = "translateY(30px) scale(0.9)";

  setTimeout(() => {
    container.style.transition = "all 0.8s cubic-bezier(0.4, 0, 0.2, 1)";
    container.style.opacity = "1";
    container.style.transform = "translateY(0) scale(1)";
  }, 100);
}

// Función para crear efectos de tooltip mejorados
function enhanceTooltips(chartInstance) {
  if (!chartInstance) return;

  const originalTooltip = chartInstance.options.plugins.tooltip;

  // Sobrescribir el tooltip
  chartInstance.options.plugins.tooltip = {
    ...originalTooltip,
    external: function (context) {
      const tooltip = context.tooltip;

      if (tooltip.opacity === 0) {
        return;
      }

      // Crear tooltip personalizado
      let tooltipEl = document.getElementById("custom-tooltip");
      if (!tooltipEl) {
        tooltipEl = document.createElement("div");
        tooltipEl.id = "custom-tooltip";
        tooltipEl.className = "custom-tooltip";
        tooltipEl.style.position = "absolute";
        tooltipEl.style.pointerEvents = "none";
        tooltipEl.style.zIndex = "1000";
        document.body.appendChild(tooltipEl);
      }

      // Configurar contenido
      if (tooltip.body) {
        const bodyLines = tooltip.body.map((b) => b.lines);
        const title = tooltip.title || [];

        let innerHtml = "<div>";
        title.forEach((t) => {
          innerHtml += `<div class="tooltip-title">${t}</div>`;
        });

        bodyLines.forEach((body) => {
          innerHtml += `<div class="tooltip-body">${body}</div>`;
        });

        innerHtml += "</div>";
        tooltipEl.innerHTML = innerHtml;
      }

      // Posicionar tooltip
      const position = Chart.helpers.getRelativePosition(
        context.chart.canvas,
        context.tooltip.caretX,
        context.tooltip.caretY
      );
      tooltipEl.style.left = position.x + "px";
      tooltipEl.style.top = position.y + "px";
      tooltipEl.style.opacity = "1";
    },
  };

  chartInstance.update();
}

// Función para limpiar efectos
function cleanupEffects() {
  // Limpiar partículas
  document
    .querySelectorAll(".chart-container-enhanced canvas:not([id])")
    .forEach((canvas) => {
      if (canvas.parentNode) {
        canvas.parentNode.removeChild(canvas);
      }
    });

  // Limpiar tooltip personalizado
  const tooltip = document.getElementById("custom-tooltip");
  if (tooltip) {
    tooltip.remove();
  }
}

// Función principal para inicializar todos los efectos
window.initializeChartEffects = function (chartInstance) {
  if (!chartInstance) return;

  try {
    // Crear efectos de partículas
    createParticleEffect(chartInstance.canvas.id);

    // Añadir efectos de hover
    addChartHoverEffects(chartInstance);

    // Crear animación de entrada
    createEntryAnimation(chartInstance);

    // Mejorar tooltips
    enhanceTooltips(chartInstance);

    console.log("Efectos de gráfico inicializados correctamente");
  } catch (error) {
    console.error("Error al inicializar efectos:", error);
  }
};

// Limpiar efectos al salir de la página
window.addEventListener("beforeunload", cleanupEffects);

console.log("Script chart-effects.js cargado correctamente");
