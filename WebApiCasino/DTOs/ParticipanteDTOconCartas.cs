namespace WebApiCasino.DTOs
{
    public class ParticipanteDTOconCartas: GETParticipantesDTO
    {
        public List<GETCartasDTO> ListaDeCartas { get; set; }
    }
}
