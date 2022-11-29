using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;
using WebApiCasino.Filtros;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("Participantes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class ParticipantesController: ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ParticipantesController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;

        public ParticipantesController(ApplicationDbContext dbContext, ILogger<ParticipantesController> logger, IWebHostEnvironment env, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.env = env;
            this.mapper = mapper;
        }

        [HttpGet("(Datos_Del_Participante)/{id:int}", Name = "Datos_Del_Participante")]
        public async Task<ActionResult<GETParticipantesDTO>> GetById([FromRoute] int id)
        {

            var participante = await dbContext.Participantes.FirstOrDefaultAsync(x => x.Id == id);

            if (participante == null)
            {
                return NotFound("No se encontro participante con ese id");
            }

            logger.LogInformation("Se obtiene el registro de un participante.");

            return mapper.Map<GETParticipantesDTO>(participante);
        }

        [HttpGet("Inscripciones_Del_Participante/{id:int}", Name = "Inscripciones_Del_Participante")]
        public async Task<ActionResult<ParticipanteDTOconCartas>> GetInscripcion([FromRoute] int id)
        {
            var inscrip = await dbContext.ParticipantesRifasCartas.Where(x => x.IdParticipante == id.ToString()).ToListAsync();

            var participante = await dbContext.Participantes
                .Include(participanteDB => participanteDB.ParticipantesRifasCartas)
                .ThenInclude(participantesRifasCartasDB => participantesRifasCartasDB.Carta)
                .Include(participanteDB => participanteDB.ParticipantesRifasCartas)
                .ThenInclude(participantesRifasCartasDB => participantesRifasCartasDB.Rifa)
                .FirstOrDefaultAsync(participanteDB => participanteDB.Id == id);
            
            if (participante == null)
            {
                logger.LogError("No se encuentra el participante con dicho id.");
                return NotFound();
            }
            List<GETCartasDTO> listacartas = new List<GETCartasDTO>();
            List<GETRifasDTO> listarifa = new List<GETRifasDTO>();

            foreach (var i in participante.ParticipantesRifasCartas)
            {
                listacartas.Add(new GETCartasDTO
                {
                    Nombre = i.Carta.Nombre,
                    NumLoteria = i.Carta.NumLoteria
                });

                listarifa.Add(new GETRifasDTO
                {
                    Id = Int32.Parse(i.IdRifa),
                    Nombre = i.Rifa.Nombre,
                    FechaRifa = i.Rifa.FechaRifa
                });
            }
            
            var inscripciones = new ParticipanteDTOconCartas()
            {
                Id = participante.Id,
                Nombre = participante.Nombre,
                Email = participante.Email,
                Telefono = participante.Telefono,
                ListaDeCartas = listacartas,
                ListaDeRifas = listarifa

            };

            return mapper.Map<ParticipanteDTOconCartas>(inscripciones);
        }


        [HttpPost("Crear_Participante")]
        public async Task<ActionResult> Post(CreacionParticipanteDTO creacionParticipanteDTO)
        {
            var existeCorreo = await dbContext.Participantes.AnyAsync(x => x.Email == creacionParticipanteDTO.Email);

            if (existeCorreo)
            {
                return BadRequest("Ya existe este correo");
            }


            var newParticipante = mapper.Map<Participantes>(creacionParticipanteDTO);
            dbContext.Add(newParticipante);

            var partDTO = mapper.Map<GETParticipantesDTO>(newParticipante);
            await dbContext.SaveChangesAsync();

            var creado = await dbContext.Participantes.FirstOrDefaultAsync(participanteDB => participanteDB.Email == creacionParticipanteDTO.Email);
            partDTO.Id = creado.Id;
            return CreatedAtRoute("Datos_Del_Participante", new { Id = creado.Id }, partDTO);
        }

        [HttpPost("Comprar_Carta_De_Rifa/ Participante{idParticipante:int}/Rifa{idRifa:int}/Carta{NumLoteria:int}")]
        [ServiceFilter(typeof(FiltroRegistro))]
        public async Task<ActionResult> PostRelacion(int idParticipante, int idRifa, int NumLoteria)
        {
            var existeParticipante = await dbContext.Participantes.AnyAsync(x => x.Id == idParticipante);

            if (!existeParticipante)
            {
                return NotFound("No se encontro un participante con ese Id");
            }

            var existeRifa = await dbContext.Rifas.AnyAsync(x => x.Id == idRifa);

            if (!existeRifa)
            {
                return NotFound("No se encontro una rifa con ese Id");
            }

            var existeCarta = await dbContext.Cartas.AnyAsync(x => x.Id == NumLoteria);

            if (!existeCarta)
            {
                return NotFound("No se encontro un carta con ese Id");
            }

            try
            {
                var relacion = await dbContext.ParticipantesRifasCartas.SingleAsync(x => x.IdParticipante == idParticipante.ToString() && x.IdRifa == idRifa.ToString() && x.IdCarta == NumLoteria.ToString());
                return BadRequest("Ya existe la relacion.");
            }
            catch
            {
                var listaParticipantes = await dbContext.ParticipantesRifasCartas.Where(x => x.IdRifa == idRifa.ToString()).ToListAsync();

                if (!(listaParticipantes.Count() <= 54))
                {
                    return BadRequest("No hay lugar disponible en la rifa");
                }

                var cartaUsada = await dbContext.ParticipantesRifasCartas.AnyAsync(x => x.IdRifa == idRifa.ToString() && x.IdCarta == NumLoteria.ToString());

                if (cartaUsada)
                {
                    return BadRequest("La carta ya esta siendo usada en esta rifa");
                }

                var carta = await dbContext.Cartas.FirstAsync(x => x.Id == NumLoteria);
                var rifa = await dbContext.Rifas.FirstAsync(x => x.Id == idRifa);
                var participante = await dbContext.Participantes.FirstAsync(x => x.Id == idParticipante);
                var nuevaRelacion = new ParticipantesRifasCartas()
                {
                    IdParticipante = idParticipante.ToString(),
                    IdRifa = idRifa.ToString(),
                    IdCarta = NumLoteria.ToString(),
                    Participante = participante,
                    Rifa = rifa,
                    Carta = carta
                };
                dbContext.Add(nuevaRelacion);

                var relacionDTO = mapper.Map<ParticipantesRifasCartas>(nuevaRelacion);
                await dbContext.SaveChangesAsync();

                return Ok();
            }
        }


        [HttpPatch("Modificar_Participante/{id:int}")]
        [ServiceFilter(typeof(FiltroRegistro))]
        public async Task<ActionResult> Patch2(int id, JsonPatchDocument<ParticipantePatchDTO> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            var participanteDB = await dbContext.Participantes.FirstOrDefaultAsync(x => x.Id == id);

            if (participanteDB == null) { return NotFound(); }

            var participanteDTO = mapper.Map<ParticipantePatchDTO>(participanteDB);

            patchDocument.ApplyTo(participanteDTO);

            var isValid = TryValidateModel(participanteDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(participanteDTO, participanteDB);
            await dbContext.SaveChangesAsync();

            return Ok();

        }


        [HttpDelete("Eliminar_Inscripcion/Participante{idParticipante:int}/Rifa{idRifa:int}/Carta{NumLoteria:int}")]
        public async Task<ActionResult> DeleteRelacion(int idParticipante, int idRifa, int NumLoteria)
        {
            var existeParticipante = await dbContext.Participantes.AnyAsync(x => x.Id == idParticipante);

            if (!existeParticipante)
            {
                return NotFound("No se encontro un participante con ese Id");
            }

            var existeRifa = await dbContext.Rifas.AnyAsync(x => x.Id == idRifa);

            if (!existeRifa)
            {
                return NotFound("No se encontro una rifa con ese Id");
            }

            var existeCarta = await dbContext.Cartas.AnyAsync(x => x.Id == NumLoteria);

            if (!existeCarta)
            {
                return NotFound("No se encontro un carta con ese Id");
            }

            dbContext.Remove(new ParticipantesRifasCartas
            {
                IdParticipante = idParticipante.ToString(),
                IdRifa = idRifa.ToString(),
                IdCarta = NumLoteria.ToString()
            });

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
