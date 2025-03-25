document.addEventListener("DOMContentLoaded", function () {
  const frm = document.getElementById("form");
  const btnSubmit = document.getElementById("btn-submit");

  if (frm != null && btnSubmit != null) {
    btnSubmit.addEventListener("click", function () {
      frm.submit();
    });
  }
});
