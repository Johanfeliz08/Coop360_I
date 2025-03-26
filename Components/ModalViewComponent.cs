using Microsoft.AspNetCore.Mvc;

public class ModalViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string tipo, string titulo, string mensaje)
    {

      if (tipo != null || titulo != null || mensaje != null){
        
        ViewBag.Tipo = tipo;
        ViewBag.Titulo = titulo;

        if (tipo == "info"){
          ViewBag.Info = mensaje;
        }

        if (tipo == "error"){
          ViewBag.Error = mensaje;
        }

        if (tipo == "success"){
          ViewBag.Success = mensaje;
        }
      } else {

        ViewBag.Tipo = "Undefined";
        ViewBag.Titulo = "Undefined";
        ViewBag.Message = "La vista no ha recibido los datos necesarios para mostrar el mensaje";
      
      }

        return View("ModalView");
    }
}