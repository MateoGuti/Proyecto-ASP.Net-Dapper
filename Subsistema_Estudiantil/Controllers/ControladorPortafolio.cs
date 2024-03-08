using Microsoft.AspNetCore.Mvc;

namespace Subsistema_Estudiantil.Controllers
{
    public class ControladorPortafolio : Controller
    {
        public IActionResult Portafolio()
        {
            return View();
        }

        public IActionResult PresentacionPortafolio()
        {
            return View();
        }
    }
}
