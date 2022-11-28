namespace WebApiCasino.DTOs
{
    public class GanadorDTO
    {
        public string NombreRifa { get; set; }
        public GETParticipantesDTO Participante { get; set; }
        public GETPremiosDTO Premio { get; set; }
    }
}
