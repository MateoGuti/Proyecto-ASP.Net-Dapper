using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Subsistema_Estudiantil.Models;
using Subsistema_Estudiantil.Servicios;
using System.Data;
using System.Runtime.Intrinsics.Arm;

namespace Subsistema_Estudiantil.Controllers
{
    public class ControladorEstudiante : Controller
    {
        private readonly IRepositorioEstudiante repositorioEstudiante;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioMaestras repositorioMaestras;

        public ControladorEstudiante(IRepositorioEstudiante repositorioEstudiante,
                                    IServicioUsuarios servicioUsuarios,IRepositorioMaestras repositorioMaestras)
        {
            this.repositorioEstudiante = repositorioEstudiante;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioMaestras = repositorioMaestras;
        }
        //Todo lo que se hace en este proceso se hace con el fin de poder imprimir las carreras en el listado del formulario 
        //_solicitudes pendientes. 
        public async Task<IActionResult> SolicitudesEstudiante()
        {
            var usuarioId = servicioUsuarios.ObtenerIdUsuario();
            // Se obtienen las carreras de la vista CarreraU con el método de ObtenerCarrera
            var tiposCarreras = await repositorioMaestras.ObtenerCarrera(usuarioId);

            // Se crea una lista de SelectListItem desde la lista de tiposCarreras
            var listaCarrerasSelectList = tiposCarreras
                .Select(carrera => new SelectListItem
                {
                    Value = carrera.Id.ToString(),
                    Text = carrera.Carrera
                });

            // Crear el modelo CarreaCreacionViewModel con la lista de SelectListItem
            var modelo = new CarreaCreacionViewModel
            {
                CarreraId = listaCarrerasSelectList
            };
            return View(modelo); // Aquí es donde se debe pasar el modelo correcto a la vista.
        }

        [HttpGet]
        public async Task<IActionResult> TablaEstudianteCancelada()
        {
            var usuarioid = servicioUsuarios.ObtenerIdUsuario();
            var tblEstudiante = await repositorioEstudiante.TablaEstudianteCancelada(usuarioid);
            return View(tblEstudiante);
        }
        //Como se creo CarreaCreacionViewModel que hereda de formularioEstudiantes se debe cambiar lo que recibe el 
        //metodo por este modelo que esta heredando. 
        [HttpPost]
        public async Task<IActionResult> SolicitudesEstudiante(CarreaCreacionViewModel formularioestudiantes)
        {
            if (!ModelState.IsValid)
            {
                // Recargar las opciones del select
                formularioestudiantes.CarreraId = await ObtenerListaCarreras();
                return View(formularioestudiantes);
            }
            formularioestudiantes.IdUsuario = servicioUsuarios.ObtenerIdUsuario();
            await repositorioEstudiante.InsertEstudiante(formularioestudiantes);
            return RedirectToAction("CorrectaSolicitud");
        }
        //Este metodo private se crea con la intencion de recargar el select del formulario 
        //solicitudesEstudiantes ya que de no hacerlo se pierde la informacion que me trae el select. 
        private async Task<IEnumerable<SelectListItem>> ObtenerListaCarreras()
        {
            var usuarioId = servicioUsuarios.ObtenerIdUsuario();
            var tiposCarreras = await repositorioMaestras.ObtenerCarrera(usuarioId);

            return tiposCarreras
                .Select(carrera => new SelectListItem
                {
                    Value = carrera.Id.ToString(),
                    Text = carrera.Carrera
                });
        }
        public IActionResult CorrectaSolicitud()
        {
            return View();
        }
        //Lista las solicitudes pendientes de los estudiantes.
        [HttpGet]
        public async Task<IActionResult> TablaEstudiante()
        {
            var usuarioid = servicioUsuarios.ObtenerIdUsuario();
            var tblEstudiante = await repositorioEstudiante.ObtenerEstudiante(usuarioid);
            return View(tblEstudiante);
        } 
        
        //El codigo no me mapeaba y traia el id aun asi teniendo el modelo de forma adecuada,
        //por que tenia que configurar la url ya que por defecto siempre espera que nombremos
        //el parametro que espera la funcion como "id" por lo que si pretendo nombrar este de
        //otra manera necesariamente debo cambiar el estado de la url y poner el nombre que quiero
        //tener, en este caso "idP"

        [HttpGet("ControladorEstudiante/EditarP/{idP}")]
        public async Task<IActionResult> EditarP(int idP)
        {
            var idregistro = await repositorioEstudiante.ObtenerIdRegistroE(idP);
            return View(idregistro);
        }

        [HttpPost]
        public async Task<IActionResult> EditarP(FormularioEstudiantes formularioestudiante)
        {
            await repositorioEstudiante.ActualizarE(formularioestudiante);
            return RedirectToAction("TablaEstudiante");
        }
        [HttpGet("ControladorEstudiante/EliminarE/{idE}")]
        public async Task<IActionResult> EliminarE(int idE)
        {
            var EliminarRegistroE = await repositorioEstudiante.ObtenerIdRegistroE(idE);
            return View(EliminarRegistroE);
        }
        [HttpPost]
        public async Task<IActionResult> EliminarE(FormularioEstudiantes formularioEstudiantes)
        {
            formularioEstudiantes.IdUsuario = servicioUsuarios.ObtenerIdUsuario();
            await repositorioEstudiante.EliminarE(formularioEstudiantes);
            return RedirectToAction("TablaEstudiante");
        }


        [HttpPost]
        //Obtiene los ids y los compara, para asi actualizar el orden en la tabla de profesor
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioid = servicioUsuarios.ObtenerIdUsuario();
            var formularioE = await repositorioEstudiante.ObtenerEstudiante(usuarioid);
            var idsFormularioE = formularioE.Select(x => x.IdEstudiante);
            var idsFormularioOrdenadoE = ids.Select((valor, indice) => new FormularioEstudiantes()
                                        { IdEstudiante = valor, Orden = indice + 1 }).AsEnumerable();
            await repositorioEstudiante.Ordenar(idsFormularioOrdenadoE);
            return Ok();
        }
        //Obtiene el detalle de las solicitudes canceladas
        [HttpGet]
        
        public async Task<IActionResult> DetalleEC(int id)
        {
            var idDetalle = await repositorioEstudiante.ObtenerIdRegistroEC(id);
            return View(idDetalle);
        }
        //Se envia un post para rellenar los campos y porque previamente se utilizo un query de tipo actualizar.
        [HttpPost]

        public async Task<IActionResult> DetalleEC(FormularioEstudiantes formularioEstudiantes)
        {
            await repositorioEstudiante.DetalleEC(formularioEstudiantes);
            return RedirectToAction("TablaEstudianteCancelada");
        }
        //Obtiene el detalle de las solicitudes pendientes del estudiante
        [HttpGet]

        public async Task<IActionResult> DetalleE(int id)
        {
            var idDetalle = await repositorioEstudiante.ObtenerIdRegistroE(id);
            return View(idDetalle);
        }
        //Se envia un post para rellenar los campos y porque previamente se utilizo un query de tipo actualizar.
        [HttpPost]

        public async Task<IActionResult> DetalleE(FormularioEstudiantes formularioEstudiantes)
        {
            await repositorioEstudiante.DetalleE(formularioEstudiantes);
            return RedirectToAction("TablaEstudianteCancelada");
        }
    }

}
