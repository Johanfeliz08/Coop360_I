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
