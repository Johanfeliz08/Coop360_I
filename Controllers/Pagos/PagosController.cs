using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Coop360_I.Controllers;

public class PagosController : Controller
{

  private readonly ILogger<PagosController> _logger;
  private readonly ApplicationDbContext _context;

  public PagosController(ILogger<PagosController> logger, ApplicationDbContext context)
  {
    _logger = logger;
    _context = context;
  }

  [HttpGet]
  public IActionResult RegistrarPago()
  {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }


    var socios = _context.Socios
    .FromSqlRaw("EXEC SP_LEER_SOCIOS")
    .AsEnumerable()
    .ToList();

    var prestamos = _context.Prestamos
    .FromSqlRaw("EXEC SP_LEER_PRESTAMOS")
    .AsEnumerable()
    .ToList();

    var ViewModel = new PagoViewModel
    {
      Socios = socios,
      Prestamos = prestamos
    };

    ViewBag.Title = "Registrar pago";
    return View("FormRegistrarPago", ViewModel);

  }

  [HttpGet("/Pagos/ObtenerPrestamo/{idPrestamo}")]
  public JsonResult ObtenerPrestamo(int idPrestamo)
  {

    if (idPrestamo > 0)
    {

      var ID_PRESTAMO = Convert.ToInt32(idPrestamo);

      try
      {
        var prestamo = _context.Prestamos
        .FromSqlRaw("EXEC SP_BUSCAR_PRESTAMO @ID_PRESTAMO={0}", ID_PRESTAMO)
        .AsEnumerable()
        .FirstOrDefault();

        var numeroCuota = _context
        .Set<NumeroCuota>()
        .FromSqlRaw("EXEC SP_OBTENER_NUMERO_CUOTA_ACTUAL @ID_PRESTAMO = {0}", ID_PRESTAMO)
        .AsEnumerable()
        .FirstOrDefault();

        var serializeOptions = new JsonSerializerOptions
        {
          PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper,
          WriteIndented = true
        };

        return new JsonResult(new
        {
          prestamo = prestamo,
          numeroCuota = numeroCuota
        }, serializeOptions);

      }
      catch (Exception ex)
      {
        return Json(new { error = "Error al obtener el prestamo" + ex.Message });
      }

    }

    return Json(new { error = "Id del prestamo no valido " + idPrestamo });

  }


  [HttpPost]

  public async Task<IActionResult> GuardarPagoAsync(Pago pago)
  {

    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (pago != null)
    {
      pago.CODIGO_EMPLEADO = Convert.ToInt32(HttpContext.Session.GetInt32("ID_USUARIO"));

      // Obtener el prestamo para hacer los calculos

      var prestamo = _context.Prestamos
      .FromSqlRaw("EXEC SP_BUSCAR_PRESTAMO @ID_PRESTAMO={0}", pago.ID_PRESTAMO)
      .AsEnumerable()
      .FirstOrDefault();

      if (prestamo != null)
      {
        // Calcular el monto interes, monto capital y monto restante
        var MONTO_INTERES = (prestamo.BALANCE_RESTANTE * (prestamo.TASA_INTERES / 100));
        var IMPUESTO = MONTO_INTERES * 0.1m; // 10% de impuesto sobre el interes
        var MONTO_A_PAGAR = (prestamo.MONTO_APROBADO / prestamo.CANTIDAD_CUOTAS) + IMPUESTO + MONTO_INTERES;
        var MONTO_CAPITAL = MONTO_A_PAGAR - MONTO_INTERES - IMPUESTO;
        var MONTO_RESTANTE = prestamo.BALANCE_RESTANTE - MONTO_CAPITAL;

        pago.MONTO_INTERES = MONTO_INTERES;
        pago.IMPUESTO = IMPUESTO;
        pago.MONTO_A_PAGAR = MONTO_A_PAGAR;
        pago.MONTO_CAPITAL = MONTO_CAPITAL;
        pago.MONTO_RESTANTE = MONTO_RESTANTE;
        pago.ESTATUS = "Pagado";

        try
        {
          await _context.Database.ExecuteSqlRawAsync("EXEC SP_REGISTRAR_PAGO @ID_PRESTAMO = {0}, @NUMERO_CUOTA = {1}, @CODIGO_EMPLEADO = {2}, @MONTO_A_PAGAR = {3}, @MONTO_RECIBIDO = {4}, @MONTO_DEVUELTO = {5}, @MONTO_INTERES = {6}, @MONTO_CAPITAL = {7}, @MONTO_RESTANTE = {8}, @IMPUESTO = {9}, @ESTATUS = {10}",
          pago.ID_PRESTAMO, pago.NUMERO_CUOTA, pago.CODIGO_EMPLEADO, pago.MONTO_A_PAGAR, pago.MONTO_RECIBIDO, pago.MONTO_DEVUELTO, pago.MONTO_INTERES, pago.MONTO_CAPITAL, pago.MONTO_RESTANTE, pago.IMPUESTO, pago.ESTATUS);
          TempData["openModal"] = true;
          TempData["Success"] = "Pago registrado exitosamente";
          Console.WriteLine("Pago registrado exitosamente");
          return RedirectToAction("RegistrarPago", "Pagos");

        }
        catch (Exception ex)
        {
          TempData["openModal"] = true;
          TempData["Error"] = "Ha ocurrido un error al guardar el pago";
          Console.WriteLine("Error al guardar el pago: " + ex.Message);
          return RedirectToAction("RegistrarPago", "Pagos");
        }

      }

      throw new Exception("El prestamo no existe o no es valido");

    }

    TempData["openModal"] = true;
    TempData["Error"] = "El pago recibido es nulo o no valido";
    Console.WriteLine("El pago recibido es nulo o no valido");
    return RedirectToAction("RegistrarPago", "Pagos");
  }


  [HttpGet]
  public IActionResult ConsultarTablaAmortizacion()
  {
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    var prestamos = _context.Prestamos
        .FromSqlRaw("EXEC SP_LEER_PRESTAMOS")
        .AsEnumerable()
        .ToList();

    return View("~/Views/Pagos/ConsultarTablaAmortizacion.cshtml", prestamos);
  }

  [HttpGet]
  public IActionResult ConsultarPagos(string IdPrestamo)
  {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (IdPrestamo != null)
    {

      var ID_PRESTAMO = Convert.ToInt32(IdPrestamo);

      try
      {

        // Prestamo

        var prestamo = _context.Prestamos
        .FromSqlRaw("EXEC SP_BUSCAR_PRESTAMO @ID_PRESTAMO={0}", ID_PRESTAMO)
        .AsEnumerable()
        .FirstOrDefault();

        // Pagos

        var pagos = _context.Pagos
        .FromSqlRaw("EXEC SP_CREAR_TABLA_AMORTIZACION_PRESTAMO @ID_PRESTAMO={0}", ID_PRESTAMO)
        .AsEnumerable()
        .ToList();

        var ViewModel = new ConsultarPagosViewModel
        {
          Prestamo = prestamo,
          Pagos = pagos
        };

        ViewBag.Title = "Tabla de amortizacion";
        return View("~/Views/Pagos/ConsultarPagos.cshtml", ViewModel);
      }
      catch (Exception ex)
      {
        TempData["openModal"] = true;
        TempData["Error"] = "Error al obtener los pagos: " + ex.Message;
        Console.WriteLine("Error al obtener los pagos: " + ex.Message);
        return RedirectToAction("ConsultarTablaAmortizacion", "Pagos");
      }

    }

    TempData["openModal"] = true;
    TempData["Error"] = "El ID del prestamo es nulo o no valido";
    Console.WriteLine("El ID del prestamo es nulo o no valido");
    return RedirectToAction("ConsularTablaAmortizacion", "Pagos");


  }

}