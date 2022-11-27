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

            //Rifas

            CreateMap<CreacionRifaDTO, Rifas>()
                .ForMember(rifa => rifa.Premios, opciones => opciones.MapFrom(MapCreacionRifa));

            CreateMap<Rifas, RifaconPremiosDTO>()
                .ForMember(rifa => rifa.PremiosDTO, opciones => opciones.MapFrom(MapRifaPremio));

            CreateMap<Rifas, RifaconCartaDTO>()
                .ForMember(rifa => rifa.CartasDTO, opciones => opciones.MapFrom(MapRifaCarta));

            //Premios

            CreateMap<CreacionPremioDTO, Premios>();
            CreateMap<Premios, GETPremiosDTO>();

            //Cartas
            CreateMap<CreacionCartaDTO, CartasLoteMex>();
            CreateMap<CartasLoteMex, GETCartasDTO>();

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

        private List<Premios> MapCreacionRifa(CreacionRifaDTO creacionRifaDTO, Rifas rifa)
        {
            var resultado = new List<Premios>();

            if (creacionRifaDTO.Premios == null) { return resultado; }

            foreach (var i in creacionRifaDTO.Premios)
            {
                resultado.Add(new Premios()
                {
                    RifaId = rifa.Id,
                    Nombre = i.Nombre,
                    Nivel = i.Nivel

                });
            }

            return resultado;
        }
    }
}
