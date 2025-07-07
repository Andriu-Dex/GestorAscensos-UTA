// Utilidades para manejo de fechas en zona horaria de Ecuador
window.EcuadorDateHelper = {
  /**
   * Convierte una fecha UTC a la zona horaria de Ecuador (UTC-5)
   * @param {string|Date} utcDate - Fecha en UTC
   * @returns {Date} Fecha convertida a zona horaria de Ecuador
   */
  convertToEcuadorTime: function (utcDate) {
    const date = new Date(utcDate);
    // Ecuador está en UTC-5
    const ecuadorOffset = -5 * 60; // en minutos
    const utcTime = date.getTime() + date.getTimezoneOffset() * 60000;
    const ecuadorTime = new Date(utcTime + ecuadorOffset * 60000);
    return ecuadorTime;
  },

  /**
   * Formatea una fecha UTC para mostrarla en hora de Ecuador
   * @param {string|Date} utcDate - Fecha en UTC
   * @param {string} format - Formato de salida ('short', 'long', 'datetime')
   * @returns {string} Fecha formateada en hora de Ecuador
   */
  formatEcuadorDate: function (utcDate, format = "datetime") {
    const ecuadorDate = this.convertToEcuadorTime(utcDate);

    const options = {
      timeZone: "America/Guayaquil", // Zona horaria de Ecuador
      locale: "es-ES",
    };

    switch (format) {
      case "short":
        return ecuadorDate.toLocaleDateString("es-ES", {
          ...options,
          day: "2-digit",
          month: "2-digit",
          year: "numeric",
        });
      case "long":
        return ecuadorDate.toLocaleDateString("es-ES", {
          ...options,
          day: "numeric",
          month: "long",
          year: "numeric",
        });
      case "time":
        return ecuadorDate.toLocaleTimeString("es-ES", {
          ...options,
          hour: "2-digit",
          minute: "2-digit",
        });
      case "datetime":
      default:
        return ecuadorDate.toLocaleString("es-ES", {
          ...options,
          day: "2-digit",
          month: "2-digit",
          year: "numeric",
          hour: "2-digit",
          minute: "2-digit",
        });
    }
  },

  /**
   * Obtiene la fecha y hora actual en Ecuador
   * @returns {Date} Fecha actual en zona horaria de Ecuador
   */
  getEcuadorNow: function () {
    return this.convertToEcuadorTime(new Date());
  },

  /**
   * Calcula el tiempo transcurrido desde una fecha UTC hasta ahora (en Ecuador)
   * @param {string|Date} utcDate - Fecha UTC de referencia
   * @returns {string} Tiempo transcurrido formateado
   */
  getTimeAgo: function (utcDate) {
    const ecuadorDate = this.convertToEcuadorTime(utcDate);
    const now = this.getEcuadorNow();
    const diffMs = now - ecuadorDate;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return "Ahora";
    if (diffMins < 60) return `${diffMins}m`;
    if (diffHours < 24) return `${diffHours}h`;
    if (diffDays < 7) return `${diffDays}d`;

    return this.formatEcuadorDate(utcDate, "short");
  },
};

// Función global para formatear fechas de revisión
window.formatearFechaRevision = function (fechaUtc, incluirHora = true) {
  if (!fechaUtc) return "";

  const formato = incluirHora ? "datetime" : "short";
  return window.EcuadorDateHelper.formatEcuadorDate(fechaUtc, formato);
};

console.log("✅ Ecuador Date Helper cargado correctamente");
