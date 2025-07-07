// JavaScript para los gráficos de estadísticas
// admin-estadisticas.js

// Función para crear gradientes elegantes
function createGradients(ctx, data) {
  const gradients = [];
  const colors = [
    { start: "#8B5A3C", mid: "#A0693C", end: "#B8754D" }, // Marrón elegante con gradiente de 3 colores
    { start: "#4A90E2", mid: "#5BA3F5", end: "#6DB7FF" }, // Azul professional
    { start: "#50C878", mid: "#66D68B", end: "#7CE49E" }, // Verde esmeralda
    { start: "#F5A623", mid: "#FFB84D", end: "#FFC766" }, // Dorado elegante
    { start: "#7B68EE", mid: "#9370DB", end: "#A584E8" }, // Violeta medio
    { start: "#8a1538", mid: "#A91B47", end: "#C42556" }, // Color institucional
  ];

  data.forEach((value, index) => {
    const colorIndex = index % colors.length;
    const gradient = ctx.createRadialGradient(0, 0, 0, 0, 0, 200);
    gradient.addColorStop(0, colors[colorIndex].start);
    gradient.addColorStop(0.5, colors[colorIndex].mid);
    gradient.addColorStop(1, colors[colorIndex].end);
    gradients.push(gradient);
  });

  return gradients;
}

// Inicializar gráficos
window.initializeCharts = function (nivelesData, actividadData) {
  try {
    console.log("Inicializando gráficos...");
    console.log("Datos de niveles:", nivelesData);
    console.log("Datos de actividad:", actividadData);

    // Verificar que Chart.js esté cargado
    if (typeof Chart === "undefined") {
      console.error("Chart.js no está cargado");
      return;
    }

    // Destruir gráficos existentes si existen
    if (
      window.chartNiveles &&
      typeof window.chartNiveles.destroy === "function"
    ) {
      window.chartNiveles.destroy();
      window.chartNiveles = null;
    }
    if (
      window.chartActividad &&
      typeof window.chartActividad.destroy === "function"
    ) {
      window.chartActividad.destroy();
      window.chartActividad = null;
    }

    // Gráfico de distribución por niveles (doughnut chart)
    const ctxNiveles = document.getElementById("chartNiveles");
    if (ctxNiveles && nivelesData) {
      console.log("Creando gráfico de niveles...");

      // Verificar que los datos sean válidos
      if (
        nivelesData.datasets &&
        nivelesData.datasets.length > 0 &&
        nivelesData.datasets[0].data
      ) {
        const hasValidData = nivelesData.datasets[0].data.some(
          (value) => value > 0
        );

        if (hasValidData) {
          // Crear gradientes para los colores
          const gradients = createGradients(
            ctxNiveles.getContext("2d"),
            nivelesData.datasets[0].data
          );

          // Aplicar gradientes a los datos
          const enhancedData = {
            ...nivelesData,
            datasets: [
              {
                ...nivelesData.datasets[0],
                backgroundColor: gradients,
                borderWidth: 4,
                borderColor: "#ffffff",
                hoverBorderWidth: 6,
                hoverBorderColor: "#ffffff",
                hoverBackgroundColor: gradients.map((gradient) => gradient), // Mantener gradientes en hover
                shadowOffsetX: 3,
                shadowOffsetY: 3,
                shadowBlur: 10,
                shadowColor: "rgba(0, 0, 0, 0.3)",
                // Añadir efecto de resplandor
                hoverOffset: 15,
              },
            ],
          };

          window.chartNiveles = new Chart(ctxNiveles, {
            type: "doughnut",
            data: enhancedData,
            options: {
              responsive: true,
              maintainAspectRatio: false,
              layout: {
                padding: {
                  top: 30,
                  bottom: 30,
                  left: 30,
                  right: 30,
                },
              },
              plugins: {
                legend: {
                  display: false,
                },
                tooltip: {
                  backgroundColor: "rgba(33, 37, 41, 0.95)",
                  titleColor: "#fff",
                  bodyColor: "#fff",
                  borderColor: "rgba(138, 21, 56, 0.5)",
                  borderWidth: 2,
                  cornerRadius: 12,
                  displayColors: true,
                  padding: 16,
                  caretPadding: 8,
                  titleFont: {
                    size: 16,
                    weight: "bold",
                  },
                  bodyFont: {
                    size: 14,
                  },
                  boxPadding: 10,
                  usePointStyle: true,
                  pointStyle: "circle",
                  callbacks: {
                    label: function (context) {
                      const total = context.dataset.data.reduce(
                        (a, b) => a + b,
                        0
                      );
                      const percentage = (
                        (context.parsed * 100) /
                        total
                      ).toFixed(1);
                      return (
                        " " +
                        context.label +
                        ": " +
                        context.parsed +
                        " docentes (" +
                        percentage +
                        "%)"
                      );
                    },
                    title: function (context) {
                      return "Distribución Académica";
                    },
                  },
                },
              },
              cutout: "70%",
              spacing: 6,
              animation: {
                animateRotate: true,
                animateScale: true,
                duration: 1800,
                easing: "easeInOutQuart",
              },
              interaction: {
                intersect: false,
                mode: "point",
              },
              elements: {
                arc: {
                  borderWidth: 4,
                  borderColor: "#ffffff",
                  hoverBorderWidth: 6,
                  hoverBorderColor: "#ffffff",
                  borderRadius: 3,
                  borderJoinStyle: "round",
                },
              },
              onHover: (event, activeElements) => {
                event.native.target.style.cursor =
                  activeElements.length > 0 ? "pointer" : "default";
              },
            },
          });
          console.log("Gráfico de niveles creado exitosamente");

          // Inicializar efectos visuales elegantes
          if (typeof window.initializeChartEffects === "function") {
            window.initializeChartEffects(window.chartNiveles);
          }
        } else {
          console.warn("No hay datos válidos para el gráfico de niveles");
          // Mostrar mensaje de "sin datos"
          showNoDataMessage(
            "chartNiveles",
            "No hay datos de niveles disponibles"
          );
        }
      } else {
        console.warn("Estructura de datos de niveles no válida");
        showNoDataMessage(
          "chartNiveles",
          "No hay datos de niveles disponibles"
        );
      }
    } else {
      console.warn("Canvas de niveles no encontrado o datos nulos");
    }

    // Gráfico de actividad mensual (line chart)
    const ctxActividad = document.getElementById("chartActividad");
    if (ctxActividad && actividadData) {
      console.log("Creando gráfico de actividad...");

      window.chartActividad = new Chart(ctxActividad, {
        type: "line",
        data: actividadData,
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              display: true,
              position: "top",
              labels: {
                usePointStyle: true,
                padding: 20,
                font: {
                  size: 12,
                },
              },
            },
            tooltip: {
              mode: "index",
              intersect: false,
              backgroundColor: "rgba(0, 0, 0, 0.8)",
              titleColor: "#fff",
              bodyColor: "#fff",
              cornerRadius: 6,
              displayColors: true,
            },
          },
          scales: {
            x: {
              display: true,
              title: {
                display: true,
                text: "Período",
                font: {
                  size: 12,
                  weight: "bold",
                },
              },
              grid: {
                display: true,
                color: "rgba(0, 0, 0, 0.1)",
              },
            },
            y: {
              display: true,
              title: {
                display: true,
                text: "Cantidad",
                font: {
                  size: 12,
                  weight: "bold",
                },
              },
              grid: {
                display: true,
                color: "rgba(0, 0, 0, 0.1)",
              },
              beginAtZero: true,
            },
          },
          elements: {
            point: {
              radius: 4,
              hoverRadius: 6,
              borderWidth: 2,
              backgroundColor: "#fff",
            },
          },
          animation: {
            duration: 1000,
            easing: "easeInOutQuart",
          },
        },
      });

      console.log("Gráfico de actividad creado exitosamente");
    } else {
      console.warn("Canvas de actividad no encontrado o datos nulos");
    }

    console.log("Gráficos inicializados correctamente");
  } catch (error) {
    console.error("Error al inicializar gráficos:", error);
  }
};

// Función para mostrar mensaje de "sin datos"
function showNoDataMessage(canvasId, message) {
  try {
    const canvas = document.getElementById(canvasId);
    if (canvas) {
      const ctx = canvas.getContext("2d");
      const rect = canvas.getBoundingClientRect();

      // Configurar el tamaño del canvas
      canvas.width = rect.width || 300;
      canvas.height = rect.height || 300;

      ctx.clearRect(0, 0, canvas.width, canvas.height);

      // Configurar el texto
      ctx.font = "16px Arial";
      ctx.fillStyle = "#6c757d";
      ctx.textAlign = "center";
      ctx.textBaseline = "middle";

      // Dibujar el mensaje
      ctx.fillText(
        message || "No hay datos disponibles",
        canvas.width / 2,
        canvas.height / 2
      );
    }
  } catch (error) {
    console.error("Error al mostrar mensaje de sin datos:", error);
  }
}

// Función para actualizar los gráficos con nuevos datos
window.updateCharts = function (nivelesData, actividadData) {
  try {
    if (
      window.chartNiveles &&
      typeof window.chartNiveles.update === "function" &&
      nivelesData
    ) {
      window.chartNiveles.data = nivelesData;
      window.chartNiveles.update("active");
    }

    if (
      window.chartActividad &&
      typeof window.chartActividad.update === "function" &&
      actividadData
    ) {
      window.chartActividad.data = actividadData;
      window.chartActividad.update("active");
    }

    console.log("Gráficos actualizados correctamente");
  } catch (error) {
    console.error("Error al actualizar gráficos:", error);
  }
};

// Función para redimensionar gráficos
window.resizeCharts = function () {
  try {
    if (
      window.chartNiveles &&
      typeof window.chartNiveles.resize === "function"
    ) {
      window.chartNiveles.resize();
    }
    if (
      window.chartActividad &&
      typeof window.chartActividad.resize === "function"
    ) {
      window.chartActividad.resize();
    }
  } catch (error) {
    console.error("Error al redimensionar gráficos:", error);
  }
};

// Función para exportar gráfico como imagen
window.exportChart = function (chartId, filename) {
  try {
    const chart =
      chartId === "niveles" ? window.chartNiveles : window.chartActividad;
    if (!chart || typeof chart.toBase64Image !== "function") {
      console.error("Gráfico no encontrado o no válido");
      return;
    }

    const url = chart.toBase64Image();
    const link = document.createElement("a");
    link.download = filename || "grafico.png";
    link.href = url;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  } catch (error) {
    console.error("Error al exportar gráfico:", error);
  }
};

// Función para limpiar gráficos
window.destroyCharts = function () {
  try {
    if (
      window.chartNiveles &&
      typeof window.chartNiveles.destroy === "function"
    ) {
      window.chartNiveles.destroy();
      window.chartNiveles = null;
    }
    if (
      window.chartActividad &&
      typeof window.chartActividad.destroy === "function"
    ) {
      window.chartActividad.destroy();
      window.chartActividad = null;
    }
  } catch (error) {
    console.error("Error al destruir gráficos:", error);
  }
};

// Configuración global de Chart.js
if (typeof Chart !== "undefined") {
  Chart.defaults.font.family = "'Segoe UI', 'Arial', sans-serif";
  Chart.defaults.font.size = 12;
  Chart.defaults.color = "#5a5c69";
  Chart.defaults.borderColor = "rgba(0, 0, 0, 0.1)";
  Chart.defaults.backgroundColor = "rgba(138, 21, 56, 0.1)";
}

// Event listeners para responsividad
window.addEventListener("resize", function () {
  setTimeout(function () {
    if (typeof window.resizeCharts === "function") {
      window.resizeCharts();
    }
  }, 100);
});

// Limpiar gráficos cuando se abandona la página
window.addEventListener("beforeunload", function () {
  if (typeof window.destroyCharts === "function") {
    window.destroyCharts();
  }
});

// Log para confirmar que el script se cargó
console.log("Script admin-estadisticas.js cargado correctamente");
