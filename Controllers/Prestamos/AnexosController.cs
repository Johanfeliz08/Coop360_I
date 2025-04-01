using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;

namespace Coop360_I.Controllers;
public class AnexosController : Controller {
    private readonly ILogger<AnexosController> _logger;
    private readonly ApplicationDbContext _context;

    public AnexosController(ApplicationDbContext context, ILogger<AnexosController> logger) {
        _logger = logger;
        _context = context;
    }

    private Anexo validarAnexo(Anexo anexo) {
        if (anexo == null) {
            throw new Exception("El servidor no puede procesar la solicitud, objeto Anexo vacio");
        }

        if (anexo.ID_ANEXO < 0 || string.IsNullOrEmpty(anexo.NOMBRE)  || string.IsNullOrEmpty(anexo.ID_NIVEL_APROBACION)) {
            throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
        }

        var anexoValido = new Anexo {
            ID_ANEXO = Convert.ToInt32(anexo.ID_ANEXO),
            NOMBRE = anexo.NOMBRE,
            OBLIGATORIO = anexo.OBLIGATORIO == "S" ? "S" : "N",
            ID_NIVEL_APROBACION = anexo.ID_NIVEL_APROBACION
        };

        return anexoValido;

    }

    // Views

    // View con tabla que muestra los datos
    [HttpGet]
    public IActionResult RegistroAnexos() {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        var anexos = _context.Anexos
        .FromSqlRaw("EXEC SP_LEER_ANEXOS")
        .AsEnumerable()
        .ToList();

        return View("~/Views/Prestamos/Anexos/RegistroAnexos.cshtml", anexos);
    
    }
    
    // View con formulario para crear empleado

    public IActionResult Crear () {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        // Obtiene los datos necesarios para la vista 

        var nivelesAprobacion = _context.NivelesAprobacion
        .FromSqlRaw("EXEC SP_LEER_NIVELES_APROBACION")
        .AsEnumerable()
        .ToList();

        // Validacion de los datos

        if (nivelesAprobacion == null) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al cargar los datos necesarios para la pantalla";
            return RedirectToAction("RegistroAnexos");
        }

        var ViewModel = new AnexoViewModel {
          NivelesAprobacion = nivelesAprobacion
        };
        
        ViewBag.Title = "Registro de Anexo";
        return View("~/Views/Prestamos/Anexos/FormAnexos.cshtml",ViewModel);
    }

    // View con formulario para editar empleado

     [HttpGet]
    public IActionResult Editar(string IdAnexo) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        }

        // Valida la clave primaria necesaria para editar la entidad
        if (IdAnexo != null) {

        var nivelesAprobacion = _context.NivelesAprobacion
        .FromSqlRaw("EXEC SP_LEER_NIVELES_APROBACION")
        .AsEnumerable()
        .ToList();

        if (nivelesAprobacion == null) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al cargar los datos necesarios para la pantalla";
            return RedirectToAction("RegistroAnexos");
        }

        var ID_ANEXO = Convert.ToInt32(IdAnexo);    
        var anexo = _context.Anexos
        .FromSqlRaw("EXEC SP_BUSCAR_ANEXO @ID_ANEXO = {0}", ID_ANEXO)
        .AsEnumerable()
        .FirstOrDefault();

        var viewModel = new AnexoViewModel {
          Anexo = anexo,
          NivelesAprobacion = nivelesAprobacion
        };
        
        Console.WriteLine($"Anexo campo obligatorio: {viewModel.Anexo?.OBLIGATORIO}"); // Mensaje para el log en el server

        ViewBag.Title = "Editar Anexo";
        return View("~/Views/Prestamos/Anexos/FormAnexos.cshtml",viewModel);

        } else {

            ViewBag.Error = "El ID del anexo no es valido.";
            return RedirectToAction("RegistroAnexos");

        }

    
    }

     // View modal de confirmacion para eliminar anexo
    [HttpGet]

    public IActionResult Eliminar(string IdAnexo) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
            
        if (IdAnexo != null ){

            int ID_ANEXO = Convert.ToInt32(IdAnexo);
            TempData["openModal"] = true;
            TempData["Tipo"] = "confirmation";
            TempData["Titulo"] = "¡Cuidado!";
            TempData["Confirmation"] = "¿Esta seguro que desea eliminar este anexo?";
            TempData["Controlador"] = "Anexos";
            TempData["Parametro"] = "IdAnexo";
            TempData["ID"] = ID_ANEXO;
            return RedirectToAction("RegistroAnexos");
        }
            return RedirectToAction("RegistroAnexos");
    }   

     // Acciones

     // Guardar

    [HttpPost]
    public async Task<IActionResult> GuardarAsync(Anexo anexo) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        // Valida el Anexo

        var anexoValido = validarAnexo(anexo);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_ANEXO @NOMBRE = {0}, @OBLIGATORIO = {1}, @ID_NIVEL_APROBACION = {2}",
            anexoValido.NOMBRE, anexoValido.OBLIGATORIO, anexoValido.ID_NIVEL_APROBACION
        );
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al guardar el anexo:";
            Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroAnexos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El anexo ha sido registrado correctamente.";
        Console.WriteLine("Anexo guardado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroAnexos");
        
        }


    // Actualizar empleado
    [HttpPost]
    public async Task<IActionResult> ActualizarAsync(Anexo anexo) {
        
        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }

        // Valida el Anexo y devuelve un objeto con los campos validados
        var anexoValido = validarAnexo(anexo);

        try {
         await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_ACTUALIZAR_ANEXO @ID_ANEXO = {0},@NOMBRE = {1}, @OBLIGATORIO = {2}, @ID_NIVEL_APROBACION = {3}",
            anexoValido.ID_ANEXO, anexoValido.NOMBRE, anexoValido.OBLIGATORIO, anexoValido.ID_NIVEL_APROBACION
        );
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al actualizar el Anexo:";
            Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroAnexos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El Anexo ha sido actualizado correctamente.";
        Console.WriteLine("Anexo actualizado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroAnexos");

    }

     // Eliminar empleado

    [HttpPost]

    public async Task<IActionResult> EliminarAsync(string IdAnexo) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }
        
        if (IdAnexo != null ) {
        
        int ID_ANEXO = Convert.ToInt32(IdAnexo);

        try {

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_ANEXO @ID_ANEXO = {0}", ID_ANEXO);

        } catch (Exception e) {
            TempData["openModal"] = true;
            TempData["Error"] = "Error al eliminar el anexo";
            Console.WriteLine("Error al eliminar el anexo: " + e.Message + e.Source);
            return RedirectToAction("RegistroAnexos");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El anexo ha sido eliminado correctamente.";
        Console.WriteLine("Anexo eliminado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroAnexos");

    }
    
    TempData["openModal"] = true;
    TempData["Error"] = "ID del anexo no es valido";
    return RedirectToAction("RegistroAnexos");


    }
   
}