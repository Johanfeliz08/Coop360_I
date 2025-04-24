using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Coop360_I.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Auth");
    }

    [HttpPost]
    public IActionResult Login(int usuario, string contrasena)
    {

        if (string.IsNullOrEmpty(usuario.ToString()) || string.IsNullOrEmpty(contrasena))
        {
            TempData["openModal"] = true;
            TempData["Error"] = "Usuario o contraseña incorrectos.";
            Console.WriteLine("Error al iniciar sesion, usuario o contraseña incorrectos"); // Mensaje para el log en el server
            return View();
        }

        var Usuario = _context.Usuarios
        .FromSqlRaw("EXEC SP_BUSCAR_USUARIO @CODIGO_EMPLEADO = {0}", Convert.ToInt32(usuario))
        .AsEnumerable()
        .FirstOrDefault();

        if (Usuario != null)
        {
            var hasher = new PasswordHasher<Object>();
            var contrasenaValida = Usuario.CONTRASENA;
            var resultado = hasher.VerifyHashedPassword(new object(), contrasenaValida, contrasena);

            if (resultado == PasswordVerificationResult.Success)
            {
                var AuthUsuario = Usuario;

                // Guardar usuario en la sesion
                HttpContext.Session.SetInt32("ID_USUARIO", AuthUsuario.ID_USUARIO);
                HttpContext.Session.SetInt32("USUARIO", AuthUsuario.CODIGO_EMPLEADO);
                HttpContext.Session.SetString("P_NOMBRE", AuthUsuario.P_NOMBRE);
                HttpContext.Session.SetString("S_NOMBRE", AuthUsuario.S_NOMBRE);
                HttpContext.Session.SetString("P_APELLIDO", AuthUsuario.P_APELLIDO);
                HttpContext.Session.SetString("S_APELLIDO", AuthUsuario.S_APELLIDO);
                HttpContext.Session.SetString("FECHA_CREACION", AuthUsuario.FECHA_CREACION.ToString());
                HttpContext.Session.SetInt32("ID_ROL", AuthUsuario.ID_ROL);
                HttpContext.Session.SetInt32("ID_NIVEL_APROBACION", AuthUsuario.ID_NIVEL_APROBACION);

                if (AuthUsuario.ID_USUARIO > 0)
                {
                    var permisos = _context
                    .Set<PermisoCuenta>()
                    .FromSqlRaw("EXEC SP_GET_ALL_USER_PERMISSIONS @ID_USUARIO = {0}", Convert.ToInt32(AuthUsuario.ID_USUARIO))
                    .AsEnumerable()
                    .ToList();

                    // Convertir lista de permisos a json para poder ser almacenado en la sesion
                    var permisosJson = JsonSerializer.Serialize(permisos);
                    HttpContext.Session.SetString("Permisos", permisosJson);
                }

                return RedirectToAction("Home", "Dashboard");
            }
            else
            {
                TempData["openModal"] = true;
                TempData["Error"] = "Usuario o contraseña incorrectos.";
                Console.WriteLine("Error al iniciar sesion, usuario o contraseña incorrectos"); // Mensaje para el log en el server
                return View();
            }

        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
