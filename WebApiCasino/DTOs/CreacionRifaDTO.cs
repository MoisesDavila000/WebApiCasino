using System.ComponentModel.DataAnnotations;
using WebApiCasino.Entidades;
using WebApiCasino.Validaciones;

namespace WebApiCasino.DTOs
{
    public class CreacionRifaDTO
    {
        [Required(ErrorMessage = "El campo de nombre es requerido")]
        [NombresConMayuscula]
        public string Nombre { get; set; }
        [Required]
        public string FechaRifa { get; set; }

    }
}
