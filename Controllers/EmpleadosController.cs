using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;

namespace Coop360_I.Controllers;

public class EmpleadosController : Controller {
    private readonly ILogger<EmpleadosController> _logger;
    private readonly ApplicationDbContext _context;

    public EmpleadosController(ApplicationDbContext context, ILogger<EmpleadosController> logger) {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult RegistroEmpleados() {

        var empleados = _context.Empleados
        .FromSqlRaw("EXEC SP_LEER_EMPLEADOS")
        .AsEnumerable()
        .ToList();
        return View(empleados);
    
    }

    public IActionResult Crear () {
        return View("FormEmpleados");
    }

    // [HttpGet]
    // public IActionResult Create() {
    //     return View();
    // }

    // [HttpPost]
    // public IActionResult Create(Empleado empleado) {
    //     if (ModelState.IsValid) {
    //         _context.Empleados.Add(empleado);
    //         _context.SaveChanges();
    //         return RedirectToAction("Index");
    //     }
    //     return View(empleado);
    // }

    // [HttpGet]
    // public IActionResult Edit(int id) {
    //     var empleado = _context.Empleados.Find(id);
    //     return View(empleado);
    // }

    // [HttpPost]
    // public IActionResult Edit(Empleado empleado) {
    //     if (ModelState.IsValid) {
    //         _context.Empleados.Update(empleado);
    //         _context.SaveChanges();
    //         return RedirectToAction("Index");
    //     }
    //     return View(empleado);
    // }

    // [HttpGet]
    // public IActionResult Delete(int id) {
    //     var empleado = _context.Empleados.Find(id);
    //     return View(empleado);
    // }

    // [HttpPost]
    // public IActionResult Delete(Empleado empleado) {
    //     _context.Empleados.Remove(empleado);
    //     _context.SaveChanges();
    //     return RedirectToAction("Index");
    // }
}