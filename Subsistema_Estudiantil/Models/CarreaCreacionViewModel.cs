using Microsoft.AspNetCore.Mvc.Rendering;
//Esto se crea con el fin de crear una propiedad que herede de formulario carrera para crar una propiedad que reciba 
//el listado. 
namespace Subsistema_Estudiantil.Models
{
    public class CarreaCreacionViewModel : FormularioEstudiantes
    {
        public IEnumerable<SelectListItem> CarreraId { get; set; }
    }
}
