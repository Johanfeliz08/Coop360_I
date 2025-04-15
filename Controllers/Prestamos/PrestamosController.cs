using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Coop360_I.Models;
using Coop360_I.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

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


    // Solicitudes de prestamos
    private SolicitudPrestamo validarSolicitudPrestamo(SolicitudPrestamo solicitudPrestamo)
    {


        if (solicitudPrestamo.CODIGO_SOCIO <= 0)
        {
            throw new Exception("El código del socio es requerido");
        }


        if (solicitudPrestamo.MONTO_SOLICITADO <= 0)
        {
            throw new Exception("El monto solicitado es requerido");
        }

        if (solicitudPrestamo.PLAZO_MESES <= 0)
        {
            throw new Exception("El plazo en meses es requerido");
        }

        if (solicitudPrestamo.CANTIDAD_CUOTAS <= 0)
        {
            throw new Exception("La cantidad de cuotas es requerida");
        }

        if (solicitudPrestamo.TASA_INTERES <= 0)
        {
            throw new Exception("La tasa de interés es requerida");
        }

        if (solicitudPrestamo.ID_CATEGORIA_PRESTAMO <= 0)
        {
            throw new Exception("La categoría del préstamo es requerida");
        }

        if (solicitudPrestamo.ID_NIVEL_APROBACION_REQUERIDO <= 0)
        {
            throw new Exception("El nivel de aprobación requerido es requerido");
        }

        if (solicitudPrestamo.ESTATUS == null || solicitudPrestamo.ESTATUS == "")
        {
            throw new Exception("El estatus es requerido");
        }

        return solicitudPrestamo;
    }

    [HttpGet]
    public IActionResult SolicitudesPrestamos()
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var solicitudesPrestamos = _context.SolicitudPrestamos
                .FromSqlRaw("EXEC SP_LEER_SOLICITUDES_PRESTAMOS")
                .AsEnumerable()
                .ToList();

            var idRol = HttpContext.Session.GetInt32("ID_ROL");
            var idNivelAprobacion = HttpContext.Session.GetInt32("ID_NIVEL_APROBACION");
            var codigoEmpleado = HttpContext.Session.GetInt32("CODIGO_EMPLEADO");

            if (idRol != 6)
            {
                // Filtra las solicitudes solo las creadas por el usuario/empleado logeado
                var solicitudesPrestamosFiltradas = solicitudesPrestamos.Where(solicitud => solicitud.SOLICITUD_REALIZADA_POR == codigoEmpleado && solicitud.ESTATUS == "Pendiente").ToList();
                return View("~/Views/Prestamos/SolicitudesPrestamos/RegistroSolicitudesPrestamos.cshtml", solicitudesPrestamosFiltradas);
            }


            return View("~/Views/Prestamos/SolicitudesPrestamos/RegistroSolicitudesPrestamos.cshtml", solicitudesPrestamos);


        }
        catch (Exception ex)
        {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al obtener las solicitudes de préstamos:";
            Console.WriteLine("Error al obtener las solicitudes de préstamos: " + ex.Message + ex.Source); // Mensaje para el log en el server

            var emptySolicitudesPrestamos = new List<SolicitudPrestamo>();

            return View("~/Views/Prestamos/SolicitudesPrestamos/RegistroSolicitudesPrestamos.cshtml", emptySolicitudesPrestamos);
        }

    }


    [HttpGet]
    public JsonResult obtenerCategoriaPrestamo(int idCategoriaPrestamo)
    {

        try
        {
            var categoriaPrestamo = _context.CategoriaPrestamo
                .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
                .AsEnumerable()
                .ToList();


            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper,
                WriteIndented = true
            };

            // var json = JsonSerializer.Serialize(categoriaPrestamo, serializeOptions);
            return new JsonResult(categoriaPrestamo, serializeOptions);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }

    }

    [HttpGet]
    public IActionResult CrearSolicitudPrestamo()
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // Obtener datos necesarios para el formulario

        var socios = _context.Socios
            .FromSqlRaw("EXEC SP_LEER_SOCIOS")
            .AsEnumerable()
            .ToList();

        var categoriasPrestamo = _context.CategoriaPrestamo
            .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
            .AsEnumerable()
            .ToList();

        var codeudores = _context.Codeudores
            .FromSqlRaw("EXEC SP_LEER_CODEUDORES")
            .AsEnumerable()
            .ToList();


        var viewModel = new SolicitudPrestamoViewModel
        {
            Socios = socios,
            CategoriasPrestamos = categoriasPrestamo,
            Codeudores = codeudores
        };


        ViewBag.Title = "Crear Solicitud de Prestamo";
        return View("~/Views/Prestamos/SolicitudesPrestamos/FormSolicitudPrestamo.cshtml", viewModel);
    }

    [HttpGet]
    public IActionResult EditarSolicitudPrestamo(string IdSolicitudPrestamo)
    {

        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (IdSolicitudPrestamo != null)
        {

            var ID_SOLICITUD = Convert.ToInt32(IdSolicitudPrestamo);
            // Obtener datos necesarios para el formulario

            var socios = _context.Socios
                .FromSqlRaw("EXEC SP_LEER_SOCIOS")
                .AsEnumerable()
                .ToList();

            var categoriasPrestamo = _context.CategoriaPrestamo
                .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
                .AsEnumerable()
                .ToList();

            var codeudores = _context.Codeudores
                .FromSqlRaw("EXEC SP_LEER_CODEUDORES")
                .AsEnumerable()
                .ToList();

            var solicitudPrestamo = _context.SolicitudPrestamos
                .FromSqlRaw("EXEC SP_BUSCAR_SOLICITUD_PRESTAMO @ID_SOLICITUD = {0}", ID_SOLICITUD)
                .AsEnumerable()
                .FirstOrDefault();

            var detallesAnexos = _context.DetalleAnexos
            .FromSqlRaw("EXEC SP_OBTENER_DETALLES_ANEXOS_SOLICITUD_PRESTAMO @ID_SOLICITUD_PRESTAMO = {0}", ID_SOLICITUD)
            .AsEnumerable()
            .ToList();


            var viewModel = new SolicitudPrestamoViewModel
            {
                SolicitudPrestamo = solicitudPrestamo,
                Socios = socios,
                CategoriasPrestamos = categoriasPrestamo,
                Codeudores = codeudores,
                DetallesAnexos = detallesAnexos
            };

            ViewBag.Title = "Editar Solicitud de Prestamo";
            return View("~/Views/Prestamos/SolicitudesPrestamos/FormSolicitudPrestamo.cshtml", viewModel);

        }


        TempData["openModal"] = true;
        TempData["Error"] = "No se encontró la solicitud de préstamo";
        return RedirectToAction("SolicitudesPrestamos", "Prestamos");
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarSolicitudPrestamo(SolicitudPrestamo solicitudPrestamo, List<string> idDetalleAnexosArchivos, List<IFormFile> detallesAnexosArchivos)
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (solicitudPrestamo != null) // Valido que la solicitud no sea nula/vacia
        {
            var nivelesAprobacion = _context.NivelesAprobacion // Obtengo los diferentes niveles de aprobacion
                            .FromSqlRaw("EXEC SP_LEER_NIVELES_APROBACION")
                            .AsEnumerable()
                            .ToList();

            if (nivelesAprobacion.Count > 0) // Valido que existan niveles de aprobacion
            {

                // Itera sobre los niveles de aprobacion y asigna el nivel si el monto solicitado se encuentra dentro del rango
                foreach (var nivel in nivelesAprobacion)
                {
                    if (solicitudPrestamo.MONTO_SOLICITADO >= nivel.MONTO_DESDE && solicitudPrestamo.MONTO_SOLICITADO <= nivel.MONTO_HASTA)
                    {
                        solicitudPrestamo.ID_NIVEL_APROBACION_REQUERIDO = Convert.ToInt32(nivel.ID_NIVEL_APROBACION);
                    }
                }

            }
            else
            {
                TempData["openModal"] = true;
                TempData["Error"] = "No hay un nivel de aprobación definido para la solicitud de préstamo";
                return RedirectToAction("SolicitudesPrestamos", "Prestamos");
            }

            solicitudPrestamo.ESTATUS = solicitudPrestamo.ESTATUS == "Anulada" ? "Anulada" : "Pendiente"; // Valido si el estatus cambio a anulada, si es asi, actualizo el estatus a anulado, sino lo mantengo como pendiente

            var solicitudPrestamoValida = validarSolicitudPrestamo(solicitudPrestamo); // Valido la solicitud de manera general

            // Obtengo los detalles de los anexos de la solicitud de prestamo

            var dataDetallesAnexos = _context.DetalleAnexos
                .FromSqlRaw("EXEC SP_OBTENER_DETALLES_ANEXOS_SOLICITUD_PRESTAMO @ID_SOLICITUD_PRESTAMO = {0}", solicitudPrestamoValida.ID_SOLICITUD)
                .AsEnumerable()
                .ToList();

            // Valido que tanto los id de los detalles de los anexos como los archivos existan
            if (idDetalleAnexosArchivos.Count > 0 && idDetalleAnexosArchivos != null && detallesAnexosArchivos.Count > 0 && detallesAnexosArchivos != null)
            {

                // Itera sobre los archivos y guarda los archivos en el servidor
                foreach (var archivo in detallesAnexosArchivos)
                {

                    var archivoAnexo = new DetalleAnexo // Inicializo el objeto para guardar el archivo
                    {
                        ID_DETALLE_ANEXO = 0,
                        NOMBRE_ANEXO = "",
                        VALOR = "",
                        TIPO = "",
                        ID_ANEXO = 0,
                        ID_SOLICITUD_PRESTAMO = 0
                    };

                    foreach (var idDetalleAnexoArchivo in idDetalleAnexosArchivos) // Itera sobre los id de los detalles de los anexos
                    {

                        Console.WriteLine("ID Detalle Anexo Archivo: " + idDetalleAnexoArchivo);
                        archivoAnexo.ID_DETALLE_ANEXO = Convert.ToInt32(idDetalleAnexoArchivo); // Obtengo el id del detalle del anexo
                        archivoAnexo.NOMBRE_ANEXO = dataDetallesAnexos.Where(detalle => detalle.ID_DETALLE_ANEXO == Convert.ToInt32(idDetalleAnexoArchivo)).FirstOrDefault()?.NOMBRE_ANEXO; // Obtengo el nombre del anexo
                        archivoAnexo.VALOR = ""; // Inicializo el valor del anexo
                        archivoAnexo.TIPO = "Archivo"; // Asigno el tipo de anexo
                        archivoAnexo.ID_ANEXO = Convert.ToInt32(dataDetallesAnexos.Where(detalle => detalle.ID_DETALLE_ANEXO == Convert.ToInt32(idDetalleAnexoArchivo)).FirstOrDefault()?.ID_ANEXO); // Obtengo el id del anexo
                        archivoAnexo.ID_SOLICITUD_PRESTAMO = Convert.ToInt32(solicitudPrestamoValida.ID_SOLICITUD); // Obtengo el id de la solicitud de prestamo

                    }
                    ;

                    var nombreUnico = ""; // Inicializo el nombre unico
                    if (string.IsNullOrEmpty(archivoAnexo.NOMBRE_ANEXO)) // Si el nombre del anexo es nulo, genero un nombre unico
                    {
                        nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName); // Genero un nombre unico
                    }
                    else // Si el nombre del anexo no es nulo, genero un nombre unico basado en el nombre del anexo
                    {
                        nombreUnico = Guid.NewGuid().ToString() + "_" + Regex.Replace(archivoAnexo.NOMBRE_ANEXO, @"\s+", "") + "_" + archivoAnexo.ID_SOLICITUD_PRESTAMO.ToString() + Path.GetExtension(archivo.FileName); // Genero un nombre unico
                    }

                    var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Archivos", nombreUnico); // Obtengo la ruta del archivo
                    archivoAnexo.VALOR = nombreUnico; // Asigno la ruta del archivo al objeto
                    await using var stream = new FileStream(ruta, FileMode.Create); // Creo un stream para guardar el archivo
                    await archivo.CopyToAsync(stream); // Copio el archivo al stream
                    // Actualizo los detalles de los anexos
                    await _context.Database.ExecuteSqlRawAsync("EXEC SP_ACTUALIZAR_DETALLE_ANEXO @ID_DETALLE_ANEXO = {0}, @VALOR = {1}", archivoAnexo.ID_DETALLE_ANEXO, archivoAnexo.VALOR);

                }

            }

            // Luego de actualizar los anexos, actualizo la solicitud de prestamo
            await _context.Database.ExecuteSqlRawAsync("EXEC SP_ACTUALIZAR_SOLICITUD_PRESTAMO @ID_SOLICITUD = {0}, @CODIGO_SOCIO = {1}, @MONTO_SOLICITADO = {2}, @PLAZO_MESES = {3}, @CANTIDAD_CUOTAS = {4}, @MONTO_POR_CUOTA = {5}, @TASA_INTERES = {6}, @ID_CATEGORIA_PRESTAMO = {7}, @ID_NIVEL_APROBACION_REQUERIDO = {8}, @CODIGO_CODEUDOR_PRINCIPAL = {9}, @CODIGO_CODEUDOR_SECUNDARIO = {10}, @CODIGO_CODEUDOR_TERCIARIO = {11}, @ESTATUS = {12}", solicitudPrestamoValida.ID_SOLICITUD, solicitudPrestamoValida.CODIGO_SOCIO, solicitudPrestamoValida.MONTO_SOLICITADO, solicitudPrestamoValida.PLAZO_MESES, solicitudPrestamoValida.CANTIDAD_CUOTAS, solicitudPrestamoValida.MONTO_POR_CUOTA, solicitudPrestamoValida.TASA_INTERES, solicitudPrestamoValida.ID_CATEGORIA_PRESTAMO, solicitudPrestamoValida.ID_NIVEL_APROBACION_REQUERIDO, solicitudPrestamoValida.CODIGO_CODEUDOR_PRINCIPAL, solicitudPrestamoValida.CODIGO_CODEUDOR_SECUNDARIO, solicitudPrestamoValida.CODIGO_CODEUDOR_TERCIARIO, solicitudPrestamoValida.ESTATUS);

            TempData["openModal"] = true;
            TempData["Success"] = "Solicitud de préstamo actualizada correctamente";
            return RedirectToAction("SolicitudesPrestamos", "Prestamos");

        }

        TempData["openModal"] = true;
        TempData["Error"] = "Ha ocurrido un error al actualizar la solicitud de préstamo";
        return RedirectToAction("SolicitudesPrestamos", "Prestamos");
    }

    [HttpGet]
    public IActionResult FormAnexosSolicitudPrestamo(string IdSolicitudPrestamo)
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (IdSolicitudPrestamo != null)
        {

            var ID_SOLICITUD = Convert.ToInt32(IdSolicitudPrestamo);

            var detallesAnexos = _context.DetalleAnexos
                .FromSqlRaw("EXEC SP_OBTENER_DETALLES_ANEXOS_SOLICITUD_PRESTAMO @ID_SOLICITUD_PRESTAMO = {0}", ID_SOLICITUD)
                .AsEnumerable()
                .ToList();

            var viewModel = new FormAnexoSolicitudPrestamoViewModel
            {
                ID_SOLICITUD_PRESTAMO = ID_SOLICITUD,
                DetallesAnexos = detallesAnexos
            };

            return View("~/Views/Prestamos/SolicitudesPrestamos/FormAnexosSolicitudPrestamo.cshtml", viewModel);
        }


        TempData["openModal"] = true;
        TempData["Error"] = "No se encontró la solicitud de préstamo, y no se puede continuar con el formulario de anexos";
        return RedirectToAction("SolicitudesPrestamos", "Prestamos");
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarAnexosSolicitudPrestamo(string IdSolicitudPrestamo, List<string> idDetalleAnexosArchivos, List<IFormFile> detallesAnexosArchivos)
    {

        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (IdSolicitudPrestamo != null)
        {
            var ID_SOLICITUD = Convert.ToInt32(IdSolicitudPrestamo);

            var dataDetallesAnexos = _context.DetalleAnexos
                .FromSqlRaw("EXEC SP_OBTENER_DETALLES_ANEXOS_SOLICITUD_PRESTAMO @ID_SOLICITUD_PRESTAMO = {0}", ID_SOLICITUD)
                .AsEnumerable()
                .ToList();

            try
            {
                // Valido que tanto los id de los detalles de los anexos como los archivos existan
                if (idDetalleAnexosArchivos.Count > 0 && idDetalleAnexosArchivos != null && detallesAnexosArchivos.Count > 0 && detallesAnexosArchivos != null)
                {

                    // Itera sobre los archivos y guarda los archivos en el servidor
                    foreach (var archivo in detallesAnexosArchivos)
                    {


                        var archivoAnexo = new DetalleAnexo // Inicializo el objeto para guardar el archivo
                        {
                            ID_DETALLE_ANEXO = 0,
                            NOMBRE_ANEXO = "",
                            VALOR = "",
                            TIPO = "",
                            ID_ANEXO = 0,
                            ID_SOLICITUD_PRESTAMO = 0
                        };

                        foreach (var idDetalleAnexoArchivo in idDetalleAnexosArchivos) // Itera sobre los id de los detalles de los anexos
                        {

                            Console.WriteLine("Entrando al bucle de idDetalleAnexoArchivo");
                            Console.WriteLine($"ID Detalle Anexo Archivo: {idDetalleAnexoArchivo}");
                            archivoAnexo.ID_DETALLE_ANEXO = Convert.ToInt32(idDetalleAnexoArchivo); // Obtengo el id del detalle del anexo
                            archivoAnexo.NOMBRE_ANEXO = dataDetallesAnexos.Where(detalle => detalle.ID_DETALLE_ANEXO == Convert.ToInt32(idDetalleAnexoArchivo)).FirstOrDefault()?.NOMBRE_ANEXO; // Obtengo el nombre del anexo
                            archivoAnexo.VALOR = ""; // Inicializo el valor del anexo
                            archivoAnexo.TIPO = "Archivo"; // Asigno el tipo de anexo
                            archivoAnexo.ID_ANEXO = Convert.ToInt32(dataDetallesAnexos.Where(detalle => detalle.ID_DETALLE_ANEXO == Convert.ToInt32(idDetalleAnexoArchivo)).FirstOrDefault()?.ID_ANEXO); // Obtengo el id del anexo
                            archivoAnexo.ID_SOLICITUD_PRESTAMO = Convert.ToInt32(ID_SOLICITUD); // Obtengo el id de la solicitud de prestamo

                        }
                        ;

                        if (archivoAnexo.ID_DETALLE_ANEXO == 0 || archivoAnexo.NOMBRE_ANEXO == null || archivoAnexo.VALOR == null || archivoAnexo.TIPO == null || archivoAnexo.ID_ANEXO == 0 || archivoAnexo.ID_SOLICITUD_PRESTAMO == 0)
                        {
                            throw new Exception("Ha ocurrido un error al actualizar los anexos de la solicitud de préstamo");
                        }


                        var nombreUnico = ""; // Inicializo el nombre unico
                        if (string.IsNullOrEmpty(archivoAnexo.NOMBRE_ANEXO)) // Si el nombre del anexo es nulo, genero un nombre unico
                        {
                            nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName); // Genero un nombre unico
                        }
                        else // Si el nombre del anexo no es nulo, genero un nombre unico basado en el nombre del anexo
                        {
                            nombreUnico = Guid.NewGuid().ToString() + "_" + Regex.Replace(archivoAnexo.NOMBRE_ANEXO, @"\s+", "") + "_" + archivoAnexo.ID_SOLICITUD_PRESTAMO.ToString() + Path.GetExtension(archivo.FileName); // Genero un nombre unico
                        }

                        var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Archivos", nombreUnico); // Obtengo la ruta del archivo
                        archivoAnexo.VALOR = nombreUnico; // Asigno la ruta del archivo al objeto
                        await using var stream = new FileStream(ruta, FileMode.Create); // Creo un stream para guardar el archivo
                        await archivo.CopyToAsync(stream); // Copio el archivo al stream
                        // Actualizo los detalles de los anexos
                        await _context.Database.ExecuteSqlRawAsync("EXEC SP_ACTUALIZAR_DETALLE_ANEXO @ID_DETALLE_ANEXO = {0}, @VALOR = {1}", archivoAnexo.ID_DETALLE_ANEXO, archivoAnexo.VALOR);

                    }
                }

                TempData["openModal"] = true;
                TempData["Success"] = "Solicitud de prestamo creada correctamente.";
                return RedirectToAction("SolicitudesPrestamos", "Prestamos");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar los anexos: " + ex.Message + ex.Source); // Mensaje para el log en el server
            }
        }

        TempData["openModal"] = true;
        TempData["Error"] = "La solicitud se ha creado correctamente, pero no se han podido actualizar los anexos";
        return RedirectToAction("SolicitudesPrestamos", "Prestamos");

    }


    [HttpPost]
    public async Task<IActionResult> GuardarSolicitudPrestamoAsync(SolicitudPrestamo solicitudPrestamo)
    {

        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (solicitudPrestamo != null)
        {

            // Obtener codigo empleado del usuario logeado y asignarlo al objeto
            solicitudPrestamo.SOLICITUD_REALIZADA_POR = Convert.ToInt32(HttpContext.Session.GetInt32("ID_USUARIO")); ;

            var nivelesAprobacion = _context.NivelesAprobacion
                .FromSqlRaw("EXEC SP_LEER_NIVELES_APROBACION")
                .AsEnumerable()
                .ToList();

            if (nivelesAprobacion.Count > 0)
            {

                // Itera sobre los niveles de aprobacion y asigna el nivel si el monto solicitado se encuentra dentro del rango
                foreach (var nivel in nivelesAprobacion)
                {
                    if (solicitudPrestamo.MONTO_SOLICITADO >= nivel.MONTO_DESDE && solicitudPrestamo.MONTO_SOLICITADO <= nivel.MONTO_HASTA)
                    {
                        solicitudPrestamo.ID_NIVEL_APROBACION_REQUERIDO = Convert.ToInt32(nivel.ID_NIVEL_APROBACION);
                    }
                }

            }
            else
            {
                TempData["openModal"] = true;
                TempData["Error"] = "No hay un nivel de aprobación definido para la solicitud de préstamo";
                return RedirectToAction("SolicitudesPrestamos", "Prestamos");
            }
            // Asignar estatus Pendiente por defecto
            solicitudPrestamo.ESTATUS = "Pendiente";

            // Validamos la solicitud
            var solicitudPrestamoValida = validarSolicitudPrestamo(solicitudPrestamo);

            // Creamos el detalle para obtener el id de la solicitud luego del insert
            var detalleSolicitudPrestamo = _context.SolicitudPrestamos
            .FromSqlRaw("EXEC SP_CREAR_SOLICITUD_PRESTAMO @CODIGO_SOCIO = {0}, @SOLICITUD_REALIZADA_POR = {1}, @MONTO_SOLICITADO = {2}, @PLAZO_MESES = {3}, @CANTIDAD_CUOTAS = {4}, @MONTO_POR_CUOTA = {5}, @TASA_INTERES = {6}, @ID_CATEGORIA_PRESTAMO = {7}, @ID_NIVEL_APROBACION_REQUERIDO = {8}, @CODIGO_CODEUDOR_PRINCIPAL = {9}, @CODIGO_CODEUDOR_SECUNDARIO = {10}, @CODIGO_CODEUDOR_TERCIARIO = {11}, @ESTATUS = {12}", solicitudPrestamoValida.CODIGO_SOCIO, solicitudPrestamoValida.SOLICITUD_REALIZADA_POR, solicitudPrestamoValida.MONTO_SOLICITADO, solicitudPrestamoValida.PLAZO_MESES, solicitudPrestamoValida.CANTIDAD_CUOTAS, solicitudPrestamoValida.MONTO_POR_CUOTA, solicitudPrestamoValida.TASA_INTERES, solicitudPrestamoValida.ID_CATEGORIA_PRESTAMO, solicitudPrestamoValida.ID_NIVEL_APROBACION_REQUERIDO, solicitudPrestamoValida.CODIGO_CODEUDOR_PRINCIPAL, solicitudPrestamoValida.CODIGO_CODEUDOR_SECUNDARIO, solicitudPrestamoValida.CODIGO_CODEUDOR_TERCIARIO, solicitudPrestamoValida.ESTATUS)
            .AsEnumerable()
            .FirstOrDefault();

            var idSolicitudPrestamo = Convert.ToInt32(detalleSolicitudPrestamo?.ID_SOLICITUD);

            // Creamos los detalles de los anexos en base al nivel de aprobacion requerido
            // Obtenemos los anexos requeridos del nivel de aprobacion
            var anexosRequeridos = _context.Anexos
                .FromSqlRaw("EXEC SP_OBTENER_ANEXOS_REQUERIDOS_NIVEL_APROBACION @ID_NIVEL_APROBACION = {0}", solicitudPrestamoValida.ID_NIVEL_APROBACION_REQUERIDO)
                .AsEnumerable()
                .ToList();

            if (anexosRequeridos.Count > 0)
            {


                try
                {
                    // Creamos los detalles de los anexos
                    foreach (var anexo in anexosRequeridos)
                    {

                        // Console.WriteLine("ID Anexo: " + anexo.ID_ANEXO);
                        await _context.Database.ExecuteSqlRawAsync("EXEC SP_CREAR_DETALLE_ANEXO @ID_ANEXO = {0}, @ID_SOLICITUD_PRESTAMO = {1}",
                        anexo.ID_ANEXO, idSolicitudPrestamo);

                    }

                }
                catch (Exception ex)
                {

                    throw new Exception("Ha ocurrido un error al crear los detalles de los anexos: " + ex.Message);

                }
            }

            TempData["openModal"] = true;
            TempData["Success"] = "Solicitud de préstamo creada correctamente";
            return RedirectToAction("FormAnexosSolicitudPrestamo", "Prestamos", new { IdSolicitudPrestamo = idSolicitudPrestamo });

        }

        TempData["openModal"] = true;
        TempData["Error"] = "Ha ocurrido un error al crear la solicitud de préstamo";
        return RedirectToAction("CrearSolicitudPrestamo", "Prestamos");

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    // Aprobacion de solicitudes de prestamos

    [HttpGet]
    public IActionResult AprobacionSolicitudesPrestamos()
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var solicitudesPrestamos = _context.SolicitudPrestamos
                .FromSqlRaw("EXEC SP_LEER_SOLICITUDES_PRESTAMOS")
                .AsEnumerable()
                .ToList();

            var idRol = HttpContext.Session.GetInt32("ID_ROL");
            var idNivelAprobacion = HttpContext.Session.GetInt32("ID_NIVEL_APROBACION");
            var codigoEmpleado = HttpContext.Session.GetInt32("CODIGO_EMPLEADO");

            if (idRol != 6)
            {
                // Filtra las solicitudes solo las creadas por el usuario/empleado logeado
                var solicitudesPrestamosFiltradas = solicitudesPrestamos.Where(solicitud => solicitud.SOLICITUD_REALIZADA_POR == codigoEmpleado && solicitud.ESTATUS == "Pendiente").ToList();
                return View("~/Views/Prestamos/SolicitudesPrestamos/AprobacionSolicitudesPrestamos.cshtml", solicitudesPrestamosFiltradas);
            }


            return View("~/Views/Prestamos/SolicitudesPrestamos/AprobacionSolicitudesPrestamos.cshtml", solicitudesPrestamos);


        }
        catch (Exception ex)
        {
            TempData["openModal"] = true;
            TempData["Error"] = "Ha ocurrido un error al obtener las solicitudes de préstamos:";
            Console.WriteLine("Error al obtener las solicitudes de préstamos: " + ex.Message + ex.Source); // Mensaje para el log en el server

            var emptySolicitudesPrestamos = new List<SolicitudPrestamo>();

            return View("~/Views/Prestamos/SolicitudesPrestamos/AprobacionSolicitudesPrestamos.cshtml", emptySolicitudesPrestamos);
        }
    }

    [HttpGet]
    public IActionResult ConsultarSolicitudPrestamoAprobacion(string IdSolicitudPrestamo)
    {
        if (HttpContext.Session.GetInt32("ID_USUARIO") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (IdSolicitudPrestamo != null)
        {

            var ID_SOLICITUD = Convert.ToInt32(IdSolicitudPrestamo);
            // Obtener datos necesarios para el formulario

            var socios = _context.Socios
                .FromSqlRaw("EXEC SP_LEER_SOCIOS")
                .AsEnumerable()
                .ToList();

            var categoriasPrestamo = _context.CategoriaPrestamo
                .FromSqlRaw("EXEC SP_LEER_CATEGORIAS_PRESTAMOS")
                .AsEnumerable()
                .ToList();

            var codeudores = _context.Codeudores
                .FromSqlRaw("EXEC SP_LEER_CODEUDORES")
                .AsEnumerable()
                .ToList();

            var solicitudPrestamo = _context.SolicitudPrestamos
                .FromSqlRaw("EXEC SP_BUSCAR_SOLICITUD_PRESTAMO @ID_SOLICITUD = {0}", ID_SOLICITUD)
                .AsEnumerable()
                .FirstOrDefault();

            var detallesAnexos = _context.DetalleAnexos
            .FromSqlRaw("EXEC SP_OBTENER_DETALLES_ANEXOS_SOLICITUD_PRESTAMO @ID_SOLICITUD_PRESTAMO = {0}", ID_SOLICITUD)
            .AsEnumerable()
            .ToList();


            var viewModel = new SolicitudPrestamoViewModel
            {
                SolicitudPrestamo = solicitudPrestamo,
                Socios = socios,
                CategoriasPrestamos = categoriasPrestamo,
                Codeudores = codeudores,
                DetallesAnexos = detallesAnexos
            };

            ViewBag.Title = "Consultar Solicitud de Prestamo";
            return View("~/Views/Prestamos/SolicitudesPrestamos/FormConsultaSolicitudPrestamoAprobacion.cshtml", viewModel);

        }


        TempData["openModal"] = true;
        TempData["Error"] = "No se encontró la solicitud de préstamo";
        return RedirectToAction("AprobacionSolicitudesPrestamos", "Prestamos");
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}