using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Subsistema_Estudiantil.Models
{
    public class FormularioCarrera
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100)]
        public string Carrera { get; set; }
        public int UsuarioId { get; set; }
    }
}
