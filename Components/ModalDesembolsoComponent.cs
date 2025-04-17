using Microsoft.AspNetCore.Mvc;

public class ModalDesembolsoViewComponent : ViewComponent
{
  public IViewComponentResult Invoke(int id)
  {


    if (id > 0)
    {

      ViewBag.id = id;
      ViewBag.Titulo = "¡Desembolso de prestamo!";

    }
    else
    {
      ViewBag.Titulo = "Undefined";
      ViewBag.Mensaje = "La vista no ha recibido los datos necesarios para mostrar el mensaje";
    }

    return View("ModalDesembolsoView");
  }
}