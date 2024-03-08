using Microsoft.AspNetCore.Mvc;

namespace Subsistema_Estudiantil.Controllers
{
    public class ControladorContenido: Controller
    {
        public IActionResult Contenido()
        {
            return View();
        }
    }
}
