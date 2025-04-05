using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;

namespace Coop360_I.Controllers;

public class CategoriaPrestamosController : Controller {

  private readonly ILogger<CategoriaPrestamosController> _logger;
  private readonly ApplicationDbContext _context;

  public CategoriaPrestamosController(ApplicationDbContext context, ILogger<CategoriaPrestamosController> logger) {
    _logger = logger;
    _context = context;
  }

  private CategoriaPrestamo validarCategoriaPrestamo(CategoriaPrestamo categoriaPrestamo) {
    if (categoriaPrestamo == null) {
      throw new Exception("El servidor no puede procesar la solicitud, objeto categoriaPrestamo vacio");
    }
    
    if (string.IsNullOrEmpty(categoriaPrestamo.NOMBRE) || categoriaPrestamo.TASA_INTERES_BASE <= 0 || categoriaPrestamo.TASA_INTERES_MINIMA <= 0 || categoriaPrestamo.TASA_INTERES_MAXIMA <= 0 || categoriaPrestamo.PLAZO_ESTANDAR <= 0 || categoriaPrestamo.PLAZO_MINIMO <= 0 || categoriaPrestamo.PLAZO_MAXIMO <= 0) {
      throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
    }

    var categoriaPrestamoValido = new CategoriaPrestamo {
      ID_CATEGORIA_PRESTAMO = Convert.ToInt32(categoriaPrestamo.ID_CATEGORIA_PRESTAMO),
      NOMBRE = categoriaPrestamo.NOMBRE,
      DESCRIPCION = categoriaPrestamo.DESCRIPCION,
      TASA_INTERES_BASE = Convert.ToDecimal(categoriaPrestamo.TASA_INTERES_BASE),
      TASA_INTERES_MINIMA = Convert.ToDecimal(categoriaPrestamo.TASA_INTERES_MINIMA),
      TASA_INTERES_MAXIMA = Convert.ToDecimal(categoriaPrestamo.TASA_INTERES_MAXIMA),
      PLAZO_ESTANDAR = Convert.ToInt32(categoriaPrestamo.PLAZO_ESTANDAR),
      PLAZO_MINIMO = Convert.ToInt32(categoriaPrestamo.PLAZO_MINIMO),
      PLAZO_MAXIMO = Convert.ToInt32(categoriaPrestamo.PLAZO_MAXIMO),
      FECHA_CREACION = categoriaPrestamo.FECHA_CREACION,
      CREADO_POR = Convert.ToInt32(categoriaPrestamo.CREADO_POR)
    };

    return categoriaPrestamoValido;
  }

  [HttpGet]
  public IActionResult RegistroCategoriaPrestamos() {
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
      return RedirectToAction("Login", "Auth");
    }

  var categoriasPrestamo = _context.CategoriaPrestamo
    .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
    .AsEnumerable()
    .ToList();

    return View("~/Views/Prestamos/CategoriaPrestamos/RegistroCategoriaPrestamos.cshtml", categoriasPrestamo);
    
  }

  [HttpGet]
  public IActionResult Crear() {

    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
      return RedirectToAction("Login", "Auth");
    }
    
    ViewBag.Title = "Registro de categoria prestamo";
    return View("~/Views/Prestamos/CategoriaPrestamos/FormCategoriaPrestamos.cshtml");
  }
  

  [HttpGet]

  // View para editar categoria prestamo
  public IActionResult Editar(string IdCategoriaPrestamo) {

    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
      return RedirectToAction("Login", "Auth");
    }

    if (IdCategoriaPrestamo != null) {

      var ID_CATEGORIA_PRESTAMO = Convert.ToInt32(IdCategoriaPrestamo);

      var categoriaPrestamo = _context.CategoriaPrestamo
      .FromSqlRaw("EXEC SP_BUSCAR_CATEGORIA_PRESTAMO @ID_CATEGORIA_PRESTAMO = {0}", ID_CATEGORIA_PRESTAMO)
      .AsEnumerable()
      .FirstOrDefault();

      ViewBag.Title = "Editar categoria prestamo";
      return View("~/Views/Prestamos/CategoriaPrestamos/FormCategoriaPrestamos.cshtml", categoriaPrestamo);
      
    } else {
      ViewBag.Error = "El ID de la categoria prestamo no es valido";
      return RedirectToAction("RegistroCategoriaPrestamos");
    }
    
  }

  [HttpGet]

  // Eliminar categoria prestamo
  public IActionResult Eliminar(string IdCategoriaPrestamo) {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
      return RedirectToAction("Login", "Auth");
    }

    if (IdCategoriaPrestamo != null) {
      var ID_CATEGORIA_PRESTAMO = Convert.ToInt32(IdCategoriaPrestamo);
      TempData["openModal"] = true;
      TempData["Tipo"] = "confirmation";
      TempData["Titulo"] = "¡Cuidado!";
      TempData["Confirmation"] = "¿Esta seguro que desea eliminar esta categoria prestamo?";
      TempData["Controlador"] = "CategoriaPrestamos";
      TempData["Parametro"] = "IdCategoriaPrestamo";
      TempData["ID"] = ID_CATEGORIA_PRESTAMO;
      return RedirectToAction("RegistroCategoriaPrestamos");
    }

    TempData["openModal"] = true;
    TempData["Error"] = "El ID de la categoria prestamo no es valido";
    return RedirectToAction("RegistroCategoriaPrestamos");
  }

  [HttpPost]

  // Guardar categoria prestamo
  public async Task<IActionResult> GuardarAsync(CategoriaPrestamo categoriaPrestamo) {

    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
      return RedirectToAction("Login", "Auth");
    }
  
    // Valida la categoria prestamo

    var categoriaPrestamoValido = validarCategoriaPrestamo(categoriaPrestamo);

    try {
      await _context.Database.ExecuteSqlRawAsync(
        "EXEC SP_CREAR_CATEGORIA_PRESTAMO @NOMBRE = {0}, @DESCRIPCION = {1}, @TASA_INTERES_BASE = {2}, @TASA_INTERES_MINIMA = {3}, @TASA_INTERES_MAXIMA = {4}, @PLAZO_ESTANDAR = {5}, @PLAZO_MINIMO = {6}, @PLAZO_MAXIMO = {7}, @CREADO_POR = {8}",
        categoriaPrestamoValido.NOMBRE, categoriaPrestamoValido.DESCRIPCION, categoriaPrestamoValido.TASA_INTERES_BASE, categoriaPrestamoValido.TASA_INTERES_MINIMA, categoriaPrestamoValido.TASA_INTERES_MAXIMA, categoriaPrestamoValido.PLAZO_ESTANDAR, categoriaPrestamoValido.PLAZO_MINIMO, categoriaPrestamoValido.PLAZO_MAXIMO, categoriaPrestamoValido.CREADO_POR ?? 0
      );
    } catch (Exception ex) {
      TempData["openModal"] = true;
      TempData["Error"] = "Ha ocurrido un error al guardar la categoria prestamo:";
      Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
      return RedirectToAction("RegistroCategoriaPrestamos");
    }

    TempData["openModal"] = true;
    TempData["Success"] = "La categoria prestamo ha sido registrada correctamente.";
    Console.WriteLine("Categoria prestamo guardada con exito"); // Mensaje para el log en el server
    return RedirectToAction("RegistroCategoriaPrestamos");
  
  }

  // Actualizar categoria prestamo
  [HttpPost]
  public async Task<IActionResult> ActualizarAsync(CategoriaPrestamo categoriaPrestamo) {

    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
      return RedirectToAction("Login", "Auth");
    }

    // Valida la categoria prestamo
    var categoriaPrestamoValido = validarCategoriaPrestamo(categoriaPrestamo);

    try {
      await _context.Database.ExecuteSqlRawAsync(
        "EXEC SP_ACTUALIZAR_CATEGORIA_PRESTAMO @ID_CATEGORIA_PRESTAMO = {0}, @NOMBRE = {1}, @DESCRIPCION = {2}, @TASA_INTERES_BASE = {3}, @TASA_INTERES_MINIMA = {4}, @TASA_INTERES_MAXIMA = {5}, @PLAZO_ESTANDAR = {6}, @PLAZO_MINIMO = {7}, @PLAZO_MAXIMO = {8}",
        categoriaPrestamoValido.ID_CATEGORIA_PRESTAMO, categoriaPrestamoValido.NOMBRE, categoriaPrestamoValido.DESCRIPCION, categoriaPrestamoValido.TASA_INTERES_BASE, categoriaPrestamoValido.TASA_INTERES_MINIMA, categoriaPrestamoValido.TASA_INTERES_MAXIMA, categoriaPrestamoValido.PLAZO_ESTANDAR, categoriaPrestamoValido.PLAZO_MINIMO, categoriaPrestamoValido.PLAZO_MAXIMO
      );
    } catch (Exception ex) {
      TempData["openModal"] = true;
      TempData["Error"] = "Ha ocurrido un error al actualizar la categoria prestamo:";
      Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
      return RedirectToAction("RegistroCategoriaPrestamos");
    }

    TempData["openModal"] = true;
    TempData["Success"] = "La categoria prestamo ha sido actualizada correctamente.";
    Console.WriteLine("Categoria prestamo actualizada con exito"); // Mensaje para el log en el server
    return RedirectToAction("RegistroCategoriaPrestamos");
    
  }

  // Eliminar categoria prestamo
  [HttpPost]
  public async Task<IActionResult> EliminarAsync(string IdCategoriaPrestamo) {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
      return RedirectToAction("Login", "Auth");
    }

    if (IdCategoriaPrestamo != null) {

      int ID_CATEGORIA_PRESTAMO = Convert.ToInt32(IdCategoriaPrestamo);

      try {
        
        await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_CATEGORIA_PRESTAMO @ID_CATEGORIA_PRESTAMO = {0}", ID_CATEGORIA_PRESTAMO);
      
      } catch (Exception ex) {
      
        TempData["openModal"] = true;
        TempData["Error"] = "Ha ocurrido un error al eliminar la categoria prestamo:";
        Console.WriteLine("Error al eliminar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
        return RedirectToAction("RegistroCategoriaPrestamos");
      
      }

      TempData["openModal"] = true;
      TempData["Success"] = "La categoria prestamo ha sido eliminada correctamente.";
      Console.WriteLine("Categoria prestamo eliminada con exito"); // Mensaje para el log en el server
      return RedirectToAction("RegistroCategoriaPrestamos");
    }

    TempData["openModal"] = true;
    TempData["Error"] = "El ID de la categoria prestamo no es valido";
    return RedirectToAction("RegistroCategoriaPrestamos");

  }

}