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

  // Renderizar provincias, ciudades y sectores dependiendo su padre

  // Obtener los select de provincia, ciudad y sector
  const selectProvincia = document.getElementById("provincia");
  const selectCiudad = document.getElementById("ciudad");
  const selectSector = document.getElementById("sector");

  // Verificar que los selects existan y luego ocultar opciones dependiendo la seleccion
  if (selectProvincia != null && selectCiudad != null && selectSector != null) {
    // Obtener todas las opciones de los selects

    const provinciasOptions = selectProvincia.querySelectorAll("option");
    const ciudadesOptions = selectCiudad.querySelectorAll("option");
    const sectoresOptions = selectSector.querySelectorAll("option");

    // Ocultar todas las opciones de ciudad y sector al cargar la pagina
    ciudadesOptions.forEach((ciudad) => {
      if (ciudad.value != "default") {
        ciudad.style.display = "none";
      }
    });
    sectoresOptions.forEach((sector) => {
      if (sector.value != "default") {
        sector.style.display = "none";
      }
    });

    selectProvincia.addEventListener("change", (e) => {
      // Obtener el ID de la provincia que esta seleccionada actualmente
      const IdProvincia = selectProvincia.value;
      // Resetear la ciudad y el sector
      selectCiudad.value = "default";
      selectSector.value = "default";

      // Validar que no sea la opcion default
      if (IdProvincia != "default") {
        ciudadesOptions.forEach((ciudad) => {
          if (ciudad.getAttribute("provincia") == IdProvincia || ciudad.value == "default") {
            // Si la ciudad tiene el mismo ID de provincia, mostrarla
            ciudad.style.display = "block";
          } else {
            ciudad.style.display = "none";
          }
        });

        const ciudadesVisibles = Array.from(ciudadesOptions).filter((ciudad) => getComputedStyle(ciudad).display === "block" && ciudad.value != "default");

        if (ciudadesVisibles.length <= 0) {
          // Si no hay ciudades disponibles, renderizar un mensaje
          const opcionNoCiudades = document.getElementById("nociudades");
          console.log(opcionNoCiudades);
          opcionNoCiudades.style.display = "block";
          selectCiudad.value = "nociudades";
        }
      }
    });

    selectCiudad.addEventListener("change", (e) => {
      const IdCiudad = selectCiudad.value;
      // Resetear el sector
      selectSector.value = "default";

      if (IdCiudad != "default") {
        sectoresOptions.forEach((sector) => {
          if (sector.getAttribute("ciudad") == IdCiudad || sector.value == "default") {
            // Si el sector tiene el mismo ID de ciudad, mostrarlo
            sector.style.display = "block";
          } else {
            sector.style.display = "none";
          }
        });

        const sectoresVisibles = Array.from(sectoresOptions).filter((sector) => getComputedStyle(sector).display === "block" && sector.value != "default");

        if (sectoresVisibles.length <= 0) {
          // Si no hay sectores disponibles, renderizar un mensaje
          const opcionNoSectores = document.getElementById("nosectores");
          console.log(opcionNoSectores);
          opcionNoSectores.style.display = "block";
          selectSector.value = "nosectores";
        }
      }
    });
  }
});
