using System.ComponentModel.DataAnnotations;

namespace WebApiCasino.DTOs
{
    public class GETPremiosDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Nivel { get; set; }
    }
}
