using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Subsistema_Estudiantil.Models;
using Subsistema_Estudiantil.Servicios;
using System.Data;

namespace Subsistema_Estudiantil.Controllers
{
    public class ControladorProfesor : Controller
    {
        private readonly IRepositorioProfesor repositorioProfesor;
        private readonly IServicioUsuarios servicioUsuarios;

        public ControladorProfesor(IRepositorioProfesor repositorioProfesor, 
                                    IServicioUsuarios servicioUsuarios)
        {
            this.repositorioProfesor = repositorioProfesor;
            this.servicioUsuarios = servicioUsuarios;
        }
        public IActionResult SolicitudesProfesor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SolicitudesProfesor(FormularioProfesor formularioprofesor)
        {
            if (!ModelState.IsValid)
            {
                return View(formularioprofesor);
            }

            formularioprofesor.IdUsuario = servicioUsuarios.ObtenerIdUsuario();
            await repositorioProfesor.InsertProfesor(formularioprofesor);

            return RedirectToAction("CorrectaSolicitud");
        }
        [HttpGet]
        public IActionResult CorrectaSolicitud()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> TablaProfesor()
        {
            var usuario = servicioUsuarios.ObtenerIdUsuario();
            var obtener=await repositorioProfesor.Obtener(usuario);
            return View(obtener);
        }

        [HttpGet]
        public async Task<IActionResult> TablaProfesorCancelada()
        {
            var usuario = servicioUsuarios.ObtenerIdUsuario();
            var obtener = await repositorioProfesor.TablaProfesorCancelada(usuario);
            return View(obtener);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var idregistro = await repositorioProfesor.ObtenerIdRegistro(id);
            return View(idregistro);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(FormularioProfesor formularioProfesor)
        {
            await repositorioProfesor.Actualizar(formularioProfesor);
            return RedirectToAction("TablaProfesor");
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var Eliminarregistro = await repositorioProfesor.ObtenerIdRegistro(id);
            return View(Eliminarregistro);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(FormularioProfesor formularioProfesor)
        {
            formularioProfesor.IdUsuario = servicioUsuarios.ObtenerIdUsuario();
            await repositorioProfesor.Eliminar(formularioProfesor);
            return RedirectToAction("TablaProfesor");
        }


        [HttpPost]
        //Obtiene los ids y los compara, para asi actualizar el orden en la tabla de profesor
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuario = servicioUsuarios.ObtenerIdUsuario(); 
            var formulario = await repositorioProfesor.Obtener(usuario);
            var idsFormulario = formulario.Select(x => x.IdProfesor);
            var idsFormularioOrdenado = ids.Select((valor, indice) => new FormularioProfesor() 
                                           { IdProfesor= valor, Orden=indice+1}).AsEnumerable();
            await repositorioProfesor.Ordenar(idsFormularioOrdenado);
            return Ok();
        }

        [HttpGet]

        public async Task<IActionResult> DetallePC(int id)
        {
            var idDetalle = await repositorioProfesor.ObtenerIdRegistroPC(id);
            return View(idDetalle);
        }

        [HttpPost]

        public async Task<IActionResult> DetallePC(FormularioProfesor formularioProfesor)
        {
            await repositorioProfesor.DetallePC(formularioProfesor);
            return RedirectToAction("TablaProfesorCancelada");
        }

        //Obtiene el detalle de las solicitudes pendientes del Profesor
        [HttpGet]

        public async Task<IActionResult> DetalleP(int id)
        {
            var idDetalle = await repositorioProfesor.ObtenerIdRegistroP(id);
            return View(idDetalle);
        }
        //Se envia un post para rellenar los campos y porque previamente se utilizo un query de tipo actualizar.
        [HttpPost]

        public async Task<IActionResult> DetalleP(FormularioProfesor formularioprofesor)
        {
            await repositorioProfesor.DetalleP(formularioprofesor);
            return RedirectToAction("TablaProfesorCancelada");
        }
    }
}
