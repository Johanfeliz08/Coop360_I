using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Coop360_I.Controllers;


public class ConfiguracionController : Controller
{

  private readonly ILogger<DashboardController> _logger;
  private readonly ApplicationDbContext _context;

  public ConfiguracionController(ApplicationDbContext context, ILogger<DashboardController> logger)
  {
    _logger = logger;
    _context = context;
  }

  [HttpGet]
  public IActionResult CuentasUsuarios()
  {

    try
    {

      var usuarios = _context
        .Set<CuentaUsuario>()
        .FromSqlRaw("EXEC SP_LEER_CUENTAS_USUARIO")
        .AsEnumerable()
        .ToList();

      return View("CuentasUsuarios", usuarios);

    }
    catch (Exception ex)
    {
      Console.WriteLine("Error al obtener los usuarios: " + ex.Message);
      throw new Exception("Error al obtener los usuarios: " + ex.Message);
    }
  }

  [HttpGet]
  public IActionResult EditarCuentaUsuario(string idUsuario)
  {

    if (idUsuario != null)
    {

      int ID_USUARIO = Convert.ToInt32(idUsuario);

      var usuario = _context
      .Set<CuentaUsuario>()
      .FromSqlRaw("EXEC SP_BUSCAR_CUENTA_USUARIO @ID_USUARIO = {0}", ID_USUARIO)
      .AsEnumerable()
      .FirstOrDefault();

      var roles = _context.Roles
      .FromSqlRaw("EXEC SP_LEER_ROLES")
      .AsEnumerable()
      .ToList();

      var permisos = _context.Permisos
      .FromSqlRaw("EXEC SP_LEER_PERMISOS")
      .AsEnumerable()
      .ToList();

      var permisosRol = _context
      .Set<PermisoCuenta>()
      .FromSqlRaw("EXEC SP_BUSCAR_PERMISOS_USUARIOS_POR_ROL @ID_USUARIO = {0}", ID_USUARIO)
      .AsEnumerable()
      .ToList();

      var permisosUsuario = _context
      .Set<PermisoCuenta>()
      .FromSqlRaw("EXEC SP_BUSCAR_PERMISOS_USUARIOS_POR_USUARIO @ID_USUARIO = {0}", ID_USUARIO)
      .AsEnumerable()
      .ToList();

      if (usuario == null || roles == null || permisos == null || permisosRol == null || permisosUsuario == null)
      {
        TempData["openModal"] = "true";
        TempData["Error"] = "No se encontraron los datos necesarios para cargar este view";
        return RedirectToAction("CuentasUsuarios", "Configuracion");
      }

      var viewModel = new CuentaUsuarioViewModel
      {
        CuentaUsuario = usuario,
        Roles = roles,
        Permisos = permisos,
        PermisosRol = permisosRol,
        PermisosUsuario = permisosUsuario
      };

      ViewBag.Title = "Editar Usuario";
      return View("FormCuentaUsuarios", viewModel);


    }

    TempData["openModal"] = "true";
    TempData["Error"] = "ID cuenta no valido";
    return RedirectToAction("Usuarios", "Configuracion");

  }

  [HttpPost]

  public async Task<IActionResult> ActualizarCuentaUsuario(string id_usuario, string id_rol, List<string> permisosSeleccionados)
  {

    if (id_usuario != null)
    {
      int ID_USUARIO = Convert.ToInt32(id_usuario);
      object ID_ROL = id_rol != null ? Convert.ToInt32(id_rol) : DBNull.Value;

      // Siempre actualizar el rol
      await _context.Database.ExecuteSqlRawAsync(
        "UPDATE USUARIOS SET ID_ROL = {0} WHERE ID_USUARIO = {1}", ID_ROL, ID_USUARIO);

      // Siempre eliminar los permisos existentes
      await _context.Database.ExecuteSqlRawAsync(
        "DELETE FROM USUARIOS_PERMISOS WHERE ID_USUARIO = {0}", ID_USUARIO);

      // Si hay nuevos permisos, insertarlos
      if (permisosSeleccionados != null && permisosSeleccionados.Count > 0)
      {
        foreach (var permiso in permisosSeleccionados)
        {
          await _context.Database.ExecuteSqlRawAsync(
            "INSERT INTO USUARIOS_PERMISOS (ID_USUARIO, ID_PERMISO) VALUES ({0}, {1})",
            ID_USUARIO, permiso
          );
        }
      }

      TempData["openModal"] = "true";
      TempData["Success"] = "Usuario actualizado correctamente";
      Console.WriteLine("Usuario actualizado correctamente");

      // Validar si el usuario editado es el usuario logeado, si es asi, mandarlo al login

      return RedirectToAction("CuentasUsuarios", "Configuracion");

    }

    TempData["openModal"] = "true";
    TempData["Error"] = "ID usuario no valido";
    Console.WriteLine("ID usuario no valido");
    return RedirectToAction("CuentasUsuarios", "Configuracion");

  }


  [HttpGet]
  public IActionResult Roles()
  {
    return View();
  }

  [HttpGet]
  public IActionResult Permisos()
  {
    return View();
  }


}
