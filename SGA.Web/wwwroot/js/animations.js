// Script para animaciones y mejoras visuales
document.addEventListener("DOMContentLoaded", function () {
  // Agregar clase de animación a elementos principales
  const elementsToAnimate = document.querySelectorAll(
    ".register-card, .card, h1, h2, h3"
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
      this.closest(".register-form-group")?.classList.add("focused");
    });

    input.addEventListener("blur", function () {
      if (this.value === "") {
        this.closest(".register-form-group")?.classList.remove("focused");
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
