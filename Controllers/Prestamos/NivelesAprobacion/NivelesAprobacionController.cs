using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;

namespace Coop360_I.Controllers;
public class NivelesAprobacionController : Controller {
    private readonly ILogger<NivelesAprobacionController> _logger;
    private readonly ApplicationDbContext _context;

    public NivelesAprobacionController(ApplicationDbContext context, ILogger<NivelesAprobacionController> logger) {
        _logger = logger;
        _context = context;
    }

    private NivelAprobacion validarNivelAprobacion(NivelAprobacion nivelAprobacion) {
        if (nivelAprobacion == null) {
            throw new Exception("El servidor no puede procesar la solicitud, objeto NivelAprobacion vacio");
        }

        if ( nivelAprobacion.ID_NIVEL_APROBACION < 0 || string.IsNullOrEmpty(nivelAprobacion.NOMBRE) || nivelAprobacion.MONTO_DESDE < 0 || nivelAprobacion.MONTO_HASTA < 0 ) {
            throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
        }

        var nivelAprobacionValido = new NivelAprobacion {
          ID_NIVEL_APROBACION = Convert.ToInt32(nivelAprobacion.ID_NIVEL_APROBACION),
          MONTO_DESDE = Convert.ToDecimal(nivelAprobacion.MONTO_DESDE),
          MONTO_HASTA = Convert.ToDecimal(nivelAprobacion.MONTO_HASTA),
          NOMBRE = nivelAprobacion.NOMBRE,
          FECHA_CREACION = nivelAprobacion.FECHA_CREACION
        };

        return nivelAprobacionValido;

    }

    // Views

    // View con tabla que muestra los datos
    [HttpGet]
    public IActionResult RegistroNivelesAprobacion() {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        var nivelesAprobacion = _context.NivelesAprobacion
        .FromSqlRaw("EXEC SP_LEER_NIVELES_APROBACION")
        .AsEnumerable()
        .ToList();

        return View("~/Views/Prestamos/NivelesAprobacion/RegistroNivelesAprobacion.cshtml", nivelesAprobacion);
    
    }
    
    // View con formulario para crear nivel aprobacion

    public IActionResult Crear () {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
        
        ViewBag.Title = "Registro nivel aprobacion";
        return View("~/Views/Prestamos/NivelesAprobacion/FormNivelesAprobacion.cshtml");
    }

    // View con formulario para editar nivel aprobacion

     [HttpGet]
    public IActionResult Editar(string IdNivelAprobacion) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        }

        // Valida la clave primaria necesaria para editar la entidad
        if (IdNivelAprobacion != null) {


        int ID_NIVEL_APROBACION = Convert.ToInt32(IdNivelAprobacion);    
        var nivelAprobacion = _context.NivelesAprobacion
        .FromSqlRaw("EXEC SP_BUSCAR_NIVEL_APROBACION @ID_NIVEL_APROBACION = {0}", ID_NIVEL_APROBACION)
        .AsEnumerable()
        .FirstOrDefault();
        
        if (nivelAprobacion == null) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al buscar el nivel aprobacion";
            return RedirectToAction("RegistroNivelesAprobacion");
        }

        ViewBag.Title = "Editar nivel aprobacion";
        return View("~/Views/Prestamos/NivelesAprobacion/FormNivelesAprobacion.cshtml",nivelAprobacion);

        } else {

            ViewBag.Error = "El ID del NivelAprobacion no es valido.";
            return RedirectToAction("RegistroNivelesAprobacion");

        }

    
    }

     // View modal de confirmacion para eliminar NivelAprobacion
    [HttpGet]

    public IActionResult Eliminar(string IdNivelAprobacion) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
            
        if (IdNivelAprobacion != null ){

            int ID_NIVEL_APROBACION = Convert.ToInt32(IdNivelAprobacion);
            TempData["openModal"] = true;
            TempData["Tipo"] = "confirmation";
            TempData["Titulo"] = "¡Cuidado!";
            TempData["Confirmation"] = "¿Esta seguro que desea eliminar este nivel de aprobacion?";
            TempData["Controlador"] = "NivelesAprobacion";
            TempData["Parametro"] = "IdNivelAprobacion";
            TempData["ID"] = ID_NIVEL_APROBACION;
            return RedirectToAction("RegistroNivelesAprobacion");
        }
            return RedirectToAction("RegistroNivelesAprobacion");
    }   

     // Acciones

     // Guardar

    [HttpPost]
    public async Task<IActionResult> GuardarAsync(NivelAprobacion nivelAprobacion) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        // Valida el NivelAprobacion

        var nivelAprobacionValido = validarNivelAprobacion(nivelAprobacion);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_NIVEL_APROBACION @NOMBRE = {0}, @MONTO_DESDE = {1}, @MONTO_HASTA = {2}",
            nivelAprobacionValido.NOMBRE ?? "", nivelAprobacionValido.MONTO_DESDE, nivelAprobacionValido.MONTO_HASTA
        );

        } catch (Exception ex) {
          
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al guardar el nivel de aprobacion:";
            Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroNivelesAprobacion");
        
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El nivel de aprobacion ha sido registrado correctamente.";
        Console.WriteLine("Nivel de aprobacion guardado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroNivelesAprobacion");
        
        }


    // Actualizar nivelAprobacion
    [HttpPost]
    public async Task<IActionResult> ActualizarAsync(NivelAprobacion nivelAprobacion) {
        
        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }

        // Valida el NivelAprobacion y devuelve un objeto con los campos validados
        var nivelAprobacionValido = validarNivelAprobacion(nivelAprobacion);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_ACTUALIZAR_NIVEL_APROBACION @ID_NIVEL_APROBACION = {0}, @NOMBRE = {1}, @MONTO_DESDE = {2}, @MONTO_HASTA = {3}",
            nivelAprobacionValido.ID_NIVEL_APROBACION ?? 0 , nivelAprobacionValido.NOMBRE ?? "", nivelAprobacionValido.MONTO_DESDE, nivelAprobacionValido.MONTO_HASTA
        );
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al actualizar el nivel de aprobacion:";
            Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroNivelesAprobacion");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El nivel de aprobacion ha sido actualizado correctamente.";
        Console.WriteLine("NivelAprobacion actualizado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroNivelesAprobacion");

    }

     // Eliminar empleado

    [HttpPost]

    public async Task<IActionResult> EliminarAsync(string IdNivelAprobacion) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }
        
        if (IdNivelAprobacion != null ) {
        
        int ID_NIVEL_APROBACION = Convert.ToInt32(IdNivelAprobacion);

        try {

         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_ELIMINAR_NIVEL_APROBACION @ID_NIVEL_APROBACION = {0}",
            ID_NIVEL_APROBACION
            
        );
        } catch (Exception e) {
            TempData["openModal"] = true;
            TempData["Error"] = "Error al eliminar el nivel de aprobacion";
            Console.WriteLine("Error al eliminar el NivelAprobacion: " + e.Message + e.Source);
            return RedirectToAction("RegistroNivelesAprobacion");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El nivel de aprobacion ha sido eliminado correctamente.";
        Console.WriteLine("NivelAprobacion eliminado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroNivelesAprobacion");

    }
    
    TempData["openModal"] = true;
    TempData["Error"] = "ID del nivel de aprobacion no es valido";
    return RedirectToAction("RegistroNivelesAprobacion");


    }
   
}