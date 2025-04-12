using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.Serialization.Json;

namespace Coop360_I.Controllers;

public class PrestamosController : Controller
{
    private readonly ILogger<PrestamosController> _logger;
    private readonly ApplicationDbContext _context;

    public PrestamosController(ApplicationDbContext context, ILogger<PrestamosController> logger)
    {
        _logger = logger;
        _context = context;
    }

    private SolicitudPrestamo validarSolicitudPrestamo(SolicitudPrestamo solicitudPrestamo)
    {

        if (solicitudPrestamo.ID_SOLICITUD <= 0)
        {
            throw new Exception("El ID de la solicitud es requerido");
        }

        if (solicitudPrestamo.CODIGO_SOCIO <= 0)
        {
            throw new Exception("El código del socio es requerido");
        }

        if (solicitudPrestamo.SOLICITUD_REALIZADA_POR <= 0)
        {
            throw new Exception("El código del empleado que realizó la solicitud es requerido");
        }

        if (solicitudPrestamo.MONTO_SOLICITADO <= 0)
        {
            throw new Exception("El monto solicitado es requerido");
        }

        if (solicitudPrestamo.PLAZO_MESES <= 0)
        {
            throw new Exception("El plazo en meses es requerido");
        }

        if (solicitudPrestamo.CANTIDAD_CUOTAS <= 0)
        {
            throw new Exception("La cantidad de cuotas es requerida");
        }

        if (solicitudPrestamo.TASA_INTERES <= 0)
        {
            throw new Exception("La tasa de interés es requerida");
        }

        if (solicitudPrestamo.ID_CATEGORIA_PRESTAMO <= 0)
        {
            throw new Exception("La categoría del préstamo es requerida");
        }

        if (solicitudPrestamo.ID_NIVEL_APROBACION_REQUERIDO <= 0)
        {
            throw new Exception("El nivel de aprobación requerido es requerido");
        }

        if (solicitudPrestamo.ESTATUS == null || solicitudPrestamo.ESTATUS == "")
        {
            throw new Exception("El estatus es requerido");
        }

        return solicitudPrestamo;
    }

    [HttpGet]
    public IActionResult SolicitudesPrestamos()
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var solicitudesPrestamos = _context.SolicitudPrestamos
                .FromSqlRaw("EXEC SP_LEER_SOLICITUDES_PRESTAMOS")
                .AsEnumerable()
                .ToList();

            var idRol = HttpContext.Session.GetInt32("ID_ROL");
            var idNivelAprobacion = HttpContext.Session.GetInt32("ID_NIVEL_APROBACION");
            var codigoEmpleado = HttpContext.Session.GetInt32("CODIGO_EMPLEADO");

            if (idRol != 6)
            {
                // Filtra las solicitudes solo las creadas por el usuario/empleado logeado
                var solicitudesPrestamosFiltradas = solicitudesPrestamos.Where(solicitud => solicitud.SOLICITUD_REALIZADA_POR == codigoEmpleado && solicitud.ESTATUS == "Pendiente").ToList();
                return View("~/Views/Prestamos/SolicitudesPrestamos/RegistroSolicitudesPrestamos.cshtml", solicitudesPrestamosFiltradas);
            }


            return View("~/Views/Prestamos/SolicitudesPrestamos/RegistroSolicitudesPrestamos.cshtml", solicitudesPrestamos);


        }
        catch (Exception ex)
        {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al obtener las solicitudes de préstamos:";
            Console.WriteLine("Error al obtener las solicitudes de préstamos: " + ex.Message + ex.Source); // Mensaje para el log en el server

            var emptySolicitudesPrestamos = new List<SolicitudPrestamo>();

            return View("~/Views/Prestamos/SolicitudesPrestamos/RegistroSolicitudesPrestamos.cshtml", emptySolicitudesPrestamos);
        }

    }


    [HttpGet]
    public JsonResult obtenerCategoriaPrestamo(int idCategoriaPrestamo)
    {

        try
        {
            var categoriaPrestamo = _context.CategoriaPrestamo
                .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
                .AsEnumerable()
                .ToList();


            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper,
                WriteIndented = true
            };

            // var json = JsonSerializer.Serialize(categoriaPrestamo, serializeOptions);
            return new JsonResult(categoriaPrestamo, serializeOptions);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }

    }

    [HttpGet]
    public IActionResult CrearSolicitudPrestamo()
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // Obtener datos necesarios para el formulario

        var socios = _context.Socios
            .FromSqlRaw("EXEC SP_LEER_SOCIOS")
            .AsEnumerable()
            .ToList();

        var categoriasPrestamo = _context.CategoriaPrestamo
            .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
            .AsEnumerable()
            .ToList();

        var codeudores = _context.Codeudores
            .FromSqlRaw("EXEC SP_LEER_CODEUDORES")
            .AsEnumerable()
            .ToList();


        var viewModel = new SolicitudPrestamoViewModel
        {
            Socios = socios,
            CategoriasPrestamos = categoriasPrestamo,
            Codeudores = codeudores
        };


        ViewBag.Title = "Crear Solicitud de Prestamo";
        return View("~/Views/Prestamos/SolicitudesPrestamos/FormSolicitudPrestamo.cshtml", viewModel);
    }

    [HttpGet]
    public IActionResult EditarSolicitudPrestamo(string IdSolicitudPrestamo)
    {

        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (IdSolicitudPrestamo != null)
        {

            var ID_SOLICITUD = Convert.ToInt32(IdSolicitudPrestamo);
            // Obtener datos necesarios para el formulario

            var socios = _context.Socios
                .FromSqlRaw("EXEC SP_LEER_SOCIOS")
                .AsEnumerable()
                .ToList();

            var categoriasPrestamo = _context.CategoriaPrestamo
                .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
                .AsEnumerable()
                .ToList();

            var codeudores = _context.Codeudores
                .FromSqlRaw("EXEC SP_LEER_CODEUDORES")
                .AsEnumerable()
                .ToList();

            var solicitudPrestamo = _context.SolicitudPrestamos
                .FromSqlRaw("EXEC SP_BUSCAR_SOLICITUD_PRESTAMO @ID_SOLICITUD = {0}", ID_SOLICITUD)
                .AsEnumerable()
                .FirstOrDefault();


            var viewModel = new SolicitudPrestamoViewModel
            {
                SolicitudPrestamo = solicitudPrestamo,
                Socios = socios,
                CategoriasPrestamos = categoriasPrestamo,
                Codeudores = codeudores
            };

            ViewBag.Title = "Editar Solicitud de Prestamo";
            return View("~/Views/Prestamos/SolicitudesPrestamos/FormSolicitudPrestamo.cshtml", viewModel);

        }


        TempData["openModal"] = true;
        TempData["Error"] = "No se encontró la solicitud de préstamo";
        return RedirectToAction("SolicitudesPrestamos", "Prestamos");
    }

    [HttpPost]
    public IActionResult ActualizarSolicitudPrestamo(SolicitudPrestamo solicitudPrestamo)
    {
        return View("~/Views/Prestamos/SolicitudesPrestamos/FormSolicitudPrestamo.cshtml", solicitudPrestamo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}