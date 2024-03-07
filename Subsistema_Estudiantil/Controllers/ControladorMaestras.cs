using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Subsistema_Estudiantil.Models;
using Subsistema_Estudiantil.Servicios;

namespace Subsistema_Estudiantil.Controllers
{
    public class ControladorMaestras : Controller
    {
        private readonly IRepositorioMaestras repositorioMaestras;
        private readonly IServicioUsuarios servicioUsuarios;

        public ControladorMaestras(IRepositorioMaestras repositorioMaestras,IServicioUsuarios servicioUsuarios) 
        {
            this.repositorioMaestras = repositorioMaestras;
            this.servicioUsuarios = servicioUsuarios;
        }
        public IActionResult CorrectaSolicitud()
        {
            return View();
        }
        //Lista las carerras agregadas por el administrador del sistema. 
        [HttpGet]
        public async Task<IActionResult> CarreraU()
        {
            var usuarioid = servicioUsuarios.ObtenerIdUsuario();
            var tblCarrera = await repositorioMaestras.ObtenerCarrera(usuarioid);
            return View(tblCarrera);
        }
        [HttpGet]
        public async Task<IActionResult> InsertCarrera()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertCarrera(FormularioCarrera formularioCarrera)
        {
            if (!ModelState.IsValid)
            {
                return View(formularioCarrera);
            }
            formularioCarrera.UsuarioId = servicioUsuarios.ObtenerIdUsuario();
            await repositorioMaestras.InsertCarrera(formularioCarrera);
            return RedirectToAction("CorrectaSolicitud");
        }
        //Este id se trae de la vista CarreraU donde pasamos el route-id que tendria que ser igual al Id del formulario. 
        [HttpGet]
        public async Task<IActionResult> EliminarCarrera(int id)
        {
            var EliminarRegistroC = await repositorioMaestras.ObtenerIdRegistroC(id);
            return View(EliminarRegistroC);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCarrera(FormularioCarrera formularioCarrera)
        {
            formularioCarrera.UsuarioId = servicioUsuarios.ObtenerIdUsuario();
            await repositorioMaestras.EliminarCarrera(formularioCarrera);
            return RedirectToAction("CarreraU");
        }
    }
}
