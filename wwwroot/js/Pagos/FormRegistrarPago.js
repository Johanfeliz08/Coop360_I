const socioInput = document.getElementById("socio");
const prestamoInput = document.getElementById("prestamos");

// Actualiza los prestamos en base al socio seleccionado
if (socioInput != null && prestamoInput != null) {
  if (socioInput.value == "default") {
    // Oculta las opciones de los prestamos si el socio es default
    let prestamosOption = document.querySelectorAll("#prestamos option");

    prestamosOption.forEach((option) => {
      if (option.value != "default") {
        option.style.display = "none";
      }
    });
  }

  socioInput.addEventListener("change", () => {
    // Obtener el valor del socio
    const socioValue = socioInput.value;

    // Resetear el prestamo
    prestamoInput.value = "default";
    // Resetear los campos
    const numeroCuotaInput = (document.getElementById("numero_cuota").value =
      "");
    const montoaPagarInput = (document.getElementById("monto_a_pagar").value =
      "");

    // Filtrar los prestamos
    let prestamosOption = document.querySelectorAll("#prestamos option");

    prestamosOption.forEach((option) => {
      if (option.getAttribute("socio") != socioValue) {
        // Si el socio no es el mismo que el del prestamo, ocultar la opción
        option.style.display = "none";
      } else {
        option.style.display = "block";
      }
    });

    // Busca las opciones visibles de los prestamos
    let opcionesVisibles = Array.from(
      document.querySelectorAll("#prestamos option")
    ).filter((option) => getComputedStyle(option).display === "block");

    // Si no hay opciones visibles, esto quiere decir que el socio no tiene prestamos disponibles, por lo que se renderiza un mensaje
    if (opcionesVisibles.length == 0) {
      const opcionNoPrestamos = document.getElementById("nodata");
      opcionNoPrestamos.style.display = "block";
      prestamoInput.value = "nodata";
    }
  });
}

// Funcion que obtiene los datos del prestamo y actualiza los inptus del formulario

const obtenerDatosPrestamo = async () => {
  if (
    prestamoInput != null &&
    prestamoInput.value != "default" &&
    prestamoInput.value != "nodata"
  ) {
    // Obtener el ID del prestamo seleccionado
    let ID_PRESTAMO = Number(prestamoInput.value);

    if (ID_PRESTAMO > 0) {
      // Obtener los datos del prestamo

      try {
        const response = await fetch(`/Pagos/ObtenerPrestamo/${ID_PRESTAMO}`);
        if (!response.ok) {
          throw new Error(`Error: ${response.status} ${response.statusText}`);
        }
        const data = await response.json();
        const numeroCuotaInput = document.getElementById("numero_cuota");
        const montoaPagarInput = document.getElementById("monto_a_pagar");

        if (numeroCuotaInput != null) {
          numeroCuotaInput.value = data.NUMERO_CUOTA.NUMERO_CUOTA_ACTUAL + 1;
        }

        if (montoaPagarInput != null) {
          let MONTO_INTERES = Number(data.PRESTAMO.BALANCE_RESTANTE) * (Number(data.PRESTAMO.TASA_INTERES) / 100);
          let IMPUESTO = MONTO_INTERES * 0.1;
          let MONTO_A_PAGAR = (Number(data.PRESTAMO.MONTO_APROBADO) / Number(data.PRESTAMO.CANTIDAD_CUOTAS)) + MONTO_INTERES + IMPUESTO;
          montoaPagarInput.value = MONTO_A_PAGAR;
        }
      } catch (error) {
        console.error("Ha ocurrido un error al obtener el prestamo", error);
      }
    } else {
      console.error("Id Prestamo no valido");
    }
  }
};

prestamoInput.addEventListener("change", (e) => {
  if (
    prestamoInput != null &&
    prestamoInput.value != "default" &&
    prestamoInput.value != "nodata"
  ) {
    obtenerDatosPrestamo();
  }
});

// Actualizar el monto a devolver en base al monto a pagar y la cantidad recibida

const montoRecibidoInput = document.getElementById("monto_recibido");
const montoaDevolverInput = document.getElementById("monto_a_devolver");
const montoaPagarInput = document.getElementById("monto_a_pagar");

if (montoRecibidoInput != null && montoaDevolverInput != null) {
  montoRecibidoInput.addEventListener("change", (e) => {
    if (montoRecibidoInput.value > 0) {
      let montoRecibido = Number(montoRecibidoInput.value);
      let montoAPagar = Number(montoaPagarInput.value);
      let montoADevolver = montoRecibido - montoAPagar;
      montoaDevolverInput.value = montoADevolver;
    }
  });
}

// Validar que el monto devuelto no sea mayor al monto a devolver

const montoDevueltoInput = document.getElementById("monto_devuelto");

if (montoDevueltoInput != null && montoaDevolverInput != null) {
  montoDevueltoInput.addEventListener("change", (e) => {
    const error = document.getElementById("error-monto-devuelto");

    if (error) {
      error.remove();
    }

    if (Number(montoDevueltoInput.value) > Number(montoaDevolverInput.value)) {
      let montoADevolver = Number(montoaDevolverInput.value);
      let montoDevuelto = Number(montoDevueltoInput.value);
      if (montoDevuelto > montoADevolver) {
        montoDevueltoInput.style.outline = "1px solid red";
        const error = document.createElement("p");
        error.id = "error-monto-devuelto";
        error.style.color = "red";
        error.style.fontSize = "12px";
        error.textContent =
          "El monto devuelto no puede ser mayor al monto a devolver";
        montoDevueltoInput.after(error);
      }
    } else if (
      Number(montoDevueltoInput.value) < Number(montoaDevolverInput.value)
    ) {
      montoDevueltoInput.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error-monto-devuelto";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.textContent =
        "El monto devuelto no puede ser menor al monto a devolver";
      montoDevueltoInput.after(error);
    } else {
      montoDevueltoInput.style.outline = "none";
    }
  });
}
