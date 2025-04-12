using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;
using System.Text.Json;

namespace Coop360_I.Controllers;
public class EntidadesBancariasController : Controller
{
  private readonly ILogger<EntidadesBancariasController> _logger;
  private readonly ApplicationDbContext _context;

  public EntidadesBancariasController(ApplicationDbContext context, ILogger<EntidadesBancariasController> logger)
  {
    _logger = logger;
    _context = context;
  }

  private EntidadBancaria validarEntidadBancaria(EntidadBancaria entidadBancaria)
  {
    if (entidadBancaria == null)
    {
      throw new Exception("El servidor no puede procesar la solicitud, objeto entidad bancaria vacio");
    }

    var entidadBancariaValida = new EntidadBancaria
    {
      ID = Convert.ToInt32(entidadBancaria.ID),
      NOMBRE = entidadBancaria.NOMBRE
    };

    return entidadBancariaValida;
  }

  [HttpGet]
  public IActionResult RegistroEntidadesBancarias()
  {

    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    var permisosJson = HttpContext.Session.GetString("Permisos");
    var permisos = JsonSerializer.Deserialize<List<PermisoCuenta>>(permisosJson ?? "[]");

    var viewPermiso = "ConsultarEntidadesBancarias";
    var validarPermiso = permisos?.Where(permiso => permiso.PERMISO == viewPermiso).FirstOrDefault();

    if (validarPermiso == null)
    {
      return RedirectToAction("Home", "Dashboard");
    }

    try
    {
      var entidadesBancarias = _context.EntidadesBancarias
      .FromSqlRaw("EXEC SP_LEER_ENTIDADES_BANCARIAS")
      .AsEnumerable()
      .ToList();

      return View("~/Views/Empleados/EntidadesBancarias/RegistroEntidadesBancarias.cshtml", entidadesBancarias);
    }
    catch (Exception ex)
    {
      throw new Exception("Ha ocurrido un error al cargar los datos" + ex.Message);
    }


  }

  public IActionResult Crear()
  {
    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    ViewBag.Title = "Registro de Entidad Bancaria";
    return View("~/Views/Empleados/EntidadesBancarias/FormEntidadesBancarias.cshtml");
  }

  [HttpGet]
  public IActionResult Editar(string IdEntidadBancaria)
  {

    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (IdEntidadBancaria != null)
    {

      var ID_EntidadBancaria = Convert.ToInt32(IdEntidadBancaria);

      var entidadBancaria = _context.EntidadesBancarias
      .FromSqlRaw("EXEC SP_BUSCAR_ENTIDAD_BANCARIA @ID = {0}", ID_EntidadBancaria)
      .AsEnumerable()
      .FirstOrDefault();

      if (entidadBancaria == null)
      {
        TempData["openModal"] = true;
        TempData["Error"] = "Ha ocurrido un error al cargar los datos necesarios para la pantalla";
        return RedirectToAction("RegistroEntidadesBancarias");

      }

      ViewBag.Title = "Editar Entidad Bancaria";
      return View("~/Views/Empleados/EntidadesBancarias/FormEntidadesBancarias.cshtml", entidadBancaria);
    }
    else
    {
      TempData["openModal"] = true;
      TempData["Error"] = "Ha ocurrido un error al cargar los datos necesarios para la pantalla";
      return RedirectToAction("RegistroEntidadesBancarias");
    }

  }


  [HttpGet]
  public IActionResult Eliminar(string IdEntidadBancaria)
  {
    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (IdEntidadBancaria != null)
    {
      int ID_EntidadBancaria = Convert.ToInt32(IdEntidadBancaria);
      TempData["openModal"] = true;
      TempData["Tipo"] = "confirmation";
      TempData["Titulo"] = "¡Cuidado!";
      TempData["Confirmation"] = "¿Esta seguro que desea eliminar esta entidad bancaria?";
      TempData["Controlador"] = "EntidadesBancarias";
      TempData["Parametro"] = "IdEntidadBancaria";
      TempData["ID"] = ID_EntidadBancaria;
      return RedirectToAction("RegistroEntidadesBancarias");

    }
    TempData["openModal"] = true;
    TempData["Error"] = "El ID de la entidad bancaria no es valido";
    return RedirectToAction("RegistroEntidadesBancarias");
  }


  // Acciones

  // Guardar

  [HttpPost]
  public async Task<IActionResult> GuardarAsync(EntidadBancaria entidadBancaria)
  {
    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    var entidadBancariaValida = validarEntidadBancaria(entidadBancaria);

    try
    {
      await _context.Database.ExecuteSqlRawAsync("EXEC SP_CREAR_ENTIDAD_BANCARIA @NOMBRE = {0}", entidadBancariaValida.NOMBRE);
    }
    catch (Exception ex)
    {

      TempData["openModal"] = true;
      TempData["Error"] = "Ha ocurrido un error al guardar la entidad bancaria:";
      Console.WriteLine("Error al guardar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
      return RedirectToAction("RegistroEntidadesBancarias");
    }

    TempData["openModal"] = true;
    TempData["Success"] = "La entidad bancaria ha sido registrada correctamente.";
    Console.WriteLine("Entidad bancaria guardada con exito"); // Mensaje para el log en el server
    return RedirectToAction("RegistroEntidadesBancarias");

  }

  // Actualizar

  [HttpPost]
  public async Task<IActionResult> ActualizarAsync(EntidadBancaria entidadBancaria)
  {
    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    var entidadBancariaValida = validarEntidadBancaria(entidadBancaria);
    Console.WriteLine("Entidad bancaria valida: " + entidadBancariaValida.ID + " " + entidadBancariaValida.NOMBRE);
    try
    {
      await _context.Database.ExecuteSqlRawAsync("EXEC SP_ACTUALIZAR_ENTIDAD_BANCARIA @ID = {0}, @NOMBRE = {1}", entidadBancariaValida.ID, entidadBancariaValida.NOMBRE);
    }
    catch (Exception ex)
    {
      TempData["openModal"] = true;
      TempData["Error"] = "Ha ocurrido un error al actualizar la entidad bancaria:";
      Console.WriteLine("Error al actualizar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
      return RedirectToAction("RegistroEntidadesBancarias");
    }

    TempData["openModal"] = true;
    TempData["Success"] = "La entidad bancaria ha sido actualizada correctamente.";
    Console.WriteLine("Entidad bancaria actualizada con exito"); // Mensaje para el log en el server
    return RedirectToAction("RegistroEntidadesBancarias");



  }

  // Eliminar

  [HttpPost]
  public async Task<IActionResult> EliminarAsync(string IdEntidadBancaria)
  {
    // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (IdEntidadBancaria != null)
    {
      int ID_EntidadBancaria = Convert.ToInt32(IdEntidadBancaria);

      try
      {
        await _context.Database.ExecuteSqlRawAsync("EXEC SP_ELIMINAR_ENTIDAD_BANCARIA @ID = {0}", ID_EntidadBancaria);
      }
      catch (Exception ex)
      {

        if (ex.Message.Contains("Esta entidad bancaria está actualmente en uso, por lo que no puede ser eliminada."))
        {
          TempData["openModal"] = true;
          TempData["Error"] = "La entidad bancaria está actualmente en uso, por lo que no puede ser eliminada.";
          Console.WriteLine("Error al eliminar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
          return RedirectToAction("RegistroEntidadesBancarias");
        }

        TempData["openModal"] = true;
        TempData["Error"] = "Ha ocurrido un error al eliminar la entidad bancaria:";
        Console.WriteLine("Error al eliminar el registro: " + ex.Message + ex.Source); // Mensaje para el log en el server
        return RedirectToAction("RegistroEntidadesBancarias");
      }

      TempData["openModal"] = true;
      TempData["Success"] = "La entidad bancaria ha sido eliminada correctamente.";
      Console.WriteLine("Entidad bancaria eliminada con exito"); // Mensaje para el log en el server
      return RedirectToAction("RegistroEntidadesBancarias");

    }

    TempData["openModal"] = true;
    TempData["Error"] = "El ID de la entidad bancaria no es valido";
    return RedirectToAction("RegistroEntidadesBancarias");

  }




}