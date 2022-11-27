namespace WebApiCasino.Entidades
{
    public class ParticipantesRifasCartas
    {
        public int Id { get; set; }
        public string IdParticipante { get; set; }

        public string IdRifa { get; set; }

        public string IdCarta { get; set; }

        public int Orden { get; set; }

        public Participantes Participante { get; set; }

        public Rifas Rifa { get; set;  }

        public CartasLoteMex Carta { get; set; }
    }
}
