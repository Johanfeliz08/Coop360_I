using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;

namespace Coop360_I.Controllers;

public class PrestamosController : Controller
{
    private readonly ILogger<PrestamosController> _logger;
    private readonly ApplicationDbContext _context;

    public PrestamosController(ApplicationDbContext context, ILogger<PrestamosController> logger)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult SolicitudPrestamo()
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}