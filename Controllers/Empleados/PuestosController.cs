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

        if (puesto.NOMBRE == null || puesto.SALARIO <= 0 || puesto.ID_DEPARTAMENTO <= 0) {
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

        var ViewModel = new PuestoViewModel {
            Departamentos = departamentos
        };
        
        ViewBag.Title = "Registro de puesto";
        return View("~/Views/Empleados/Puestos/FormPuestos.cshtml",ViewModel);
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

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
            
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

    [HttpPost]
    public async Task<IActionResult> GuardarAsync(Puesto puesto) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        // Valida el puesto

        var puestoValido = validarPuesto(puesto);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_PUESTO @NOMBRE = {0}, @SALARIO = {1}, @ID_DEPARTAMENTO = {2}",
            puestoValido.NOMBRE, puestoValido.SALARIO, puestoValido.ID_DEPARTAMENTO ?? 0
        );
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al guardar el puesto:";
            Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroPuestos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El puesto ha sido registrado correctamente.";
        Console.WriteLine("Puesto guardado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroPuestos");
        
        }


    // Actualizar empleado
    [HttpPost]
    public async Task<IActionResult> ActualizarAsync(Puesto puesto) {
        
        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }

        // Valida el puesto y devuelve un objeto con los campos validados
        var puestoValido = validarPuesto(puesto);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_ACTUALIZAR_PUESTO @ID_PUESTO = {0}, @NOMBRE = {1}, @SALARIO = {2}, @ID_DEPARTAMENTO = {3}",
            puestoValido.ID_PUESTO, puestoValido.NOMBRE, puestoValido.SALARIO, puestoValido.ID_DEPARTAMENTO ?? 0
        );
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al actualizar el puesto:";
            Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroPuestos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El puesto ha sido actualizado correctamente.";
        Console.WriteLine("Puesto actualizado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroPuestos");

    }

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