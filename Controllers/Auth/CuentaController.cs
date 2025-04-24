using Coop360_I.Data;
using Microsoft.AspNetCore.Identity;
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

                if (empleado != null)
                {
                    return View(empleado);
                }
                else
                {
                    ViewBag.Error = "No se encontró el empleado";
                    return View();
                }
            }

            return RedirectToAction("Login", "Auth");

        }

        [HttpGet]
        public IActionResult ModalCambiarContrasena()
        {

            if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Obtener el ID del usuario actual desde la sesión
            var ID_USUARIO = Convert.ToInt32(HttpContext.Session.GetInt32("USUARIO"));
            TempData["openModalAprobacionRechazo"] = true;
            TempData["CambiarContrasena"] = "true";
            TempData["Controlador"] = "Cuenta";
            TempData["Parametro"] = "IdUsuario";
            TempData["ID"] = ID_USUARIO;
            return RedirectToAction("Perfil", new { IdUsuario = ID_USUARIO });


        }

        [HttpPost]

        public async Task<IActionResult> CambiarContrasenaAsync(string IdUsuario, string confirmarContrasena)
        {
            if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
            {
                return RedirectToAction("Login", "Auth");
            }


            if (!string.IsNullOrEmpty(IdUsuario) && !string.IsNullOrEmpty(confirmarContrasena))
            {
                int ID_USUARIO = Convert.ToInt32(IdUsuario);
                string contrasena = confirmarContrasena;

                var hasher = new PasswordHasher<object>();
                string hashedContrasena = hasher.HashPassword(new object(), contrasena);

                try
                {

                    await _context.Database.ExecuteSqlRawAsync("EXEC SP_CAMBIAR_CONTRASENA @CODIGO_EMPLEADO = {0}, @CONTRASENA = {1}", ID_USUARIO, hashedContrasena);
                    TempData["openModal"] = true;
                    TempData["Success"] = "Su contraseña ha sido cambiada con exito";
                    Console.WriteLine("Contraseña cambiada con exito"); // Mensaje para el log en el server
                    return RedirectToAction("Login", "Auth");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al cambiar la contraseña: " + ex.Message);
                    TempData["openModal"] = true;
                    TempData["Error"] = "Ha ocurrido un error al cambiar la contraseña:";
                    return RedirectToAction("Perfil", new { IdUsuario = ID_USUARIO });
                }

            }

            TempData["openModal"] = true;
            TempData["Error"] = "Valores recibidos no son validos";
            Console.WriteLine("Valores recibidos no son validos"); // Mensaje para el log en el server
            return RedirectToAction("Perfil", new { IdUsuario = IdUsuario });


        }
    }
}