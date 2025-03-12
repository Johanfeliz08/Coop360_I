using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;

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

    [HttpPost]
    public IActionResult Login(int usuario, string contrasena)
    {   
        var AuthUsuario = _context.Usuarios.FirstOrDefault(u => u.CODIGO_EMPLEADO == Convert.ToInt32(usuario) && u.CONTRASENA == contrasena);
        if (AuthUsuario != null)
        {
            return RedirectToAction("Home", "Dashboard");
        }
        else
        {
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
