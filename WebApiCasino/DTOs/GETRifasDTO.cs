using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.DTOs
{
    public class GETRifasDTO
    {

        public string Nombre { get; set; }

        public DateTime FechaRifa { get; set; }
    }
}
