using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;
using Microsoft.AspNetCore.Http;

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
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var menu = new Menu
        {
            Items = new List<Item> {
                new Item {
                    Titulo = "Gestion de Prestamos",
                    Icono = "fi fi-tr-handshake-deal-loan",
                    ModuloPermiso = "Prestamos",
                    SubItems = new List<SubItem> {
                        new SubItem { Titulo = "Crear solicitud de prestamo", Url = Url.Action("CrearSolicitudPrestamo", "Prestamos"), Permiso = "RegistrarSolicitudPrestamo" },
                        new SubItem { Titulo = "Consultar solicitudes de prestamo", Url = Url.Action("RegistroSolicitudPrestamos", "Prestamos"), Permiso = "ConsultarSolicitudesPrestamos" },
                        new SubItem { Titulo = "Aprobacion de prestamos", Url = Url.Action("AprobacionPrestamos", "Prestamos"), Permiso = "AprobacionSolicitudPrestamos" },
                        new SubItem { Titulo = "Consultar prestamos", Url = Url.Action("RegistroPrestamos", "Prestamos"), Permiso = "ConsultarPrestamos" },
                        new SubItem { Titulo = "Registrar categoria de prestamo", Url = Url.Action("Crear", "CategoriasPrestamo"), Permiso = "RegistrarCategoriaPrestamo"    },
                        new SubItem { Titulo = "Consultar categorias de prestamo", Url = Url.Action("RegistroCategoriasPrestamo", "CategoriaPrestamo"), Permiso = "ConsultarCategoriasPrestamos"   },
                        new SubItem { Titulo = "Registrar nivel de aprobacion", Url = Url.Action("Crear", "NivelesAprobacion"), Permiso = "RegistrarNivelAprobacion"},
                        new SubItem { Titulo = "Consultar niveles de aprobacion", Url = Url.Action("RegistroNivelesAprobacion", "NivelesAprobacion"), Permiso = "ConsultarNivelesAprobacion"},
                        new SubItem { Titulo = "Registrar anexo", Url = Url.Action("Crear", "Anexos"), Permiso = "RegistrarAnexo" },
                        new SubItem { Titulo = "Consultar anexos", Url = Url.Action("RegistroAnexos", "Anexos"), Permiso = "ConsultarAnexos" }                    }
                },
                new Item {
                    Titulo = "Gestion de pagos",
                    Icono = "fi fi-tr-sack-dollar",
                    ModuloPermiso = "Pagos",
                    SubItems = new List<SubItem> {
                        new SubItem { Titulo = "Registrar pago", Url = Url.Action("Crear", "Pagos"), Permiso = "RegistrarPago" },
                        new SubItem { Titulo = "Consultar pagos", Url = Url.Action("RegistroPagos", "Pagos"), Permiso = "ConsultarPagos" },
                    }
                },
                new Item {
                    Titulo = "Gestion de socios",
                    Icono = "fi fi-tr-customer-care",
                    ModuloPermiso = "Socios",
                    SubItems = new List<SubItem> {
                        new SubItem { Titulo = "Registrar socio", Url = Url.Action("Crear", "Socios"), Permiso = "RegistrarSocio" },
                        new SubItem { Titulo = "Consultar socios", Url = Url.Action("RegistroSocios", "Socios"), Permiso = "ConsultarSocios" }
                    }
                },
                new Item {
                    Titulo = "Gestion de empleados",
                    Icono = "fi fi-tr-employee-man",
                    ModuloPermiso = "Empleados",
                    SubItems = new List<SubItem> {
                        new SubItem { Titulo = "Registrar empleado", Url = Url.Action("Crear", "Empleados"), Permiso = "RegistrarEmpleado" },
                        new SubItem { Titulo = "Consultar empledos", Url = Url.Action("RegistroEmpleados", "Empleados"), Permiso = "ConsultarEmpleados" },
                        new SubItem { Titulo = "Registrar puesto", Url = Url.Action("Crear", "Puestos"), Permiso = "RegistrarPuesto" },
                        new SubItem { Titulo = "Consultar puestos", Url = Url.Action("RegistroPuestos", "Puestos"), Permiso = "ConsultarPuestos" },
                        new SubItem { Titulo = "Registrar departamento", Url = Url.Action("Crear", "Departamentos"), Permiso = "RegistrarDepartamento" },
                        new SubItem { Titulo = "Consultar departamentos", Url = Url.Action("RegistroDepartamentos", "Departamentos"), Permiso = "ConsultarDepartamentos" },
                        new SubItem { Titulo = "Registrar entidad bancaria", Url = Url.Action("Crear", "EntidadesBancarias"), Permiso = "RegistrarEntidadBancaria" },
                        new SubItem { Titulo = "Consultar entidades bancarias", Url = Url.Action("RegistroEntidadBancaria", "EntidadesBancarias"), Permiso = "ConsultarEntidadesBancarias" }
                    }
                },
                new Item {
                    Titulo = "Gestion de codeudores",
                    Icono = "fi fi-tr-user-salary",
                    ModuloPermiso = "Codeudores",
                    SubItems = new List<SubItem> {
                        new SubItem { Titulo = "Registrar codeudor", Url = Url.Action("Crear", "Codeudores"), Permiso = "RegistrarCodeudor" },
                        new SubItem { Titulo = "Consultar codeudores", Url = Url.Action("RegistroCodeudores", "Codeudores"), Permiso = "ConsultarCodeudores" },
                    }

                },
                new Item {
                    Titulo = "Configuracion",
                    Icono = "fi fi-tc-settings",
                    ModuloPermiso = "Configuracion",
                    SubItems = new List<SubItem> {
                        new SubItem { Titulo = "Usuarios", Url = Url.Action("Usuarios", "Configuracion"), Permiso = "ConfigurarUsuarios" },
                        new SubItem { Titulo = "Roles", Url = Url.Action("Roles", "Configuracion"), Permiso = "ConfigurarRoles" },
                        new SubItem { Titulo = "Permisos", Url = Url.Action("Permisos", "Configuracion"), Permiso = "ConfigurarPermisos" }
                    }
                }
            }
        };

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
