using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;

namespace Coop360_I.Controllers;
public class PuestosController : Controller {
    private readonly ILogger<PuestosController> _logger;
    private readonly ApplicationDbContext _context;

    public PuestosController(ApplicationDbContext context, ILogger<PuestosController> logger) {
        _logger = logger;
        _context = context;
    }

    private Puesto validarPuesto(Puesto puesto) {
        if (puesto == null) {
            throw new Exception("El servidor no puede procesar la solicitud, objeto puesto vacio");
        }

        if (puesto.ID_PUESTO <= 0 || puesto.NOMBRE == null || puesto.ID_DEPARTAMENTO <= 0) {
            throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
        }

        var puestoValido = new Puesto {
            ID_PUESTO = Convert.ToInt32(puesto.ID_PUESTO),
            SALARIO = Convert.ToDecimal(puesto.SALARIO),
            NOMBRE = puesto.NOMBRE,
            ID_DEPARTAMENTO = Convert.ToInt32(puesto.ID_DEPARTAMENTO)
        };

        return puestoValido;

    }

    // Views

    // View con tabla que muestra los datos
    [HttpGet]
    public IActionResult RegistroPuestos() {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        var puestos = _context.Puestos
        .FromSqlRaw("EXEC SP_LEER_PUESTOS")
        .AsEnumerable()
        .ToList();

        return View("~/Views/Empleados/Puestos/RegistroPuestos.cshtml", puestos);
    
    }
    
    // View con formulario para crear empleado

    public IActionResult Crear () {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        // Obtiene los datos necesarios para la vista 

        var departamentos = _context.Departamentos
        .FromSqlRaw("EXEC SP_LEER_DEPARTAMENTOS")
        .AsEnumerable()
        .ToList();

        // Validacion de los datos

        if (departamentos == null) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al cargar los datos necesarios para la pantalla";
            return RedirectToAction("RegistroPuestos");
        }
        
        ViewBag.Title = "Registro de puesto";
        return View("~/Views/Empleados/Puestos/FormPuestos.cshtml",departamentos);
    }

    // View con formulario para editar empleado

     [HttpGet]
    public IActionResult Editar(string IdPuesto) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        }

        // Valida la clave primaria necesaria para editar la entidad
        if (IdPuesto != null) {

        var departamentos = _context.Departamentos
        .FromSqlRaw("EXEC SP_LEER_DEPARTAMENTOS")
        .AsEnumerable()
        .ToList();

        if (departamentos == null) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al cargar los datos necesarios para la pantalla";
            return RedirectToAction("RegistroPuestos");
        }

        var ID_Puesto = Convert.ToInt32(IdPuesto);    
        var puesto = _context.Puestos
        .FromSqlRaw("EXEC SP_OBTENER_PUESTO @ID_PUESTO = {0}", ID_Puesto)
        .AsEnumerable()
        .FirstOrDefault();

        var viewModel = new PuestoViewModel {
            Puesto = puesto,
            Departamentos = departamentos
        };
        
        ViewBag.Title = "Editar Puesto";
        return View("~/Views/Empleados/Puestos/FormPuestos.cshtml",viewModel);

        } else {

            ViewBag.Error = "El ID del puesto no es valido.";
            return RedirectToAction("RegistroPuestos");

        }

    
    }

     // View modal de confirmacion para eliminar empleado
    [HttpGet]

    public IActionResult Eliminar(string IdPuesto) {
            
        if (IdPuesto != null ){

            int ID_PUESTO = Convert.ToInt32(IdPuesto);
            TempData["openModal"] = true;
            TempData["Tipo"] = "confirmation";
            TempData["Titulo"] = "¡Cuidado!";
            TempData["Confirmation"] = "¿Esta seguro que desea eliminar este puesto?";
            TempData["Controlador"] = "Puestos";
            TempData["Parametro"] = "IdPuesto";
            TempData["ID"] = ID_PUESTO;
            return RedirectToAction("RegistroPuestos");
        }
            return RedirectToAction("RegistroPuestos");
    }   

     // Acciones

     // Guardar

    // [HttpPost]
    // public async Task<IActionResult> GuardarAsync(Puesto puesto) {

    //     // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    //     if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
    //             return RedirectToAction("Login", "Auth");
    //     } 

    //     // try {
    //     // await _context.Database.ExecuteSqlRawAsync(
    //     //     "EXEC SP_CREAR_EMPLEADO @CEDULA = {0}, @P_NOMBRE = {1}, @S_NOMBRE = {2}, @P_APELLIDO = {3}, @S_APELLIDO = {4}, @DIRECCION = {5}, @ID_SECTOR = {6}, @ID_CIUDAD = {7}, @ID_PROVINCIA = {8}, @PAIS_NACIMIENTO = {9}, @TELEFONO_PRINCIPAL = {10}, @TELEFONO_SECUNDARIO = {11}, @FECHA_NACIMIENTO = {12}, @EMAIL = {13}, @SEXO = {14}, @ESTADO_CIVIL = {15}, @FRECUENCIA_COBRO = {16}, @CUENTA_BANCO = {17}, @ID_ENTIDAD_BANCARIA = {18}, @ID_PUESTO = {19}, @ID_DEPARTAMENTO = {20}, @TIPO_SANGRE = {21}, @NOMBRE_FAMILIAR_PRIMARIO = {22}, @TELEFONO_FAMILIAR_PRIMARIO = {23}, @PARENTESCO_FAMILIAR_PRIMARIO = {24}, @NOMBRE_FAMILIAR_SECUNDARIO = {25}, @TELEFONO_FAMILIAR_SECUNDARIO = {26}, @PARENTESCO_FAMILIAR_SECUNDARIO = {27}, @ID_NIVEL_APROBACION = {28}, @ESTATUS = {29}, @CREADO_POR = {30}",
    //     //     empleadoValido.CEDULA, empleadoValido.P_NOMBRE, empleadoValido.S_NOMBRE ?? "", empleadoValido.P_APELLIDO, empleadoValido.S_APELLIDO ?? "",
    //     //     empleadoValido.DIRECCION, empleadoValido.ID_SECTOR ?? 0, empleadoValido.ID_CIUDAD ?? 0, empleadoValido.ID_PROVINCIA ?? 0, empleadoValido.PAIS_NACIMIENTO,
    //     //     empleadoValido.TELEFONO_PRINCIPAL, empleadoValido.TELEFONO_SECUNDARIO ?? "", empleadoValido.FECHA_NACIMIENTO, empleadoValido.EMAIL ?? "",
    //     //     empleadoValido.SEXO, empleadoValido.ESTADO_CIVIL, empleadoValido.FRECUENCIA_COBRO, empleadoValido.CUENTA_BANCO,
    //     //     empleadoValido.ID_ENTIDAD_BANCARIA ?? 0, empleadoValido.ID_PUESTO ?? 0, empleadoValido.ID_DEPARTAMENTO ?? 0, empleadoValido.TIPO_SANGRE,
    //     //     empleadoValido.NOMBRE_FAMILIAR_PRIMARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_PRIMARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_PRIMARIO ?? "",
    //     //     empleadoValido.NOMBRE_FAMILIAR_SECUNDARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_SECUNDARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_SECUNDARIO ?? "",
    //     //     empleadoValido.ID_NIVEL_APROBACION, empleadoValido.ESTATUS, empleadoValido.CREADO_POR ?? 0
    //     // );
    //     // } catch (Exception ex) {
    //     //     TempData["openModal"] = true;
    //     //     TempData["Error"] = "Ha ocurrido un error al guardar el empleado:";
    //     //     Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
    //     //     return RedirectToAction("RegistroEmpleados");
    //     //     // throw new Exception("Error al guardar el registro: " + ex.Message + ex.Source);
    //     // }

    //     // // ViewBag.Sucess = "Registro guardado con exito";
    //     // TempData["openModal"] = true;
    //     // TempData["Success"] = "El empleado ha sido registrado correctamente.";
    //     // Console.WriteLine("Empleado guardado con exito"); // Mensaje para el log en el server
    //     // return RedirectToAction("RegistroEmpleados");
        
        
    //     }


    // // Actualizar empleado
    // [HttpPost]
    // public async Task<IActionResult> ActualizarAsync(Empleado empleado) {
        
    //     // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    //     if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
    //         return RedirectToAction("Login", "Auth");
    //     }

    //     // // Valida el empleado y devuelve un objeto con los campos validados
    //     // var empleadoValido = validarEmpleado(empleado);

    //     // try {

    //     //     await _context.Database.ExecuteSqlRawAsync("EXEC SP_ACTUALIZAR_EMPLEADO @CODIGO_EMPLEADO = {0}, @CEDULA = {1}, @P_NOMBRE = {2}, @S_NOMBRE = {3}, @P_APELLIDO = {4}, @S_APELLIDO = {5}, @DIRECCION = {6}, @ID_SECTOR = {7}, @ID_CIUDAD = {8}, @ID_PROVINCIA = {9}, @PAIS_NACIMIENTO = {10}, @TELEFONO_PRINCIPAL = {11}, @TELEFONO_SECUNDARIO = {12}, @FECHA_NACIMIENTO = {13}, @EMAIL = {14}, @SEXO = {15}, @ESTADO_CIVIL = {16}, @FRECUENCIA_COBRO = {17}, @CUENTA_BANCO = {18}, @ID_ENTIDAD_BANCARIA = {19}, @ID_PUESTO = {20}, @ID_DEPARTAMENTO = {21}, @TIPO_SANGRE = {22}, @NOMBRE_FAMILIAR_PRIMARIO = {23}, @TELEFONO_FAMILIAR_PRIMARIO = {24}, @PARENTESCO_FAMILIAR_PRIMARIO = {25}, @NOMBRE_FAMILIAR_SECUNDARIO = {26}, @TELEFONO_FAMILIAR_SECUNDARIO = {27}, @PARENTESCO_FAMILIAR_SECUNDARIO = {28}, @ID_NIVEL_APROBACION = {29}, @ESTATUS = {30}",
    //     //     empleadoValido.CODIGO_EMPLEADO,empleadoValido.CEDULA, empleadoValido.P_NOMBRE, empleadoValido.S_NOMBRE ?? "", empleadoValido.P_APELLIDO, empleadoValido.S_APELLIDO ?? "",
    //     //     empleadoValido.DIRECCION, empleadoValido.ID_SECTOR ?? 0, empleadoValido.ID_CIUDAD ?? 0, empleadoValido.ID_PROVINCIA ?? 0, empleadoValido.PAIS_NACIMIENTO,
    //     //     empleadoValido.TELEFONO_PRINCIPAL, empleadoValido.TELEFONO_SECUNDARIO ?? "", empleadoValido.FECHA_NACIMIENTO, empleadoValido.EMAIL ?? "",
    //     //     empleadoValido.SEXO, empleadoValido.ESTADO_CIVIL, empleadoValido.FRECUENCIA_COBRO, empleadoValido.CUENTA_BANCO,
    //     //     empleadoValido.ID_ENTIDAD_BANCARIA ?? 0, empleadoValido.ID_PUESTO ?? 0, empleadoValido.ID_DEPARTAMENTO ?? 0, empleadoValido.TIPO_SANGRE,
    //     //     empleadoValido.NOMBRE_FAMILIAR_PRIMARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_PRIMARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_PRIMARIO ?? "",
    //     //     empleadoValido.NOMBRE_FAMILIAR_SECUNDARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_SECUNDARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_SECUNDARIO ?? "",
    //     //     empleadoValido.ID_NIVEL_APROBACION, empleadoValido.ESTATUS);

    //     // } catch(Exception ex) {
    //     //     TempData["openModal"] = true;
    //     //     TempData["Error"] = "Ha ocurrido un error al actualizar el empleado";
    //     //     Console.WriteLine("Error al actualizar el empleado" + ex.Message + ex.Source); // Mensaje para el log en el server
    //     //     return RedirectToAction("RegistroEmpleados");
            
    //     // }
        
    //     //     // ViewBag.Sucess = "Registro actualizado con exito";    
    //     //     TempData["openModal"] = true;
    //     //     TempData["Success"] = "El empleado ha sido actualizado correctamente."; 
    //     //     Console.WriteLine("Empleado actualizado con exito"); // Mensaje para el log en el server
    //     //     return RedirectToAction("RegistroEmpleados");

    // }

     // Eliminar empleado

    [HttpPost]

    public async Task<IActionResult> EliminarAsync(string IdPuesto) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }
        
        if (IdPuesto != null ) {
        
        int ID_PUESTO = Convert.ToInt32(IdPuesto);

        try {

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_PUESTO @ID_PUESTO = {0}", ID_PUESTO);

        } catch (Exception e) {
            TempData["openModal"] = true;
            TempData["Error"] = "Error al eliminar el puesto";
            Console.WriteLine("Error al eliminar el puesto: " + e.Message + e.Source);
            return RedirectToAction("RegistroPuestos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El puesto ha sido eliminado correctamente.";
        Console.WriteLine("Puesto eliminado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroPuestos");

    }
    
    TempData["openModal"] = true;
    TempData["Error"] = "ID del Puesto no es valido";
    return RedirectToAction("RegistroPuestos");


    }
   
}