using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.DTOs
{
    public class GETRifasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string FechaRifa { get; set; }
    }
}
