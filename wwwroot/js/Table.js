// document.addEventListener("DOMContentLoaded", function () {
//   // Obtiene el codido_empleado de la fila seleccionada para editar

//   const botonesEditar = document.querySelectorAll(".accion-editar");

//   if (botonesEditar != null) {
//     botonesEditar.forEach((btneditar) => {
//       btneditar.addEventListener("click", (e) => {
//         e.preventDefault();
//         const codigo_empleado = btneditar.dataset.codigoEmpleado;
//         btneditar.setAttribute("href", `@Url.Action("Editar", "Empleado")/${codigo_empleado}`);
//       });
//     });
//   }

//   // const botonesEditar = document
//   //   .querySelectorAll(".accion-editar")
//   //   .forEach((btneditar) => {
//   //     if (botonesEditar != null) {
//   //       console.log("Boton editar no es nullo");
//   //     }

//   //     btneditar.addEventListener("Click", () => {
//   //       preventDefault();
//   //       const codigo_empleado = btneditar.getAttribute("data-id");
//   //       console.log("Codigo empleado:", codigo_empleado);
//   //     });
//   //   });
// });
