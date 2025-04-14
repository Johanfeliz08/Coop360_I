// ESTABLECER LA TASA DE INTERES BASE, PLAZO BASE, ACTUALIZAR MONTO POR CUOTA Y CANTIDAD DE CUOTAS EN BASE A LA CATEGORIA DE PRESTAMO

// Inputs
const categoriaPrestamoInput = document.getElementById("id_categoria_prestamo");
const montoSolicitadoInput = document.getElementById("monto_solicitado");
const plazoMesesInput = document.getElementById("plazo_meses");
const tasaInteresInput = document.getElementById("tasa_interes");
const cantidadCuotasInput = document.getElementById("cantidad_cuotas");
const montoPorCuotaInput = document.getElementById("monto_por_cuota");

// Establecer el plazo meses estandar, la tasa de interes estandar basado en la categoria de prestamo
const asignarPlazoTasaInteres = async () => {
  // Obtener las categorias de prestamos
  const categoriasPrestamos = await fetch(
    "/Prestamos/obtenerCategoriaPrestamo/"
  )
    .then((response) => response.json())
    .then((data) => {
      // Validar que no este seleccionada la opcion default
      if (categoriaPrestamoInput != "default") {
        // Recorrer las categorias de prestamos
        data.map((categoria) => {
          // Validar que la categoria seleccionada sea la misma que la categoria de prestamo
          if (categoria.ID_CATEGORIA_PRESTAMO == categoriaPrestamoInput.value) {
            // Establecer el plazo meses estandar, la tasa de interes estandar basado en la categoria de prestamo
            plazoMesesInput.value = categoria.PLAZO_ESTANDAR;
            tasaInteresInput.value = categoria.TASA_INTERES_BASE;
          }
        });
      } else {
        console.log("Debe seleccionar una categoria de prestamo");
      }
    });
};

// Asignar el evento al input de la categoria de prestamo
categoriaPrestamoInput.addEventListener("change", () => {
  asignarPlazoTasaInteres();
});

// Asignar la cantidad de cuotas y el monto por cuotas basado en el plazo y la tasa de interes
const asignarCuotasMontoCuota = () => {
  const plazoMeses = plazoMesesInput.value;

  // Calcular la cantidad de cuotas
  const cantidadCuotas = plazoMeses * 1;
  cantidadCuotasInput.value = cantidadCuotas;

  // Calcular el monto por cuota
  const montoPorCuota = montoSolicitadoInput.value / cantidadCuotas;
  montoPorCuotaInput.value = montoPorCuota.toLocaleString("en-US", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
};

// Asignar el evento al input de la cantidad de cuotas
montoSolicitadoInput.addEventListener("blur", () => {
  if (
    categoriaPrestamoInput.value != "default" &&
    plazoMesesInput.value != "" &&
    tasaInteresInput.value != "" &&
    montoSolicitadoInput.value != ""
  ) {
    asignarCuotasMontoCuota();
  }
});

// FILTRA QUE NO SE PUEDA ELEGIR UN CODEUDOR DOS VECES E OBLIGA AL USUARIO A SELECCIONARLOS POR ORDEN

const codeudorPrincipalSelect = document.getElementById(
  "codigo_codeudor_principal"
);

const codeudorSecundarioSelect = document.getElementById(
  "codigo_codeudor_secundario"
);

const codeudorTerciarioSelect = document.getElementById(
  "codigo_codeudor_terciario"
);

let codeudoresPrincipalesOptions = document.querySelectorAll(
  "#codigo_codeudor_principal option"
);

let codeudoresSecundariosOptions = document.querySelectorAll(
  "#codigo_codeudor_secundario option"
);

let codeudoresTerciariosOptions = document.querySelectorAll(
  "#codigo_codeudor_terciario option"
);

// Oculta las opciones de los codeudores secundarios y terciarios si el codeudor principal es default y obliga a que se seleccione los codeudores por orden
const codeudorPrincipalValue = codeudorPrincipalSelect.value;
const codeudorSecundarioValue = codeudorSecundarioSelect.value;

if (codeudorPrincipalValue == "default") {
  codeudoresSecundariosOptions.forEach((option) => {
    if (option.value != "default") {
      option.style.display = "none";
    }
  });

  if (
    codeudorPrincipalValue == "default" ||
    codeudorSecundarioValue == "default"
  ) {
    codeudoresTerciariosOptions.forEach((option) => {
      if (option.value != "default") {
        option.style.display = "none";
      }
    });
  }
}

codeudorPrincipalSelect.addEventListener("change", () => {
  // Obtener el valor del codeudor principal
  const codeudorPrincipalValue = codeudorPrincipalSelect.value;

  // Resetear los codeudores secundarios y terciarios
  codeudorTerciarioSelect.value = "default";
  codeudorSecundarioSelect.value = "default";

  // Filtrar los codeudores secundarios y terciarios
  codeudoresSecundariosOptions.forEach((option) => {
    if (option.value == codeudorPrincipalValue) {
      option.style.display = "none";
    } else {
      option.style.display = "block";
    }
  });

  if (codeudorSecundarioValue != "default") {
    codeudoresTerciariosOptions.forEach((option) => {
      if (option.value == codeudorPrincipalValue) {
        option.style.display = "none";
      } else {
        option.style.display = "block";
      }
    });
  }
});

codeudorSecundarioSelect.addEventListener("change", () => {
  // Obtener el valor del codeudor principal
  const codeudorPrincipalValue = codeudorPrincipalSelect.value;

  // Obtener el valor del codeudor secundario
  const codeudorSecundarioValue = codeudorSecundarioSelect.value;

  // Resetear los codeudores  terciarios
  codeudorTerciarioSelect.value = "default";

  // Filtrar los codeudores terciarios
  codeudoresTerciariosOptions.forEach((option) => {
    if (
      option.value == codeudorSecundarioValue ||
      option.value == codeudorPrincipalValue
    ) {
      option.style.display = "none";
    } else {
      option.style.display = "block";
    }
  });
});
