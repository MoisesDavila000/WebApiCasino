namespace WebApiCasino.DTOs
{
    public class GETGanadorDTO
    {
        public string NombreRifa { get; set; }
        public GETParticipantesDTO Participante { get; set; }
        public GETPremiosDTO Premio { get; set; }
    }
}
