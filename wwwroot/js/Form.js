document.addEventListener("DOMContentLoaded", function () {
  const frm = document.getElementById("form");
  const btnSubmit = document.getElementById("btn-submit");

  // Valida que el formulario y el botón de submit existan
  if (frm != null && btnSubmit != null) {
    btnSubmit.addEventListener("click", function () {
      frm.submit();
    });
  }
});
