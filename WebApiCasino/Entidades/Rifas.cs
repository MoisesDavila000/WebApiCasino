using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.Entidades
{
    public class Rifas
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo de nombre es requerido")]
        [NombresConMayuscula]
        public string Nombre { get; set; }
        [Required]
        public string FechaRifa { get; set; }

        public List<ParticipantesRifasCartas> participantesRifasCartas { get; set; }

        public List<Premios> Premios { get; set; }

    }
}
