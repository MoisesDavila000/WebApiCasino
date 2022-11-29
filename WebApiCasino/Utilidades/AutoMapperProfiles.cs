using AutoMapper;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;

namespace WebApiCasino.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            //Participantes
            CreateMap<CreacionParticipanteDTO, Participantes>();
            CreateMap<Participantes, GETParticipantesDTO>();

            CreateMap<Participantes, ParticipanteDTOconCartas>()
                .ForMember(participante => participante.ListaDeCartas, opciones => opciones.MapFrom(MapParticipanteCarta))
                .ForMember(participante => participante.ListaDeRifas, opciones => opciones.MapFrom(MapParticipanteRifa));

            //Rifas

            CreateMap<CreacionRifaDTO, Rifas>();
            CreateMap<Rifas, GETRifasDTO>();

            CreateMap<Rifas, RifaconPremiosDTO>()
                .ForMember(rifa => rifa.PremiosDTO, opciones => opciones.MapFrom(MapRifaPremio));

            CreateMap<Rifas, RifaconCartaDTO>()
                .ForMember(rifa => rifa.CartasDTO, opciones => opciones.MapFrom(MapRifaCarta));

            //Premios

            CreateMap<CreacionPremioDTO, Premios>();
            CreateMap<Premios, GETPremiosDTO>();
            CreateMap<GETPremiosDTO, Premios>();

            //Cartas
            CreateMap<CreacionCartaDTO, CartasLoteMex>();
            CreateMap<CartasLoteMex, GETCartasDTO>();

            //Extras
            CreateMap<ParticipantePatchDTO, Participantes>().ReverseMap();
            CreateMap<ParticipantesRifasCartas, ParticipantesCartasRifaDTO>();

        }

        private List<GETCartasDTO> MapParticipanteCarta(Participantes participantes, GETParticipantesDTO getParticipantesDTO)
        {
            var resultado = new List<GETCartasDTO>();

            if (participantes.ParticipantesRifasCartas == null) { return resultado; }

            foreach (var partiCarta in participantes.ParticipantesRifasCartas)
            {
                resultado.Add(new GETCartasDTO()
                {
                    Nombre = partiCarta.Carta.Nombre,
                    NumLoteria = partiCarta.Carta.NumLoteria
                });
            }

            return resultado;
        }

        private List<GETRifasDTO> MapParticipanteRifa(Participantes participantes, GETParticipantesDTO getParticipantesDTO)
        {
            var resultado = new List<GETRifasDTO>();

            if (participantes.ParticipantesRifasCartas == null) { return resultado; }

            foreach (var partiCarta in participantes.ParticipantesRifasCartas)
            {
                resultado.Add(new GETRifasDTO()
                {
                    Nombre = partiCarta.Rifa.Nombre,
                    FechaRifa = partiCarta.Rifa.FechaRifa
                });
            }

            return resultado;
        }

        private List<GETPremiosDTO> MapRifaPremio(Rifas rifa, RifaconPremiosDTO rifaconPremiosDTO)
        {
            var resultado = new List<GETPremiosDTO>();

            if(rifa.Premios == null) { return resultado; }

            foreach (var i in rifa.Premios)
            {
                resultado.Add(new GETPremiosDTO()
                {
                    Nombre = i.Nombre,
                    Nivel = i.Nivel
                });
            }

            return resultado;
        }

        private List<GETCartasDTO> MapRifaCarta(Rifas rifa, RifaconCartaDTO rifaconCartaDTO)
        {
            var resultado = new List<GETCartasDTO>();

            if (rifa.participantesRifasCartas == null) { return resultado; }

            foreach (var i in rifa.participantesRifasCartas)
            {
                resultado.Add(new GETCartasDTO()
                {
                    Nombre = i.Carta.Nombre, 
                    NumLoteria = i.Carta.NumLoteria

                });
            }

            return resultado;
        }

    }
}
