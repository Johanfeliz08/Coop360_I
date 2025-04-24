const dialog = document.querySelector("dialog");

// Codigo general para cerrar el modal
if (dialog != null) {
  const btnCerrar = document.getElementById("btn-cerrar");
  btnCerrar.addEventListener("click", (e) => {
    e.preventDefault();
    dialog.close();
  });

  const btnCancelar = document.getElementById("btn-cancelar");
  btnCancelar.addEventListener("click", (e) => {
    e.preventDefault();
    dialog.close();
  });
}

const frmAprobacion = document.getElementById("frmAprobacion");
const frmRechazo = document.getElementById("frmRechazo");

// Validaciones para el formulario de aprobación
// Verifica si el formulario de aprobación existe en el DOM
if (frmAprobacion != null) {
  // Obtener los elementos del formulario
  const inputMontoSolicitado = document.getElementById("monto_solicitado");
  const inputMontoAprobado = document.getElementById("monto_aprobado");
  const textareaNotaAprobacion = document.getElementById("nota_aprobacion_rechazo");

  // Deshabilitar el botón de envío al cargar la página
  const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = true);

  // Evento de cambio al campo de monto solicitado para validar el monto aprobado
  inputMontoAprobado.addEventListener("change", (e) => {
    let montoSolicitado = inputMontoSolicitado.value;
    let montoAprobado = inputMontoAprobado.value;

    const error = document.getElementById("error_monto_aprobado");
    const errorNotaAprobacion = document.getElementById("error_nota_aprobacion");

    // Verificar si hay errores previos y eliminarlos
    if (error != null) {
      error.remove();
    }

    // Verificar si hay errores previos en la nota de aprobación y eliminarlos

    if (errorNotaAprobacion != null) {
      errorNotaAprobacion.remove();
    }

    // Validar el monto aprobado no sea mayor al monto solicitado

    if (Number(montoAprobado) > Number(montoSolicitado)) {
      inputMontoAprobado.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_monto_aprobado";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "El monto aprobado no puede ser mayor al monto solicitado";
      inputMontoAprobado.after(error);
      const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = true);

      // Validar si el monto aprobado es menor al solicitado, para hacer la nota de aprobación obligatoria
    } else if (Number(montoAprobado) < Number(montoSolicitado) && textareaNotaAprobacion.value == "") {
      inputMontoAprobado.style.outline = "none";
      textareaNotaAprobacion.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_nota_aprobacion";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "10px 0px";
      error.textContent = "La nota de aprobación es requerida cuando el monto aprobado es menor al monto solicitado";
      textareaNotaAprobacion.after(error);
      const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = true);
    } else {
      inputMontoAprobado.style.outline = "none";
      textareaNotaAprobacion.style.outline = "none";
      const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = false);
    }
  });

  textareaNotaAprobacion.addEventListener("change", (e) => {
    let montoSolicitado = inputMontoSolicitado.value;
    let montoAprobado = inputMontoAprobado.value;

    const error = document.getElementById("error_nota_aprobacion");

    if (error != null) {
      error.remove();
    }

    if (Number(montoAprobado) < Number(montoSolicitado) && textareaNotaAprobacion.value == "") {
      textareaNotaAprobacion.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_nota_aprobacion";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La nota de aprobación es requerida cuando el monto aprobado es menor al monto solicitado";
      textareaNotaAprobacion.after(error);
      const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = true);
    } else {
      inputMontoAprobado.style.outline = "none";
      textareaNotaAprobacion.style.outline = "none";
      const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = false);
    }
  });
}

// Validaciones para el formulario de rechazo
if (frmRechazo != null) {
  const inputMontoSolicitado = document.getElementById("monto_solicitado");
  const inputMontoAprobado = document.getElementById("monto_aprobado");
  const textareaNotaAprobacion = document.getElementById("nota_aprobacion_rechazo");
  const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = true);

  textareaNotaAprobacion.addEventListener("change", (e) => {
    const error = document.getElementById("error_nota_aprobacion");

    if (error != null) {
      error.remove();
    }

    if (textareaNotaAprobacion.value == "") {
      textareaNotaAprobacion.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_nota_aprobacion";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La nota de rechazo es requerida";
      textareaNotaAprobacion.after(error);
      const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = true);
    } else {
      textareaNotaAprobacion.style.outline = "none";
      const submitBtn = (document.getElementById("btn-aprobacionrechazo").disabled = false);
    }
  });
}

// Validaciones para el formulario de cambio de contraseña
const frmCambioContrasena = document.getElementById("frmCambioContrasena");

// Verifica si el formulario de cambio de contraseña existe en el DOM
if (frmCambioContrasena != null) {
  // Obtener los elementos del formulario
  const inputContrasenaActual = document.getElementById("contrasenaActual");
  const inputContrasenaNueva = document.getElementById("contrasenaNueva");
  const inputConfirmarContrasena = document.getElementById("confirmarContrasena");

  // Valida el formulario y habilita el botón de envío si no hay errores
  const validarFrmCambiarContrasena = () => {
    const errorContrasenaActual = document.getElementById("error_contrasena_actual");
    const errorContrasenaNueva = document.getElementById("error_contrasena_nueva");
    const errorContrasenaNuevaActual = document.getElementById("error_contrasena_nueva_actual");
    const errorConfirmarContrasena = document.getElementById("error_confirmar_contrasena");

    if (errorContrasenaActual == null && errorContrasenaNueva == null && errorContrasenaNuevaActual == null && errorConfirmarContrasena == null) {
      const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = false);
    }
  };

  // Deshabilitar el botón de envío al cargar la página
  const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = true);

  // Evento de cambio al campo de contraseña actual
  inputContrasenaActual.addEventListener("blur", (e) => {
    const error = document.getElementById("error_contrasena_actual");

    // Verificar si hay errores previos y eliminarlos
    if (error != null) {
      error.remove();
    }

    // Validar que la contraseña actual no esté vacía
    if (inputContrasenaActual.value == "") {
      inputContrasenaActual.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_contrasena_actual";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La contraseña actual es requerida";
      inputContrasenaActual.after(error);
      const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = true);
    } else {
      inputContrasenaActual.style.outline = "none";
    }
  });

  // Evento de cambio al campo de nueva contraseña
  inputContrasenaNueva.addEventListener("blur", (e) => {
    const error = document.getElementById("error_contrasena_nueva");
    const errorContrasenaActual = document.getElementById("error_contrasena_nueva_actual");

    // Verificar si hay errores previos y eliminarlos
    if (error != null) {
      error.remove();
    }

    if (errorContrasenaActual != null) {
      errorContrasenaActual.remove();
    }

    // Validar que la nueva contraseña no esté vacía
    if (inputContrasenaNueva.value == "") {
      inputContrasenaNueva.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_contrasena_nueva";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La nueva contraseña es requerida";
      inputContrasenaNueva.after(error);
      const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = true);
    } else if (inputContrasenaActual.value == inputContrasenaNueva.value) {
      inputContrasenaNueva.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_contrasena_nueva_actual";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La nueva contraseña no puede ser igual a la contraseña actual";
      inputContrasenaNueva.after(error);
      const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = true);
    } else {
      inputContrasenaNueva.style.outline = "none";
    }
  });

  inputConfirmarContrasena.addEventListener("blur", (e) => {
    const errorConfirmarContrasena = document.getElementById("error_confirmar_contrasena");
    const errorContrasenaNueva = document.getElementById("error_contrasena_nueva");

    // Verificar si hay errores previos y eliminarlos
    if (errorConfirmarContrasena != null) {
      errorConfirmarContrasena.remove();
    }

    if (errorContrasenaNueva != null) {
      errorContrasenaNueva.remove();
    }

    // Validar que la nueva contraseña no esté vacía
    if (inputContrasenaNueva.value == "") {
      inputContrasenaNueva.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_contrasena_nueva";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La nueva contraseña es requerida";
      inputContrasenaNueva.after(error);
      const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = true);

      // Validar que la confirmación de la nueva contraseña no esté vacía
    } else if (inputConfirmarContrasena.value == "") {
      inputConfirmarContrasena.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_confirmar_contrasena";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La confirmación de la nueva contraseña es requerida";
      inputConfirmarContrasena.after(error);
      const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = true);

      // Validar que la nueva contraseña y la confirmación no coincidan
    } else if (inputContrasenaNueva.value != inputConfirmarContrasena.value) {
      inputConfirmarContrasena.style.outline = "1px solid red";
      const error = document.createElement("p");
      error.id = "error_confirmar_contrasena";
      error.style.color = "red";
      error.style.fontSize = "12px";
      error.style.margin = "0px";
      error.style.padding = "0px";
      error.textContent = "La nueva contraseña y la confirmación no coinciden";
      inputConfirmarContrasena.after(error);
      const btnCambiarContrasena = (document.getElementById("btn-cambiarContrasena").disabled = true);
    } else {
      inputConfirmarContrasena.style.outline = "none";
      validarFrmCambiarContrasena();
    }
  });
}
