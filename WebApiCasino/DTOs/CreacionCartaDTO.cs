using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.DTOs
{
    public class CreacionCartaDTO
    {
        [Required(ErrorMessage = "El campo de nombre es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage = "El campo de nombre no puede excede los 50 caracteres")]
        [NombresConMayuscula]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo de NumLoteria es requerido")]
        public int NumLoteria { get; set; }
    }
}
