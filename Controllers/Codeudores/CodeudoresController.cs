using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;
using Microsoft.IdentityModel.Tokens;

namespace Coop360_I.Controllers;
public class CodeudoresController : Controller {
    private readonly ILogger<CodeudoresController> _logger;
    private readonly ApplicationDbContext _context;

    public CodeudoresController(ApplicationDbContext context, ILogger<CodeudoresController> logger) {
        _logger = logger;
        _context = context;
    }

    private Codeudor validarCodeudor(Codeudor codeudor) {
        if (codeudor == null) {
            throw new Exception("El servidor no puede procesar la solicitud, objeto puesto vacio");
        }

        if (string.IsNullOrEmpty(codeudor.CEDULA) || string.IsNullOrEmpty(codeudor.P_NOMBRE) || string.IsNullOrEmpty(codeudor.P_APELLIDO) ||  string.IsNullOrEmpty(codeudor.DIRECCION) || codeudor.ID_SECTOR == null || codeudor.ID_CIUDAD == null || codeudor.ID_PROVINCIA == null || string.IsNullOrEmpty(codeudor.PAIS_NACIMIENTO) || string.IsNullOrEmpty(codeudor.TELEFONO_PRINCIPAL) || string.IsNullOrEmpty(codeudor.SEXO) || string.IsNullOrEmpty(codeudor.PROFESION) || string.IsNullOrEmpty(codeudor.CARGO) || string.IsNullOrEmpty(codeudor.LUGAR_TRABAJO) || string.IsNullOrEmpty(codeudor.DIRECCION_TRABAJO) || codeudor.INGRESOS_MENSUALES < 0 || string.IsNullOrEmpty(codeudor.ESTATUS)) {
            throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
        }

        if (codeudor.ID_SECTOR < 0 || codeudor.ID_CIUDAD < 0 || codeudor.ID_PROVINCIA < 0) {
            throw new Exception("El servidor no puede procesar la solicitud, ID de sector, ciudad o provincia no valido");
        }

        var codeudorValido = new Codeudor {
            CODIGO_CODEUDOR = Convert.ToInt32(codeudor.CODIGO_CODEUDOR),
            CEDULA = codeudor.CEDULA,
            P_NOMBRE = codeudor.P_NOMBRE,
            S_NOMBRE = codeudor.S_NOMBRE ?? "",
            P_APELLIDO = codeudor.P_APELLIDO,
            S_APELLIDO = codeudor.S_APELLIDO ?? "",
            DIRECCION = codeudor.DIRECCION,
            ID_SECTOR = Convert.ToInt32(codeudor.ID_SECTOR),
            ID_CIUDAD = Convert.ToInt32(codeudor.ID_CIUDAD),
            ID_PROVINCIA = Convert.ToInt32(codeudor.ID_PROVINCIA),
            PAIS_NACIMIENTO = codeudor.PAIS_NACIMIENTO,
            TELEFONO_PRINCIPAL = codeudor.TELEFONO_PRINCIPAL,
            FECHA_NACIMIENTO = Convert.ToDateTime(codeudor.FECHA_NACIMIENTO),
            EMAIL = codeudor.EMAIL ?? "",
            SEXO = codeudor.SEXO,
            PROFESION = codeudor.PROFESION,
            CARGO = codeudor.CARGO,
            LUGAR_TRABAJO = codeudor.LUGAR_TRABAJO,
            DIRECCION_TRABAJO = codeudor.DIRECCION_TRABAJO,
            TELEFONO_TRABAJO = codeudor.TELEFONO_TRABAJO ?? "",
            INGRESOS_MENSUALES = Convert.ToDecimal(codeudor.INGRESOS_MENSUALES),
            ESTATUS = codeudor.ESTATUS,
            CREADO_POR = Convert.ToInt32(codeudor.CREADO_POR)
        };

        return codeudorValido;

    }

    // Views

    // View con tabla que muestra los datos
    [HttpGet]
    public IActionResult RegistroCodeudores() {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        var Codeudores = _context.Codeudores
        .FromSqlRaw("EXEC SP_LEER_CODEUDORES")
        .AsEnumerable()
        .ToList();

        return View("registroCodeudores",Codeudores);
    
    }
    
    // View con formulario para crear empleado

    public IActionResult Crear () {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        var provincias = _context.Provincias
        .FromSqlRaw("EXEC SP_LEER_PROVINCIAS")
        .AsEnumerable()
        .ToList();

        var ciudades = _context.Ciudades
        .FromSqlRaw("EXEC SP_LEER_CIUDADES")
        .AsEnumerable()
        .ToList();

        var sectores = _context.Sectores
        .FromSqlRaw("EXEC SP_LEER_SECTORES")
        .AsEnumerable()
        .ToList();

        var ViewModel = new CodeudorViewModel {
            Provincias = provincias,
            Ciudades = ciudades,
            Sectores = sectores
        };
        
        ViewBag.Title = "Registro de codeudor";
        return View("FormCodeudores", ViewModel);
    }

    // View con formulario para editar empleado

     [HttpGet]
    public IActionResult Editar(string codigoCodeudor) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        }

        // Valida la clave primaria necesaria para editar la entidad
        if (codigoCodeudor != null) {

        var CODIGO_CODEUDOR = Convert.ToInt32(codigoCodeudor);    
        var codeudor = _context.Codeudores
        .FromSqlRaw("EXEC SP_BUSCAR_CODEUDOR @CODIGO_CODEUDOR = {0}", CODIGO_CODEUDOR)
        .AsEnumerable()
        .FirstOrDefault();

                var provincias = _context.Provincias
        .FromSqlRaw("EXEC SP_LEER_PROVINCIAS")
        .AsEnumerable()
        .ToList();

        var ciudades = _context.Ciudades
        .FromSqlRaw("EXEC SP_LEER_CIUDADES")
        .AsEnumerable()
        .ToList();

        var sectores = _context.Sectores
        .FromSqlRaw("EXEC SP_LEER_SECTORES")
        .AsEnumerable()
        .ToList();

        var ViewModel = new CodeudorViewModel {
            Codeudor = codeudor,
            Provincias = provincias,
            Ciudades = ciudades,
            Sectores = sectores
        };
        
        ViewBag.Title = "Editar Codeudor";
        return View("formCodeudores",ViewModel);

        } else {

            ViewBag.Error = "El ID del departamento no es valido.";
            return RedirectToAction("RegistroDepartamentos");

        }

    
    }

     // View modal de confirmacion para eliminar empleado
    [HttpGet]

    public IActionResult Eliminar(string codigoCodeudor) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
            
        if (codigoCodeudor != null ){

            int CODIGO_CODEUDOR = Convert.ToInt32(codigoCodeudor);
            TempData["openModal"] = true;
            TempData["Tipo"] = "confirmation";
            TempData["Titulo"] = "¡Cuidado!";
            TempData["Confirmation"] = "¿Esta seguro que desea eliminar este codeudor?";
            TempData["Controlador"] = "Codeudores";
            TempData["Parametro"] = "codigoCodeudor";
            TempData["ID"] = CODIGO_CODEUDOR;
            return RedirectToAction("RegistroCodeudores");
        }
            return RedirectToAction("RegistroCodeudores");
    }   

     // Acciones

     // Guardar

    [HttpPost]
    public async Task<IActionResult> GuardarAsync(Codeudor codeudor) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } else {
                // Asigna el codigo del empleado que esta creando el registro
                codeudor.CREADO_POR = HttpContext.Session.GetInt32("ID_USUARIO");
        };

        // Valida el puesto

        var codeudorValido = validarCodeudor(codeudor);

        try {
            await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_CODEUDOR @CEDULA = {0}, @P_NOMBRE = {1}, @S_NOMBRE = {2}, @P_APELLIDO = {3}, @S_APELLIDO = {4}, @DIRECCION = {5}, @ID_SECTOR = {6}, @ID_CIUDAD = {7}, @ID_PROVINCIA = {8}, @PAIS_NACIMIENTO = {9}, @TELEFONO_PRINCIPAL = {10}, @FECHA_NACIMIENTO = {11}, @EMAIL = {12}, @SEXO = {13}, @PROFESION = {14}, @CARGO = {15}, @LUGAR_TRABAJO = {16}, @DIRECCION_TRABAJO = {17}, @TELEFONO_TRABAJO = {18}, @INGRESOS_MENSUALES = {19}, @ESTATUS = {20}, @CREADO_POR = {21}",
            codeudorValido.CEDULA, codeudorValido.P_NOMBRE, codeudorValido.S_NOMBRE ?? "", codeudorValido.P_APELLIDO, codeudorValido.S_APELLIDO ?? "",
            codeudorValido.DIRECCION, codeudorValido.ID_SECTOR ?? 0, codeudorValido.ID_CIUDAD ?? 0, codeudorValido.ID_PROVINCIA ?? 0, codeudorValido.PAIS_NACIMIENTO,
            codeudorValido.TELEFONO_PRINCIPAL, codeudorValido.FECHA_NACIMIENTO, codeudorValido.EMAIL ?? "",
            codeudorValido.SEXO, codeudorValido.PROFESION, codeudorValido.CARGO, codeudorValido.LUGAR_TRABAJO,codeudorValido.DIRECCION_TRABAJO,codeudorValido.TELEFONO_TRABAJO ?? "", codeudorValido.INGRESOS_MENSUALES, codeudorValido.ESTATUS, codeudorValido.CREADO_POR ?? 0
        );
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al guardar el codeudor:";
            Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroCodeudores");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El codeudor ha sido registrado correctamente.";
        Console.WriteLine("Codeudro guardado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroCodeudores");
        
        }


    // Actualizar empleado
    [HttpPost]
    public async Task<IActionResult> ActualizarAsync(Codeudor codeudor) {
        
        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }

        // Valida el puesto y devuelve un objeto con los campos validados
        var codeudorValido = validarCodeudor(codeudor);

        try {
            await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_ACTUALIZAR_CODEUDOR @CODIGO_CODEUDOR = {0}, @CEDULA = {1}, @P_NOMBRE = {2}, @S_NOMBRE = {3}, @P_APELLIDO = {4}, @S_APELLIDO = {5}, @DIRECCION = {6}, @ID_SECTOR = {7}, @ID_CIUDAD = {8}, @ID_PROVINCIA = {9}, @PAIS_NACIMIENTO = {10}, @TELEFONO_PRINCIPAL = {11}, @FECHA_NACIMIENTO = {12}, @EMAIL = {13}, @SEXO = {14}, @PROFESION = {15}, @CARGO = {16}, @LUGAR_TRABAJO = {17}, @DIRECCION_TRABAJO = {18}, @TELEFONO_TRABAJO = {19}, @INGRESOS_MENSUALES = {20}, @ESTATUS = {21}",
            codeudorValido.CODIGO_CODEUDOR, codeudorValido.CEDULA, codeudorValido.P_NOMBRE, codeudorValido.S_NOMBRE ?? "", codeudorValido.P_APELLIDO, codeudorValido.S_APELLIDO ?? "",
            codeudorValido.DIRECCION, codeudorValido.ID_SECTOR ?? 0, codeudorValido.ID_CIUDAD ?? 0, codeudorValido.ID_PROVINCIA ?? 0, codeudorValido.PAIS_NACIMIENTO,
            codeudorValido.TELEFONO_PRINCIPAL, codeudorValido.FECHA_NACIMIENTO, codeudorValido.EMAIL ?? "",
            codeudorValido.SEXO, codeudorValido.PROFESION, codeudorValido.CARGO, codeudorValido.LUGAR_TRABAJO, codeudorValido.DIRECCION_TRABAJO, codeudorValido.TELEFONO_TRABAJO ?? "", codeudorValido.INGRESOS_MENSUALES, codeudorValido.ESTATUS
          );} catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al actualizar el codeudor:";
            Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroCodeudores");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El codeudor ha sido actualizado correctamente.";
        Console.WriteLine("Codeudor actualizado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroCodeudores");

    }

     // Eliminar empleado

    [HttpPost]

    public async Task<IActionResult> EliminarAsync(string codigoCodeudor) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }
        
        if (codigoCodeudor != null ) {
        
        int CODIGO_CODEUDOR = Convert.ToInt32(codigoCodeudor);

        try {

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_CODEUDOR @CODIGO_CODEUDOR = {0}", CODIGO_CODEUDOR);

        } catch (Exception e) {
            TempData["openModal"] = true;
            TempData["Error"] = "Error al eliminar el codeudor";
            Console.WriteLine("Error al eliminar el codeudor: " + e.Message + e.Source);
            return RedirectToAction("RegistroCodeudores");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El codeudor ha sido eliminado correctamente.";
        Console.WriteLine("Codeudor eliminado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroCodeudores");

    }
    
    TempData["openModal"] = true;
    TempData["Error"] = "ID del codeudor no es valido";
    return RedirectToAction("RegistroCodeudores");


    }
   
}