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

        var regiones = _context.Regiones
        .FromSqlRaw("EXEC SP_LEER_REGIONES")
        .AsEnumerable()
        .ToList();

        var provincias = _context.Provincias
        .FromSqlRaw("EXEC SP_LEER_PROVINCIAS")
        .AsEnumerable()
        .ToList();

        var ciudades = _context.Ciudades
        .FromSqlRaw("EXEC SP_LEER_CIUDADES")
        .AsEnumerable()
        .ToList();

        var sectores = _context.Sectores
        .FromSqlRaw("EXEC SP_LEER_SECTORES")
        .AsEnumerable()
        .ToList();

        var puestos = _context.Puestos
        .FromSqlRaw("EXEC SP_LEER_PUESTOS")
        .AsEnumerable()
        .ToList();

        var departamentos = _context.Departamentos
        .FromSqlRaw("EXEC SP_LEER_DEPARTAMENTOS")
        .AsEnumerable()
        .ToList();

        var nivelesAprobacion = _context.NivelesAprobacion
        .FromSqlRaw("EXEC SP_LEER_NIVELES_APROBACION")
        .AsEnumerable()
        .ToList();

        var entidadesBancarias = _context.EntidadesBancarias
        .FromSqlRaw("EXEC SP_LEER_ENTIDADES_BANCARIAS")
        .AsEnumerable()
        .ToList();

        var viewModel = new EmpleadoFormViewModel
        {
            Regiones = regiones,
            Provincias = provincias,
            Ciudades = ciudades,
            Sectores = sectores,
            Puestos = puestos,
            Departamentos = departamentos,
            NivelesAprobacion = nivelesAprobacion,
            EntidadesBancarias = entidadesBancarias
        };

        return View("FormEmpleados",viewModel);
    }

        [HttpPost]
        public IActionResult Guardar(Empleado empleado) {
        
            if (empleado != null) {
                var context = _context.Empleados
                .FromSqlRaw("EXEC SP_CREAR_EMPLEADO @CODIGO_EMPLEADO = {0}, @CEDULA = {1}, @P_NOMBRE = {2}, @S_NOMBRE = {3}, @P_APELLIDO = {4}, @S_APELLIDO = {5}, @DIRECCION = {6}, @ID_SECTOR = {7}, @ID_CIUDAD = {8}, @ID_PROVINCIA = {9}, @PAIS_NACIMIENTO = {10}, @TELEFONO_PRINCIPAL = {11}, @TELEFONO_SECUNDARIO = {12}, @FECHA_NACIMIENTO = {13}, @EMAIL = {14}, @SEXO = {15}, @ESTADO_CIVIL = {16}, @FECHA_CREACION = {17}, @FRECUENCIA_COBRO = {18}, @CUENTA_BANCO = {19}, @ID_ENTIDAD_BANCARIA = {20}, @ID_PUESTO = {21}, @ID_DEPARTAMENTO = {22}, @TIPO_SANGRE = {23}, @NOMBRE_FAMILIAR_PRIMARIO = {24}, @TELEFONO_FAMILIAR_PRIMARIO = {25}, @PARENTESCO_FAMILIAR_PRIMARIO = {26}, @NOMBRE_FAMILIAR_SECUNDARIO = {27}, @TELEFONO_FAMILIAR_SECUNDARIO = {28}, @PARENTESCO_FAMILIAR_SECUNDARIO = {29}, @ID_NIVEL_APROBACION = {30}, @ESTATUS = {31}, @CREADO_POR = {32}", empleado.CODIGO_EMPLEADO, empleado.CEDULA, empleado.P_NOMBRE, empleado.S_NOMBRE, empleado.P_APELLIDO, empleado.S_APELLIDO, empleado.DIRECCION, empleado.ID_SECTOR, empleado.ID_CIUDAD, empleado.ID_PROVINCIA, empleado.PAIS_NACIMIENTO, empleado.TELEFONO_PRINCIPAL, empleado.TELEFONO_SECUNDARIO, empleado.FECHA_NACIMIENTO, empleado.EMAIL, empleado.SEXO, empleado.ESTADO_CIVIL, empleado.FECHA_CREACION, empleado.FRECUENCIA_COBRO, empleado.CUENTA_BANCO, empleado.ID_ENTIDAD_BANCARIA, empleado.ID_PUESTO, empleado.ID_DEPARTAMENTO, empleado.TIPO_SANGRE, empleado.NOMBRE_FAMILIAR_PRIMARIO, empleado.TELEFONO_FAMILIAR_PRIMARIO, empleado.PARENTESCO_FAMILIAR_PRIMARIO, empleado.NOMBRE_FAMILIAR_SECUNDARIO, empleado.TELEFONO_FAMILIAR_SECUNDARIO, empleado.PARENTESCO_FAMILIAR_SECUNDARIO, empleado.ID_NIVEL_APROBACION, empleado.ESTATUS, empleado.CREADO_POR);
            }
        
            return RedirectToAction("RegistroEmpleados");
        
        }


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