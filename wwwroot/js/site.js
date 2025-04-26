// Boton volver

const btnsRegresar = document.querySelectorAll(".btn-regresar");
if (btnsRegresar != null) {
  btnsRegresar.forEach((btn) => {
    btn.addEventListener("click", function (e) {
      e.preventDefault();
      window.history.back();
    });
  });
}
