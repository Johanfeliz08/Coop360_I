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

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    try
    {

      var roles = _context.Roles
        .FromSqlRaw("EXEC SP_LEER_ROLES")
        .AsEnumerable()
        .ToList();

      return View("Roles", roles);

    }
    catch (Exception ex)
    {
      Console.WriteLine("Error al obtener los roles: " + ex.Message);
      TempData["openModal"] = true;
      TempData["Error"] = "Ha ocurrido un error al obtener los roles: " + ex.Message;
      return View("Roles", new List<Rol>());
    }

  }

  [HttpGet]
  public IActionResult EditarRol(string IdRol)
  {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (IdRol != null)
    {

      int ID_ROL = Convert.ToInt32(IdRol);

      try
      {

        var rol = _context.Roles
        .FromSqlRaw("EXEC SP_BUSCAR_ROL @ID_ROL = {0}", ID_ROL)
        .AsEnumerable()
        .FirstOrDefault();

        var permisos = _context.Permisos
        .FromSqlRaw("EXEC SP_LEER_PERMISOS")
        .AsEnumerable()
        .ToList();

        var permisosRol = _context
        .Set<PermisoRol>()
        .FromSqlRaw("EXEC SP_BUSCAR_PERMISOS_ROL @ID_ROL = {0}", ID_ROL)
        .AsEnumerable()
        .ToList();

        var ViewModel = new PermisosRolViewModel
        {
          Rol = rol,
          Permisos = permisos,
          PermisosRol = permisosRol
        };

        ViewBag.Title = "Editar Rol";
        return View("FormRoles", ViewModel);

      }
      catch (Exception ex)
      {
        Console.WriteLine("Error al obtener los datos necesarios para el view: " + ex.Message);
        TempData["openModal"] = true;
        TempData["Error"] = "Error al obtener los datos necesarios para el view: " + ex.Message;
        RedirectToAction("Roles", "Configuracion");
      }
    }

    TempData["openModal"] = true;
    TempData["Error"] = "ID rol no valido";
    return RedirectToAction("Roles", "Configuracion");

  }

  [HttpGet]
  public IActionResult CrearRol()
  {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    try
    {

      var permisos = _context.Permisos
        .FromSqlRaw("EXEC SP_LEER_PERMISOS")
        .AsEnumerable()
        .ToList();

      ViewBag.Title = "Crear Rol";
      var ViewModel = new PermisosRolViewModel
      {
        Permisos = permisos
      };

      return View("FormRoles", ViewModel);

    }
    catch (Exception ex)
    {
      Console.WriteLine("Error al obtener los datos necesarios para el view: " + ex.Message);
      TempData["openModal"] = true;
      TempData["Error"] = "Error al obtener los datos necesarios para el view: " + ex.Message;
      RedirectToAction("Roles", "Configuracion");
    }

    return RedirectToAction("Roles", "Configuracion");

  }

  [HttpGet]
  public IActionResult EliminarRol(string IdRol)
  {
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (IdRol != null)
    {

      int ID_ROL = Convert.ToInt32(IdRol);
      TempData["openModal"] = true;
      TempData["Tipo"] = "confirmation";
      TempData["Titulo"] = "¡Cuidado!";
      TempData["Confirmation"] = "¿Esta seguro que desea eliminar este rol?";
      TempData["Controlador"] = "Configuracion";
      TempData["Parametro"] = "IdRol";
      TempData["ID"] = ID_ROL;
      return RedirectToAction("Roles", "Configuracion");

    }

    TempData["openModal"] = true;
    TempData["Error"] = "ID rol no valido";
    Console.WriteLine("ID rol no valido");
    return RedirectToAction("Roles", "Configuracion");

  }

  [HttpPost]
  public async Task<IActionResult> GuardarRol(string nombre_rol, string descripcion_rol, List<string> permisosSeleccionados)
  {
    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (nombre_rol != null)
    {

      // Insertamos el rol y luego obtenemos toda la informacion del rol creado
      var Rol = _context.Roles
      .FromSqlRaw("EXEC SP_CREAR_ROL @NOMBRE = {0}, @DESCRIPCION = {1}", nombre_rol, descripcion_rol)
      .AsEnumerable()
      .FirstOrDefault();

      // Sacamos el id del rol creado
      int ID_ROL = Convert.ToInt32(Rol?.ID_ROL);

      // Utilizamos dicho id del rol para insertar los permisos
      if (ID_ROL > 0 && permisosSeleccionados != null && permisosSeleccionados.Count > 0)
      {

        foreach (var permiso in permisosSeleccionados)
        {
          await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_ROL_PERMISO @ID_ROL = {0}, @ID_PERMISO = {1}",
            ID_ROL, permiso
          );
        }

        TempData["openModal"] = true;
        TempData["Success"] = "Rol creado correcctamente";
        Console.WriteLine("Rol creado correctamente");
        return RedirectToAction("Roles", "Configuracion");

      }


    }

    TempData["openModal"] = true;
    TempData["Error"] = "El nombre del rol es requerido";
    Console.WriteLine("El nombre del rol es requerido");
    return RedirectToAction("Roles", "Configuracion");

  }

  [HttpPost]
  public async Task<IActionResult> ActualizarRol(string id_rol, string nombre_rol, string descripcion_rol, List<string> permisosSeleccionados)
  {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (id_rol != null)
    {
      int ID_ROL = Convert.ToInt32(id_rol);

      // Siempre actualizar el nombre del rol
      await _context.Database.ExecuteSqlRawAsync(
        "UPDATE ROLES SET NOMBRE = {0}, DESCRIPCION = {1} WHERE ID_ROL = {2}", nombre_rol, descripcion_rol, ID_ROL);

      // Siempre eliminar los permisos existentes
      await _context.Database.ExecuteSqlRawAsync(
        "DELETE FROM ROLES_PERMISOS WHERE ID_ROL = {0}", ID_ROL);

      // Si hay nuevos permisos, insertarlos
      if (permisosSeleccionados != null && permisosSeleccionados.Count > 0)
      {
        foreach (var permiso in permisosSeleccionados)
        {
          await _context.Database.ExecuteSqlRawAsync(
            "INSERT INTO ROLES_PERMISOS (ID_ROL, ID_PERMISO) VALUES ({0}, {1})",
            ID_ROL, permiso
          );
        }
      }

      TempData["openModal"] = true;
      TempData["Success"] = "Rol actualizado correctamente";
      Console.WriteLine("Rol actualizado correctamente");

      return RedirectToAction("Roles", "Configuracion");

    }

    TempData["openModal"] = true;
    TempData["Error"] = "ID rol no valido";
    Console.WriteLine("ID rol no valido");
    return RedirectToAction("Roles", "Configuracion");

  }

  [HttpPost]
  public async Task<IActionResult> Eliminar(string IdRol)
  {

    if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
    {
      return RedirectToAction("Login", "Auth");
    }

    if (IdRol != null)
    {
      int ID_ROL = Convert.ToInt32(IdRol);

      // Cambiar los empleados que tienen ese rol a nulo
      await _context.Database.ExecuteSqlRawAsync(
        "UPDATE USUARIOS SET ID_ROL = NULL WHERE ID_ROL = {0}", ID_ROL);

      // Eliminar los permisos del rol
      await _context.Database.ExecuteSqlRawAsync("DELETE FROM ROLES_PERMISOS WHERE ID_ROL = {0}", ID_ROL);

      // Eliminar el rol
      await _context.Database.ExecuteSqlRawAsync(
        "DELETE FROM ROLES WHERE ID_ROL = {0}", ID_ROL);

      TempData["openModal"] = true;
      TempData["Success"] = "Rol eliminado correctamente";
      Console.WriteLine("Rol eliminado correctamente");

      return RedirectToAction("Roles", "Configuracion");

    }

    TempData["openModal"] = true;
    TempData["Error"] = "ID rol no valido";
    Console.WriteLine("ID rol no valido");
    return RedirectToAction("Roles", "Configuracion");

  }
}