using Microsoft.AspNetCore.Mvc;

public class ModalViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string tipo, string mensaje, string controlador, string parametro , int id)
    {

      // Tipos: error, success, confirmation

      if (tipo != null  || mensaje != null){
        

        if (tipo == "success") {
          ViewBag.Titulo = "¡Enhorabuena!";
        } else

        if (tipo == "error") {
          ViewBag.Titulo = "¡Oopss!";
        }

        if (tipo == "confirmation") {
          
          ViewBag.Controlador = controlador;
          ViewBag.Titulo = "¡Cuidado!";
          ViewBag.Parametro = parametro;   

          if (id != 0) {
            ViewBag.id = id;
          }

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