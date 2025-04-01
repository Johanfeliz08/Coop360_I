using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;

namespace Coop360_I.Controllers;
public class DepartamentosController : Controller {
    private readonly ILogger<DepartamentosController> _logger;
    private readonly ApplicationDbContext _context;

    public DepartamentosController(ApplicationDbContext context, ILogger<DepartamentosController> logger) {
        _logger = logger;
        _context = context;
    }

    private Departamento validarDepartamento(Departamento departamento) {
        if (departamento == null) {
            throw new Exception("El servidor no puede procesar la solicitud, objeto puesto vacio");
        }

        if (departamento.NOMBRE == null || departamento.ENCARGADO == null ) {
            throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
        }

        var departamentoValido = new Departamento {
            ID_DEPARTAMENTO = Convert.ToInt32(departamento.ID_DEPARTAMENTO),
            NOMBRE = departamento.NOMBRE,
            ENCARGADO = departamento.ENCARGADO,
        };

        return departamentoValido;

    }

    // Views

    // View con tabla que muestra los datos
    [HttpGet]
    public IActionResult RegistroDepartamentos() {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        var departamentos = _context.Departamentos
        .FromSqlRaw("EXEC SP_LEER_DEPARTAMENTOS")
        .AsEnumerable()
        .ToList();

        return View("~/Views/Empleados/Departamentos/RegistroDepartamentos.cshtml", departamentos);
    
    }
    
    // View con formulario para crear empleado

    public IActionResult Crear () {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
        
        ViewBag.Title = "Registro de departamento";
        return View("~/Views/Empleados/Departamentos/FormDepartamentos.cshtml");
    }

    // View con formulario para editar empleado

     [HttpGet]
    public IActionResult Editar(string IdDepartamento) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        }

        // Valida la clave primaria necesaria para editar la entidad
        if (IdDepartamento != null) {

        var ID_Departamento = Convert.ToInt32(IdDepartamento);    
        var departamento = _context.Departamentos
        .FromSqlRaw("EXEC SP_OBTENER_DEPARTAMENTO @ID_DEPARTAMENTO = {0}", ID_Departamento)
        .AsEnumerable()
        .FirstOrDefault();
        
        ViewBag.Title = "Editar Departamento";
        return View("~/Views/Empleados/Departamentos/FormDepartamentos.cshtml",departamento);

        } else {

            ViewBag.Error = "El ID del departamento no es valido.";
            return RedirectToAction("RegistroDepartamentos");

        }

    
    }

     // View modal de confirmacion para eliminar empleado
    [HttpGet]

    public IActionResult Eliminar(string IdDepartamento) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
            
        if (IdDepartamento != null ){

            int ID_DEPARTAMENTO = Convert.ToInt32(IdDepartamento);
            TempData["openModal"] = true;
            TempData["Tipo"] = "confirmation";
            TempData["Titulo"] = "¡Cuidado!";
            TempData["Confirmation"] = "¿Esta seguro que desea eliminar este departamento?";
            TempData["Controlador"] = "Departamentos";
            TempData["Parametro"] = "IdDepartamento";
            TempData["ID"] = ID_DEPARTAMENTO;
            return RedirectToAction("RegistroDepartamentos");
        }
            return RedirectToAction("RegistroDepartamentos");
    }   

     // Acciones

     // Guardar

    [HttpPost]
    public async Task<IActionResult> GuardarAsync(Departamento departamento) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        // Valida el puesto

        var departamentoValido = validarDepartamento(departamento);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_DEPARTAMENTO @NOMBRE = {0}, @ENCARGADO = {1}",
            departamentoValido.NOMBRE, departamentoValido.ENCARGADO);
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al guardar el departamento:";
            Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroDepartamentos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El departamento ha sido registrado correctamente.";
        Console.WriteLine("Departamento guardado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroDepartamentos");
        
        }


    // Actualizar empleado
    [HttpPost]
    public async Task<IActionResult> ActualizarAsync(Departamento departamento) {
        
        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }

        // Valida el puesto y devuelve un objeto con los campos validados
        var departamentoValido = validarDepartamento(departamento);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_ACTUALIZAR_DEPARTAMENTO @ID_DEPARTAMENTO = {0}, @NOMBRE = {1}, @ENCARGADO = {2}",
            departamentoValido.ID_DEPARTAMENTO, departamentoValido.NOMBRE, departamentoValido.ENCARGADO);
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al actualizar el departamento:";
            Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroDepartamentos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El departamento ha sido actualizado correctamente.";
        Console.WriteLine("Departamento actualizado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroDepartamentos");

    }

     // Eliminar empleado

    [HttpPost]

    public async Task<IActionResult> EliminarAsync(string IdDepartamento) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }
        
        if (IdDepartamento != null ) {
        
        int ID_DEPARTAMENTO = Convert.ToInt32(IdDepartamento);

        try {

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_DEPARTAMENTO @ID_DEPARTAMENTO = {0}", ID_DEPARTAMENTO);

        } catch (Exception e) {
            TempData["openModal"] = true;
            TempData["Error"] = "Error al eliminar el departamento";
            Console.WriteLine("Error al eliminar el departamento: " + e.Message + e.Source);
            return RedirectToAction("RegistroDepartamentos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El departamento ha sido eliminado correctamente.";
        Console.WriteLine("Departamento eliminado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroDepartamentos");

    }
    
    TempData["openModal"] = true;
    TempData["Error"] = "ID del departamento no es valido";
    return RedirectToAction("RegistroDepartamentos");


    }
   
}