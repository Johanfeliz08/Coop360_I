using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;

namespace Coop360_I.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Home()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
