using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;
using Microsoft.IdentityModel.Tokens;

namespace Coop360_I.Controllers;
public class SociosController : Controller {
    private readonly ILogger<SociosController> _logger;
    private readonly ApplicationDbContext _context;

    public SociosController(ApplicationDbContext context, ILogger<SociosController> logger) {
        _logger = logger;
        _context = context;
    }

    private Socio validarSocio(Socio socio) {
        if (socio == null) {
            throw new Exception("El servidor no puede procesar la solicitud, objeto puesto vacio");
        }

        if (string.IsNullOrEmpty(socio.CEDULA) || string.IsNullOrEmpty(socio.P_NOMBRE) || string.IsNullOrEmpty(socio.P_APELLIDO) ||  string.IsNullOrEmpty(socio.DIRECCION) || socio.ID_SECTOR == null || socio.ID_CIUDAD == null || socio.ID_PROVINCIA == null || string.IsNullOrEmpty(socio.PAIS_NACIMIENTO) || string.IsNullOrEmpty(socio.TELEFONO_PRINCIPAL) || string.IsNullOrEmpty(socio.SEXO) || string.IsNullOrEmpty(socio.PROFESION) || string.IsNullOrEmpty(socio.CARGO) || string.IsNullOrEmpty(socio.LUGAR_TRABAJO) || string.IsNullOrEmpty(socio.DIRECCION_TRABAJO) || string.IsNullOrEmpty(socio.TIPO_CONTRATO) || socio.INGRESOS_MENSUALES < 0  || string.IsNullOrEmpty(socio.FRECUENCIA_COBRO) || string.IsNullOrEmpty(socio.ESTATUS)) {
            throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
        }

        if (socio.ID_SECTOR < 0 || socio.ID_CIUDAD < 0 || socio.ID_PROVINCIA < 0) {
            throw new Exception("El servidor no puede procesar la solicitud, ID de sector, ciudad o provincia no valido");
        }

        var socioValido = new Socio {
            CODIGO_SOCIO = Convert.ToInt32(socio.CODIGO_SOCIO),
            CEDULA = socio.CEDULA,
            P_NOMBRE = socio.P_NOMBRE,
            S_NOMBRE = socio.S_NOMBRE ?? "",
            P_APELLIDO = socio.P_APELLIDO,
            S_APELLIDO = socio.S_APELLIDO ?? "",
            DIRECCION = socio.DIRECCION,
            ID_SECTOR = Convert.ToInt32(socio.ID_SECTOR),
            ID_CIUDAD = Convert.ToInt32(socio.ID_CIUDAD),
            ID_PROVINCIA = Convert.ToInt32(socio.ID_PROVINCIA),
            PAIS_NACIMIENTO = socio.PAIS_NACIMIENTO,
            TELEFONO_PRINCIPAL = socio.TELEFONO_PRINCIPAL,
            FECHA_NACIMIENTO = Convert.ToDateTime(socio.FECHA_NACIMIENTO),
            EMAIL = socio.EMAIL ?? "",
            SEXO = socio.SEXO,
            ESTADO_CIVIL = socio.ESTADO_CIVIL,
            PROFESION = socio.PROFESION,
            CARGO = socio.CARGO,
            LUGAR_TRABAJO = socio.LUGAR_TRABAJO,
            DIRECCION_TRABAJO = socio.DIRECCION_TRABAJO,
            TELEFONO_TRABAJO = socio.TELEFONO_TRABAJO ?? "",
            TIPO_CONTRATO = socio.TIPO_CONTRATO,
            INGRESOS_MENSUALES = Convert.ToDecimal(socio.INGRESOS_MENSUALES),
            FECHA_INGRESO_TRABAJO = socio.FECHA_INGRESO_TRABAJO,
            FRECUENCIA_COBRO = socio.FRECUENCIA_COBRO,
            ESTATUS = socio.ESTATUS,
            CREADO_POR = Convert.ToInt32(socio.CREADO_POR)
        };

        return socioValido;

    }

    // Views

    // View con tabla que muestra los datos
    [HttpGet]
    public IActionResult RegistroSocios() {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 

        var Socios = _context.Socios
        .FromSqlRaw("EXEC SP_LEER_SOCIOS")
        .AsEnumerable()
        .ToList();

        return View("registroSocios",Socios);
    
    }
    
    // View con formulario para crear socio

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

        var ViewModel = new SocioViewModel {
            Provincias = provincias,
            Ciudades = ciudades,
            Sectores = sectores
        };
        
        ViewBag.Title = "Registro de Socio";
        return View("FormSocios", ViewModel);
    }

    // View con formulario para editar socio

     [HttpGet]
    public IActionResult Editar(string codigoSocio) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        }

        // Valida la clave primaria necesaria para editar la entidad
        if (codigoSocio != null) {

        var CODIGO_SOCIO = Convert.ToInt32(codigoSocio);    
        var socio = _context.Socios
        .FromSqlRaw("EXEC SP_BUSCAR_SOCIO @CODIGO_SOCIO = {0}", CODIGO_SOCIO)
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

        var ViewModel = new SocioViewModel {
            Socio = socio,
            Provincias = provincias,
            Ciudades = ciudades,
            Sectores = sectores
        };
        
        ViewBag.Title = "Editar Socio";
        return View("formSocios",ViewModel);

        } else {

            ViewBag.Error = "El ID del socio no es valido.";
            return RedirectToAction("RegistroSocios");

        }

    
    }

     // View modal de confirmacion para eliminar socio
    [HttpGet]

    public IActionResult Eliminar(string codigoSocio) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } 
            
        if (codigoSocio != null ){

            int CODIGO_SOCIO = Convert.ToInt32(codigoSocio);
            TempData["openModal"] = true;
            TempData["Tipo"] = "confirmation";
            TempData["Titulo"] = "¡Cuidado!";
            TempData["Confirmation"] = "¿Esta seguro que desea eliminar este socio?";
            TempData["Controlador"] = "Socios";
            TempData["Parametro"] = "codigoSocio";
            TempData["ID"] = CODIGO_SOCIO;
            return RedirectToAction("RegistroSocios");
        }
            return RedirectToAction("RegistroSocios");
    }   

     // Acciones

     // Guardar

    [HttpPost]
    public async Task<IActionResult> GuardarAsync(Socio socio) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } else {
                // Asigna el codigo del empleado que esta creando el registro
                socio.CREADO_POR = HttpContext.Session.GetInt32("ID_USUARIO");
        };

        // Valida el puesto

        var socioValido = validarSocio(socio);

        try {
            await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_SOCIO @CEDULA = {0}, @P_NOMBRE = {1}, @S_NOMBRE = {2}, @P_APELLIDO = {3}, @S_APELLIDO = {4}, @DIRECCION = {5}, @ID_SECTOR = {6}, @ID_CIUDAD = {7}, @ID_PROVINCIA = {8}, @PAIS_NACIMIENTO = {9}, @TELEFONO_PRINCIPAL = {10}, @TELEFONO_SECUNDARIO = {11}, @FECHA_NACIMIENTO = {12}, @EMAIL = {13}, @SEXO = {14}, @ESTADO_CIVIL = {15}, @PROFESION = {16}, @CARGO = {17}, @LUGAR_TRABAJO = {18}, @DIRECCION_TRABAJO = {19}, @TELEFONO_TRABAJO = {20}, @TIPO_CONTRATO = {21}, @INGRESOS_MENSUALES = {22}, @FECHA_INGRESO_TRABAJO = {23}, @FRECUENCIA_COBRO = {24}, @ESTATUS = {25}, @CREADO_POR = {26}",
            socioValido.CEDULA, socioValido.P_NOMBRE, socioValido.S_NOMBRE ?? "", socioValido.P_APELLIDO, socioValido.S_APELLIDO ?? "",
            socioValido.DIRECCION, socioValido.ID_SECTOR ?? 0, socioValido.ID_CIUDAD ?? 0, socioValido.ID_PROVINCIA ?? 0, socioValido.PAIS_NACIMIENTO,
            socioValido.TELEFONO_PRINCIPAL, socioValido.TELEFONO_SECUNDARIO ?? "", socioValido.FECHA_NACIMIENTO, socioValido.EMAIL ?? "",
            socioValido.SEXO, socioValido.ESTADO_CIVIL, socioValido.PROFESION, socioValido.CARGO, socioValido.LUGAR_TRABAJO, socioValido.DIRECCION_TRABAJO, socioValido.TELEFONO_TRABAJO ?? "", socioValido.TIPO_CONTRATO, socioValido.INGRESOS_MENSUALES, socioValido.FECHA_INGRESO_TRABAJO, socioValido.FRECUENCIA_COBRO, socioValido.ESTATUS, socioValido.CREADO_POR ?? 0
            );
        } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al guardar el socio:";
            Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroSocios");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El socio ha sido registrado correctamente.";
        Console.WriteLine("Codeudro guardado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroSocios");
        
        }


    // Actualizar empleado
    [HttpPost]
    public async Task<IActionResult> ActualizarAsync(Socio socio) {
        
        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }

        // Valida el puesto y devuelve un objeto con los campos validados
        var socioValido = validarSocio(socio);

        try {
            await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_ACTUALIZAR_SOCIO @CODIGO_SOCIO = {0}, @CEDULA = {1}, @P_NOMBRE = {2}, @S_NOMBRE = {3}, @P_APELLIDO = {4}, @S_APELLIDO = {5}, @DIRECCION = {6}, @ID_SECTOR = {7}, @ID_CIUDAD = {8}, @ID_PROVINCIA = {9}, @PAIS_NACIMIENTO = {10}, @TELEFONO_PRINCIPAL = {11}, @TELEFONO_SECUNDARIO = {12}, @FECHA_NACIMIENTO = {13}, @EMAIL = {14}, @SEXO = {15}, @ESTADO_CIVIL = {16}, @PROFESION = {17}, @CARGO = {18}, @LUGAR_TRABAJO = {19}, @DIRECCION_TRABAJO = {20}, @TELEFONO_TRABAJO = {21}, @TIPO_CONTRATO = {22}, @INGRESOS_MENSUALES = {23}, @FECHA_INGRESO_TRABAJO = {24}, @FRECUENCIA_COBRO = {25}, @ESTATUS = {26}",
            socioValido.CODIGO_SOCIO, socioValido.CEDULA, socioValido.P_NOMBRE, socioValido.S_NOMBRE ?? "", socioValido.P_APELLIDO, socioValido.S_APELLIDO ?? "",
            socioValido.DIRECCION, socioValido.ID_SECTOR ?? 0, socioValido.ID_CIUDAD ?? 0, socioValido.ID_PROVINCIA ?? 0, socioValido.PAIS_NACIMIENTO,
            socioValido.TELEFONO_PRINCIPAL, socioValido.TELEFONO_SECUNDARIO ?? "", socioValido.FECHA_NACIMIENTO, socioValido.EMAIL ?? "",
            socioValido.SEXO, socioValido.ESTADO_CIVIL, socioValido.PROFESION, socioValido.CARGO, socioValido.LUGAR_TRABAJO, socioValido.DIRECCION_TRABAJO, socioValido.TELEFONO_TRABAJO ?? "", socioValido.TIPO_CONTRATO, socioValido.INGRESOS_MENSUALES, socioValido.FECHA_INGRESO_TRABAJO, socioValido.FRECUENCIA_COBRO, socioValido.ESTATUS
            );
            
            } catch (Exception ex) {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al actualizar el socio:";
            Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
            return RedirectToAction("RegistroSocios");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El socio ha sido actualizado correctamente.";
        Console.WriteLine("Socio actualizado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroSocios");

    }

     // Eliminar empleado

    [HttpPost]

    public async Task<IActionResult> EliminarAsync(string codigoSocio) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
            return RedirectToAction("Login", "Auth");
        }
        
        if (codigoSocio != null ) {
        
        int CODIGO_SOCIO = Convert.ToInt32(codigoSocio);

        try {

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_SOCIO @CODIGO_SOCIO = {0}", CODIGO_SOCIO);

        } catch (Exception e) {
            TempData["openModal"] = true;
            TempData["Error"] = "Error al eliminar el socio";
            Console.WriteLine("Error al eliminar el socio: " + e.Message + e.Source);
            return RedirectToAction("RegistroSocios");
        }

        TempData["openModal"] = true;
        TempData["Success"] = "El socio ha sido eliminado correctamente.";
        Console.WriteLine("Socio eliminado con exito"); // Mensaje para el log en el server
        return RedirectToAction("RegistroSocios");

    }
    
    TempData["openModal"] = true;
    TempData["Error"] = "ID del socio no es valido";
    return RedirectToAction("RegistroSocios");


    }
   
}