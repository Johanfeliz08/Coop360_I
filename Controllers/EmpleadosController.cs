using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Microsoft.EntityFrameworkCore;
using Coop360_I.Data;
using Coop360_I.Controllers;

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

    private Empleado validarEmpleado(Empleado empleado){

        // Verifica el objeto recibido desde el formulario no este vacio
        if (empleado == null) { 
            throw new Exception("El servidor no puede procesar la solicitud, objeto empleado vacio");
        };
    
        // Valida que los campos obligatorios no esten vacios

        if (empleado.CEDULA == null || empleado.P_NOMBRE == null || empleado.P_APELLIDO == null || empleado.DIRECCION == null || empleado.ID_SECTOR == null || empleado.ID_CIUDAD == null || empleado.ID_PROVINCIA == null || empleado.PAIS_NACIMIENTO == null || empleado.TELEFONO_PRINCIPAL == null  || empleado.SEXO == null || empleado.ESTADO_CIVIL == null || empleado.FRECUENCIA_COBRO == null || empleado.CUENTA_BANCO == "" || empleado.ID_ENTIDAD_BANCARIA == null || empleado.ID_PUESTO == null || empleado.ID_DEPARTAMENTO == null || empleado.TIPO_SANGRE == null || empleado.NOMBRE_FAMILIAR_PRIMARIO == null || empleado.TELEFONO_FAMILIAR_PRIMARIO == null || empleado.PARENTESCO_FAMILIAR_PRIMARIO == null || empleado.ID_NIVEL_APROBACION == null || empleado.ESTATUS == null || empleado.CREADO_POR == null) {
            throw new Exception("El servidor no puede procesar la solicitud, campos obligatorios vacios");
        };

        // Validacion y conversion de datos
        var empleadoValido = new Empleado
        {
            CEDULA = empleado.CEDULA,
            P_NOMBRE = empleado.P_NOMBRE,
            S_NOMBRE = empleado.S_NOMBRE ?? "",
            P_APELLIDO = empleado.P_APELLIDO,
            S_APELLIDO = empleado.S_APELLIDO ?? "",
            DIRECCION = empleado.DIRECCION,
            ID_SECTOR = Convert.ToInt32(empleado.ID_SECTOR ?? 0),
            ID_CIUDAD = Convert.ToInt32(empleado.ID_CIUDAD ?? 0),
            ID_PROVINCIA = Convert.ToInt32(empleado.ID_PROVINCIA ?? 0),
            PAIS_NACIMIENTO = empleado.PAIS_NACIMIENTO,
            TELEFONO_PRINCIPAL = empleado.TELEFONO_PRINCIPAL,
            TELEFONO_SECUNDARIO = empleado.TELEFONO_SECUNDARIO ?? "",
            FECHA_NACIMIENTO = Convert.ToDateTime(empleado.FECHA_NACIMIENTO),
            EMAIL = empleado.EMAIL ?? "",
            SEXO = empleado.SEXO,
            ESTADO_CIVIL = empleado.ESTADO_CIVIL,
            FRECUENCIA_COBRO = empleado.FRECUENCIA_COBRO,
            CUENTA_BANCO = empleado.CUENTA_BANCO,
            ID_ENTIDAD_BANCARIA = Convert.ToInt32(empleado.ID_ENTIDAD_BANCARIA ?? 0),
            ID_PUESTO = Convert.ToInt32(empleado.ID_PUESTO ?? 0),
            ID_DEPARTAMENTO = Convert.ToInt32(empleado.ID_DEPARTAMENTO ?? 0),
            TIPO_SANGRE = empleado.TIPO_SANGRE,
            NOMBRE_FAMILIAR_PRIMARIO = empleado.NOMBRE_FAMILIAR_PRIMARIO ?? "",
            TELEFONO_FAMILIAR_PRIMARIO = empleado.TELEFONO_FAMILIAR_PRIMARIO ?? "",
            PARENTESCO_FAMILIAR_PRIMARIO = empleado.PARENTESCO_FAMILIAR_PRIMARIO ?? "",
            NOMBRE_FAMILIAR_SECUNDARIO = empleado.NOMBRE_FAMILIAR_SECUNDARIO ?? "",
            TELEFONO_FAMILIAR_SECUNDARIO = empleado.TELEFONO_FAMILIAR_SECUNDARIO ?? "",
            PARENTESCO_FAMILIAR_SECUNDARIO = empleado.PARENTESCO_FAMILIAR_SECUNDARIO ?? "",
            ID_NIVEL_APROBACION = empleado.ID_NIVEL_APROBACION ?? "",
            ESTATUS = empleado.ESTATUS,
            CREADO_POR = Convert.ToInt32(empleado.CREADO_POR)
        };

        return empleadoValido;
    }

    public IActionResult Crear () {

        // var regiones = _context.Regiones
        // .FromSqlRaw("EXEC SP_LEER_REGIONES")
        // .AsEnumerable()
        // .ToList();

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
            // Regiones = regiones,
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
    public async Task<IActionResult> GuardarAsync(Empleado empleado) {

        // Obtiene el CODIGO_EMPLEADO del usuario logeado y redirige al login si no hay un usuario logeado
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null) {
                return RedirectToAction("Login", "Auth");
        } else {
                // Asigna el codigo del empleado que esta creando el registro
                empleado.CREADO_POR = HttpContext.Session.GetInt32("ID_USUARIO");
        };

        // Valida el empleado y devuelve un objeto con los campos validados
        var empleadoValido = validarEmpleado(empleado);

        try {
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC SP_CREAR_EMPLEADO @CEDULA = {0}, @P_NOMBRE = {1}, @S_NOMBRE = {2}, @P_APELLIDO = {3}, @S_APELLIDO = {4}, @DIRECCION = {5}, @ID_SECTOR = {6}, @ID_CIUDAD = {7}, @ID_PROVINCIA = {8}, @PAIS_NACIMIENTO = {9}, @TELEFONO_PRINCIPAL = {10}, @TELEFONO_SECUNDARIO = {11}, @FECHA_NACIMIENTO = {12}, @EMAIL = {13}, @SEXO = {14}, @ESTADO_CIVIL = {15}, @FRECUENCIA_COBRO = {16}, @CUENTA_BANCO = {17}, @ID_ENTIDAD_BANCARIA = {18}, @ID_PUESTO = {19}, @ID_DEPARTAMENTO = {20}, @TIPO_SANGRE = {21}, @NOMBRE_FAMILIAR_PRIMARIO = {22}, @TELEFONO_FAMILIAR_PRIMARIO = {23}, @PARENTESCO_FAMILIAR_PRIMARIO = {24}, @NOMBRE_FAMILIAR_SECUNDARIO = {25}, @TELEFONO_FAMILIAR_SECUNDARIO = {26}, @PARENTESCO_FAMILIAR_SECUNDARIO = {27}, @ID_NIVEL_APROBACION = {28}, @ESTATUS = {29}, @CREADO_POR = {30}",
            empleadoValido.CEDULA, empleadoValido.P_NOMBRE, empleadoValido.S_NOMBRE ?? "", empleadoValido.P_APELLIDO, empleadoValido.S_APELLIDO ?? "",
            empleadoValido.DIRECCION, empleadoValido.ID_SECTOR ?? 0, empleadoValido.ID_CIUDAD ?? 0, empleadoValido.ID_PROVINCIA ?? 0, empleadoValido.PAIS_NACIMIENTO,
            empleadoValido.TELEFONO_PRINCIPAL, empleadoValido.TELEFONO_SECUNDARIO ?? "", empleadoValido.FECHA_NACIMIENTO, empleadoValido.EMAIL ?? "",
            empleadoValido.SEXO, empleadoValido.ESTADO_CIVIL, empleadoValido.FRECUENCIA_COBRO, empleadoValido.CUENTA_BANCO,
            empleadoValido.ID_ENTIDAD_BANCARIA ?? 0, empleadoValido.ID_PUESTO ?? 0, empleadoValido.ID_DEPARTAMENTO ?? 0, empleadoValido.TIPO_SANGRE,
            empleadoValido.NOMBRE_FAMILIAR_PRIMARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_PRIMARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_PRIMARIO ?? "",
            empleadoValido.NOMBRE_FAMILIAR_SECUNDARIO ?? "", empleadoValido.TELEFONO_FAMILIAR_SECUNDARIO ?? "", empleadoValido.PARENTESCO_FAMILIAR_SECUNDARIO ?? "",
            empleadoValido.ID_NIVEL_APROBACION, empleadoValido.ESTATUS, empleadoValido.CREADO_POR ?? 0
        );
        } catch (Exception ex) {
            throw new Exception("Error al guardar el registro: " + ex.Message + ex.Source);
        }

        ViewBag.Message = "Registro guardado con exito";
        return RedirectToAction("RegistroEmpleados");
        
        
        }
}