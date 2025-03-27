using Microsoft.AspNetCore.Mvc;

public class ModalViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string tipo, string mensaje)
    {

      // Tipos: error, success

      if (tipo != null  || mensaje != null){
        

        if (tipo == "success") {
          ViewBag.Titulo = "¡Enhorabuena!";
        } else

        if (tipo == "error") {
          ViewBag.Titulo = "¡Oopss!";
        }

        ViewBag.Tipo = tipo;
        ViewBag.Mensaje = mensaje;

      } else {

        ViewBag.Tipo = "Undefined";
        ViewBag.Titulo = "Undefined";
        ViewBag.Mensaje = "La vista no ha recibido los datos necesarios para mostrar el mensaje";
      
      }

        return View("ModalView");
    }
}