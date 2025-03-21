using Coop360_I.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coop360_I.Controllers
{
    public class CuentaController : Controller
    {

    private readonly ILogger<CuentaController> _logger;
    private readonly ApplicationDbContext _context;

    public CuentaController(ApplicationDbContext context, ILogger<CuentaController> logger)
    {
        _logger = logger;
        _context = context;
    }
        public IActionResult Perfil()
        {

        // Obtener el ID del usuario actual desde la sesión
        var AuthUsuario = HttpContext.Session.GetInt32("USUARIO");
        if (AuthUsuario != null)
        {

          var empleado = _context.Empleados
         .FromSqlRaw("EXEC SP_BUSCAR_EMPLEADO @CODIGO_EMPLEADO = {0}", Convert.ToInt32(AuthUsuario))
         .AsEnumerable()
         .FirstOrDefault();

            if (empleado != null){
                return View(empleado);
            } else {
                ViewBag.Error = "No se encontró el empleado";
                return View();
            }
        }

            return RedirectToAction("Login", "Auth");
        
        }
        }
    }