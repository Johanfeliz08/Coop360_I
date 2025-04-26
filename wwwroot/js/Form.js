document.addEventListener("DOMContentLoaded", function () {
  const frm = document.getElementById("form");
  const btnSubmit = document.getElementById("btn-submit");

  // Valida que el formulario y el botón de submit existan
  if (frm != null && btnSubmit != null) {
    btnSubmit.addEventListener("click", function () {
      frm.submit();
    });
  }

  // Obtener todos los elementos del formulario con la propiedad "required"
  const requiredElements = document.querySelectorAll("[required]");
  btnSubmit.disabled = true; // Deshabilitar el botón de submit inicialmente

  // Recibe un elemento, y muestra un error si esta vacio o es default
  const validarInput = (Element) => {
    const error = document.getElementById("error" + Element.id);

    // Verificar si hay errores previos y eliminarlos
    if (error != null) {
      error.remove();
    }

    // Si esta vacio, renderizar un error de que es requerido
    if (Element.value == "" || Element.value == "default") {
      Element.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error" + Element.id;
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "8px 0px";
      error.textContent = "El campo es requerido";
      btnSubmit.disabled = true;
      Element.after(error);
    } else {
      Element.style.outline = "none";
    }
  };

  // Recibe un array de elementos, y valida si todos son validos
  const validarFormulario = (requiredElements) => {
    let isValid = true;
    requiredElements.forEach((Element) => {
      if (Element.value == "" || Element.value == "default") {
        isValid = false;
      }
    });
    btnSubmit.disabled = !isValid; // Habilitar o deshabilitar el botón de submit
  };

  // Iterar sobre los inputs requeridos y agregar los eventos para validar
  if (requiredElements.length > 0) {
    requiredElements.forEach((Element) => {
      Element.addEventListener("blur", () => {
        validarInput(Element);
        validarFormulario(requiredElements);
      });
      Element.addEventListener("input", () => {
        validarInput(Element);
        validarFormulario(requiredElements);
      });
    });
  }

  // Validar el formulario al cargar la página
  validarFormulario(requiredElements);
});
