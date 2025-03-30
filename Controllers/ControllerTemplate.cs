// using System.Diagnostics;
// using Microsoft.AspNetCore.Mvc;
// using Coop360_I.Models;
// using Microsoft.EntityFrameworkCore;
// using Coop360_I.Data;
// using Coop360_I.Controllers;

// namespace Coop360_I.Controllers;

// public class NameController : Controller {
//     private readonly ILogger<NameController> _logger;
//     private readonly ApplicationDbContext _context;

//     public NameController(ApplicationDbContext context, ILogger<NameController> logger) {
//         _logger = logger;
//         _context = context;
//     }

//     // Views

//     // View con tabla que muestra los datos
//     [HttpGet]
//     public IActionResult RegistroName() {

//         // var EntityName = _context.TableName
//         // .FromSqlRaw("EXEC ProcedureName")
//         // .AsEnumerable()
//         // .ToList();

//         return View("EntityName");
    
//     }
    
//     // View con formulario para crear empleado

//     public IActionResult Crear () {

//         // Obtiene los datos necesarios para la vista 

//         // var regiones = _context.Regiones
//         // .FromSqlRaw("EXEC SP_LEER_REGIONES")
//         // .AsEnumerable()
//         // .ToList();


//         // Validacion de los datos
//         // if (provincias == null || ciudades == null || sectores == null || puestos == null || departamentos == null || nivelesAprobacion == null || entidadesBancarias == null) {
//         //     throw new Exception("Error al cargar los datos del formulario");
//         // }

//         // Asignacion de los datos necesarios para la vista, al viewModel

//         // var viewModel = new nameViewModel
//         // {
//         //     // Regiones = regiones,
//         //     // Provincias = provincias,
//         //     // Ciudades = ciudades,
//         //     // Sectores = sectores,
//         //     // Puestos = puestos,
//         //     // Departamentos = departamentos,
//         //     // NivelesAprobacion = nivelesAprobacion,
//         //     // EntidadesBancarias = entidadesBancarias
//         // };



//         // ViewBag.Title = "ViewTitle";
//         // return View("ViewName",viewModel);
//     }

//     // View con formulario para editar empleado

//      [HttpGet]
//     public IActionResult Editar(string codigoEmpleado) {

//         // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
//         if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
//                 return RedirectToAction("Login", "Auth");
//         }

//         // // Valida la clave primaria necesaria para editar la entidad
//         // if (codigoEmpleado != null) {

        
//         // var provincias = _context.Provincias
//         // .FromSqlRaw("EXEC SP_LEER_PROVINCIAS")
//         // .AsEnumerable()
//         // .ToList();

//         // var ciudades = _context.Ciudades
//         // .FromSqlRaw("EXEC SP_LEER_CIUDADES")
//         // .AsEnumerable()
//         // .ToList();

//         // var sectores = _context.Sectores
//         // .FromSqlRaw("EXEC SP_LEER_SECTORES")
//         // .AsEnumerable()
//         // .ToList();

//         // var puestos = _context.Puestos
//         // .FromSqlRaw("EXEC SP_LEER_PUESTOS")
//         // .AsEnumerable()
//         // .ToList();

//         // var departamentos = _context.Departamentos
//         // .FromSqlRaw("EXEC SP_LEER_DEPARTAMENTOS")
//         // .AsEnumerable()
//         // .ToList();

//         // var nivelesAprobacion = _context.NivelesAprobacion
//         // .FromSqlRaw("EXEC SP_LEER_NIVELES_APROBACION")
//         // .AsEnumerable()
//         // .ToList();

//         // var entidadesBancarias = _context.EntidadesBancarias
//         // .FromSqlRaw("EXEC SP_LEER_ENTIDADES_BANCARIAS")
//         // .AsEnumerable()
//         // .ToList();

//         // if (provincias == null || ciudades == null || sectores == null || puestos == null || departamentos == null || nivelesAprobacion == null || entidadesBancarias == null) {
//         //     throw new Exception("Error al cargar los datos del formulario");
//         // }

//         // var codigo_empleado = Convert.ToInt32(codigoEmpleado);    
//         // var empleado = _context.Empleados
//         // .FromSqlRaw("EXEC SP_BUSCAR_EMPLEADO @CODIGO_EMPLEADO = {0}", codigo_empleado)
//         // .AsEnumerable()
//         // .FirstOrDefault();
    
//         // var viewModel = new EmpleadoViewModel
//         // {
//         //     Provincias = provincias,
//         //     Ciudades = ciudades,
//         //     Sectores = sectores,
//         //     Puestos = puestos,
//         //     Departamentos = departamentos,
//         //     NivelesAprobacion = nivelesAprobacion,
//         //     EntidadesBancarias = entidadesBancarias,
//         //     Empleado = empleado
//         // };
        
//         // ViewBag.Title = "Editar Empleado";
//         // return View("FormEmpleados",viewModel);

//         // } else {

//         //     ViewBag.Error = "Codigo de empleado no valido";
//         //     return RedirectToAction("RegistroEmpleados");

//         // }

    
//     }

//     // View modal de confirmacion para eliminar empleado
//     [HttpGet]

//     public IActionResult Eliminar(string CodigoEmpleado) {
            
//         // if (CodigoEmpleado != null ){

//         //     int codigo_empleado = Convert.ToInt32(CodigoEmpleado);
//         //     TempData["openModal"] = true;
//         //     TempData["Tipo"] = "confirmation";
//         //     TempData["Titulo"] = "¡Cuidado!";
//         //     TempData["Confirmation"] = "¿Esta seguro que desea eliminar este empleado?";
//         //     TempData["CodigoEmpleado"] = codigo_empleado;
//         //     return RedirectToAction("RegistroEmpleados");
//         // }

//         //     return RedirectToAction("RegistroEmpleados");
//     }   


//     // Acciones

//     // Guardar empleado

//     [HttpPost]
//     public async Task<IActionResult> GuardarAsync(Empleado empleado) {

//         // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
//         if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
//                 return RedirectToAction("Login", "Auth");
//         } else {
//                 // Asigna el codigo del empleado que esta creando el registro
//                 empleado.CREADO_POR = HttpContext.Session.GetInt32("ID_USUARIO");
//         };

//         // // Valida el empleado y devuelve un objeto con los campos validados
//         // var empleadoValido = validarEmpleado(empleado);

//         // try {
//         // await _context.Database.ExecuteSqlRawAsync(
//         //     "EXEC SP_CREAR_EMPLEADO @CEDULA = {0}, @P_NOMBRE = {1}, @S_NOMBRE = {2}, @P_APELLIDO = {3}, @S_APELLIDO = {4}, @DIRECCION = {5}, @ID_SECTOR = {6}, @ID_CIUDAD = {7}, @ID_PROVINCIA = {8}, @PAIS_NACIMIENTO = {9}, @TELEFONO_PRINCIPAL = {10}, @TELEFONO_SECUNDARIO = {11}, @FECHA_NACIMIENTO = {12}, @EMAIL = {13}, @SEXO = {14}, @ESTADO_CIVIL = {15}, @FRECUENCIA_COBRO = {16}, @CUENTA_BANCO = {17}, @ID_ENTIDAD_BANCARIA = {18}, @ID_PUESTO = {19}, @ID_DEPARTAMENTO = {20}, @TIPO_SANGRE = {21}, @NOMBRE_FAMILIAR_PRIMARIO = {22}, @TELEFONO_FAMILIAR_PRIMARIO = {23}, @PARENTESCO_FAMILIAR_PRIMARIO = {24}, @NOMBRE_FAMILIAR_SECUNDARIO = {25}, @TELEFONO_FAMILIAR_SECUNDARIO = {26}, @PARENTESCO_FAMILIAR_SECUNDARIO = {27}, @ID_NIVEL_APROBACION = {28}, @ESTATUS = {29}, @CREADO_POR = {30}",
//         //     empleadoValido.CEDULA, empleadoValido.P_NOMBRE, empleadoValido.S_NOMBRE ?? "", empleadoValido.P_APELLIDO, empleadoValido.S_APELLIDO ?? "",
//         //     empleadoValido.DIRECCION, empleadoValido.ID_SECTOR ?? 0, empleadoValido.ID_CIUDAD ?? 0, empleadoValido.ID_PROVINCIA ?? 0, empleadoValido.PAIS_NACIMIENTO,
//         //     empleadoValido.TELEFONO_PRINCIPAL, empleadoValido.TELEFONO_SECUNDARIO ?? "", empleadoValido.FECHA_NACIMIENTO, empleadoValido.EMAIL ?? "",
//         //     empleadoValido.SEXO, empleadoValido.ESTADO_CIVIL, empleadoValido.FRECUENCIA_COBRO, empleadoValido.CUENTA_BANCO,
//         //     empleadoValido.ID_ENTIDAD_BANCARIA ?? 0, empleadoValido.ID_PUESTO ?? 0, empleadoValido.ID_DEPARTAMENTO ?? 0, empleadoValido.TIPO_SANGRE,
//         //     empleadoValido.NOMBRE_FAMILIAR_PRIMARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_PRIMARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_PRIMARIO ?? "",
//         //     empleadoValido.NOMBRE_FAMILIAR_SECUNDARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_SECUNDARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_SECUNDARIO ?? "",
//         //     empleadoValido.ID_NIVEL_APROBACION, empleadoValido.ESTATUS, empleadoValido.CREADO_POR ?? 0
//         // );
//         // } catch (Exception ex) {
//         //     TempData["openModal"] = true;
//         //     TempData["Error"] = "Ha ocurrido un error al guardar el empleado:";
//         //     Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
//         //     return RedirectToAction("RegistroEmpleados");
//         //     // throw new Exception("Error al guardar el registro: " + ex.Message + ex.Source);
//         // }

//         // // ViewBag.Sucess = "Registro guardado con exito";
//         // TempData["openModal"] = true;
//         // TempData["Success"] = "El empleado ha sido registrado correctamente.";
//         // Console.WriteLine("Empleado guardado con exito"); // Mensaje para el log en el server
//         // return RedirectToAction("RegistroEmpleados");
        
        
//         }


//     // Actualizar empleado
//     [HttpPost]
//     public async Task<IActionResult> ActualizarAsync(Empleado empleado) {
        
//         // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
//         if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
//             return RedirectToAction("Login", "Auth");
//         }

//         // // Valida el empleado y devuelve un objeto con los campos validados
//         // var empleadoValido = validarEmpleado(empleado);

//         // try {

//         //     await _context.Database.ExecuteSqlRawAsync("EXEC SP_ACTUALIZAR_EMPLEADO @CODIGO_EMPLEADO = {0}, @CEDULA = {1}, @P_NOMBRE = {2}, @S_NOMBRE = {3}, @P_APELLIDO = {4}, @S_APELLIDO = {5}, @DIRECCION = {6}, @ID_SECTOR = {7}, @ID_CIUDAD = {8}, @ID_PROVINCIA = {9}, @PAIS_NACIMIENTO = {10}, @TELEFONO_PRINCIPAL = {11}, @TELEFONO_SECUNDARIO = {12}, @FECHA_NACIMIENTO = {13}, @EMAIL = {14}, @SEXO = {15}, @ESTADO_CIVIL = {16}, @FRECUENCIA_COBRO = {17}, @CUENTA_BANCO = {18}, @ID_ENTIDAD_BANCARIA = {19}, @ID_PUESTO = {20}, @ID_DEPARTAMENTO = {21}, @TIPO_SANGRE = {22}, @NOMBRE_FAMILIAR_PRIMARIO = {23}, @TELEFONO_FAMILIAR_PRIMARIO = {24}, @PARENTESCO_FAMILIAR_PRIMARIO = {25}, @NOMBRE_FAMILIAR_SECUNDARIO = {26}, @TELEFONO_FAMILIAR_SECUNDARIO = {27}, @PARENTESCO_FAMILIAR_SECUNDARIO = {28}, @ID_NIVEL_APROBACION = {29}, @ESTATUS = {30}",
//         //     empleadoValido.CODIGO_EMPLEADO,empleadoValido.CEDULA, empleadoValido.P_NOMBRE, empleadoValido.S_NOMBRE ?? "", empleadoValido.P_APELLIDO, empleadoValido.S_APELLIDO ?? "",
//         //     empleadoValido.DIRECCION, empleadoValido.ID_SECTOR ?? 0, empleadoValido.ID_CIUDAD ?? 0, empleadoValido.ID_PROVINCIA ?? 0, empleadoValido.PAIS_NACIMIENTO,
//         //     empleadoValido.TELEFONO_PRINCIPAL, empleadoValido.TELEFONO_SECUNDARIO ?? "", empleadoValido.FECHA_NACIMIENTO, empleadoValido.EMAIL ?? "",
//         //     empleadoValido.SEXO, empleadoValido.ESTADO_CIVIL, empleadoValido.FRECUENCIA_COBRO, empleadoValido.CUENTA_BANCO,
//         //     empleadoValido.ID_ENTIDAD_BANCARIA ?? 0, empleadoValido.ID_PUESTO ?? 0, empleadoValido.ID_DEPARTAMENTO ?? 0, empleadoValido.TIPO_SANGRE,
//         //     empleadoValido.NOMBRE_FAMILIAR_PRIMARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_PRIMARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_PRIMARIO ?? "",
//         //     empleadoValido.NOMBRE_FAMILIAR_SECUNDARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_SECUNDARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_SECUNDARIO ?? "",
//         //     empleadoValido.ID_NIVEL_APROBACION, empleadoValido.ESTATUS);

//         // } catch(Exception ex) {
//         //     TempData["openModal"] = true;
//         //     TempData["Error"] = "Ha ocurrido un error al actualizar el empleado";
//         //     Console.WriteLine("Error al actualizar el empleado" + ex.Message + ex.Source); // Mensaje para el log en el server
//         //     return RedirectToAction("RegistroEmpleados");
            
//         // }
        
//         //     // ViewBag.Sucess = "Registro actualizado con exito";    
//         //     TempData["openModal"] = true;
//         //     TempData["Success"] = "El empleado ha sido actualizado correctamente."; 
//         //     Console.WriteLine("Empleado actualizado con exito"); // Mensaje para el log en el server
//         //     return RedirectToAction("RegistroEmpleados");

//     }

//     // Eliminar empleado

//     [HttpPost]

//     public async Task<IActionResult> EliminarAsync(string codigoEmpleado) {

//         // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
//         if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
//             return RedirectToAction("Login", "Auth");
//         }
        
//     //     if (codigoEmpleado != null ) {
        
//     //     int codigo_empleado = Convert.ToInt32(codigoEmpleado);

//     //     try {

//     //         await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_EMPLEADO @CODIGO_EMPLEADO = {0}", codigo_empleado);

//     //     } catch (Exception e) {
//     //         TempData["openModal"] = true;
//     //         TempData["Error"] = "Error al eliminar el empleado";
//     //         Console.WriteLine("Error al eliminar el empleado: " + e.Message + e.Source);
//     //         return RedirectToAction("RegistroEmpleados");
//     //     }

//     //     TempData["openModal"] = true;
//     //     TempData["Success"] = "El empleado ha sido eliminado correctamente.";
//     //     Console.WriteLine("Empleado eliminado con exito"); // Mensaje para el log en el server
//     //     return RedirectToAction("RegistroEmpleados");

//     // }
    
//     // TempData["openModal"] = true;
//     // TempData["Error"] = "Codigo de empleado no valido";
//     // return RedirectToAction("RegistroEmpleados");


//     }
   
// }