using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.Entidades
{
    public class Premios
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo de nombre es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage = "El campo de nombre no puede excede los 50 caracteres")]
        [NombresConMayuscula]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo de nivel es requerido")]
        public int Nivel { get; set; }

        public int RifaId { get; set; }

        public Rifas Rifa { get; set; }


    }
}
