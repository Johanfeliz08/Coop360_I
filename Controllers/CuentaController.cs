using Coop360_I.Data;
using Microsoft.AspNetCore.Mvc;

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
        var Usuario_Actual = HttpContext.Session.GetInt32("USUARIO");
        if (Usuario_Actual != null)
        {
                // Buscar el empleado en la base de datos por el ID del usuario
                Empleado? Data_Empleado = _context.Empleados
                    .Where(e => e.CODIGO_EMPLEADO == Usuario_Actual)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (Data_Empleado != null)
                {
                    return View(Data_Empleado);
                }
                else
                {
                    ViewBag.Error = "No se encontró el empleado";
                    return View();
                }
            }

            return RedirectToAction("Login", "Auth");
        
        }
        }
    }