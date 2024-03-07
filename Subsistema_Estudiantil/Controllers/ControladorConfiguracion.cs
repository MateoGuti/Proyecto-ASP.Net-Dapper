using Microsoft.AspNetCore.Mvc;

namespace Subsistema_Estudiantil.Controllers
{
    public class ControladorConfiguracion : Controller
    {
        public IActionResult Configuracion()
        {
            return View();
        }
    }
}
