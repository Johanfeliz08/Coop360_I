using Microsoft.AspNetCore.Mvc;

public class ModalAprobacionRechazoViewComponent : ViewComponent
{
  public IViewComponentResult Invoke(string tipo, string mensaje, string controlador, string parametro, int id, string montoSolicitado)
  {

    // Tipos: aprobacion, rechazo, cambiarContrasena

    if (tipo != null || mensaje != null)
    {


      if (tipo == "aprobacion")
      {

        ViewBag.Controlador = controlador;
        ViewBag.Titulo = "¡Aprobación de préstamo!";
        ViewBag.Parametro = parametro;

        if (id != 0)
        {
          ViewBag.id = id;
        }

        if (montoSolicitado != null)
        {
          ViewBag.montoSolicitado = montoSolicitado;
        }

      }
      else if (tipo == "rechazo")
      {
        ViewBag.Controlador = controlador;
        ViewBag.Titulo = "¡Rechazo de préstamo!";
        ViewBag.Parametro = parametro;

        if (id != 0)
        {
          ViewBag.id = id;
        }
      }
      else if (tipo == "cambiarContrasena")
      {
        ViewBag.Controlador = controlador;
        ViewBag.Titulo = "¡Cambio de contraseña!";
        ViewBag.Parametro = parametro;

        if (id != 0)
        {
          ViewBag.id = id;
        }
      }

      ViewBag.Tipo = tipo;
      ViewBag.Mensaje = mensaje;

    }
    else
    {

      ViewBag.Tipo = "Undefined";
      ViewBag.Titulo = "Undefined";
      ViewBag.Mensaje = "La vista no ha recibido los datos necesarios para mostrar el mensaje";

    }

    return View("ModalAprobacionRechazoView");
  }
}