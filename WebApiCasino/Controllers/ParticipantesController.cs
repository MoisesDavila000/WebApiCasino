using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("Participantes")]
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

        //GET

        [HttpGet]
        public async Task<ActionResult<List<Participantes>>> GetPrueba()
        {
            return await dbContext.Participantes.ToListAsync();
        }

        [HttpGet("Listado_De_Participantes")]
        //[AllowAnonymous]
        public async Task<ActionResult<List<GETParticipantesDTO>>> Get()
        {
            /*Escribir en archivo*/
            logger.LogInformation("Se obtiene el listado de participantes.");
            var participantes = await dbContext.Participantes.ToListAsync();
            return mapper.Map<List<GETParticipantesDTO>>(participantes);
        }

        [HttpGet("(Datos_Del_Participante)/{id:int}", Name = "Datos_Del_Participante")]
        public async Task<ActionResult<GETParticipantesDTO>> GetById(int id)
        {

            var participante = await dbContext.Participantes.FirstOrDefaultAsync(x => x.Id == id);

            if (participante == null)
            {
                return NotFound("No se encontro participante con ese id");
            }

            /*Escribir en archivo*/
            logger.LogInformation("Se obtiene el registro de un participante.");

            return mapper.Map<GETParticipantesDTO>(participante);
        }

        [HttpGet("(Participante_Con_Rifas)/{id:int}", Name = "Participante_Con_Rifa")]
        public async Task<ActionResult<GETParticipantesDTO>> GetPartRif(int id)
        {
            var participante = await dbContext.Participantes
                .Include(participanteDB => participanteDB.ParticipantesRifasCartas)
                .ThenInclude(participanteRifas => participanteRifas.Rifa)
                .Include(participanteDB => participanteDB.ParticipantesRifasCartas)
                .ThenInclude(participanteCartas => participanteCartas.Carta)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (participante == null)
            {
                return NotFound("No se encontro participante con ese id");
            }

            participante.ParticipantesRifasCartas = participante.ParticipantesRifasCartas.OrderBy(x => x.Orden).ToList();
            /*Escribir en archivo*/
            logger.LogInformation("Se obtiene el registro de un participante.");

            return mapper.Map<GETParticipantesDTO>(participante);

        }

        //POST

        [HttpPost]
        public async Task<ActionResult> PostPrueba(Participantes participante)
        {
            dbContext.Add(participante);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Crear_Participante")]
        public async Task<ActionResult> Post(CreacionParticipanteDTO creacionParticipanteDTO)
        {
            var existeParticipante = await dbContext.Participantes.AnyAsync(x => x.Email == creacionParticipanteDTO.Email);

            if (existeParticipante)
            {
                return BadRequest("Ya existe este participante");
            }

            var newParticipante = mapper.Map<Participantes>(creacionParticipanteDTO);
            dbContext.Add(newParticipante);

            var partDTO = mapper.Map<GETParticipantesDTO>(newParticipante);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Comprar_Carta_De_Rifa/ Participante{idParticipante:int}/Rifa{idRifa:int}/Carta{idCarta:int}")]
        public async Task<ActionResult> PostRelacion(int idParticipante, int idRifa, int idCarta)
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

            var existeCarta = await dbContext.Cartas.AnyAsync(x => x.Id == idCarta);

            if (!existeCarta)
            {
                return NotFound("No se encontro un carta con ese Id");
            }

            try
            {
                var relacion = await dbContext.ParticipantesRifasCartas.SingleAsync(x => x.IdParticipante == idParticipante.ToString() && x.IdRifa == idRifa.ToString() && x.IdCarta == idCarta.ToString());
                return BadRequest("Ya existe la relacion.");
            }
            catch
            {
                var listaParticipantes = await dbContext.ParticipantesRifasCartas.Where(x => x.IdRifa == idRifa.ToString()).ToListAsync();

                if (!(listaParticipantes.Count() <= 54))
                {
                    return BadRequest("No hay lugar disponible en la rifa");
                }

                var cartaUsada = await dbContext.ParticipantesRifasCartas.AnyAsync(x => x.IdRifa == idRifa.ToString() && x.IdCarta == idCarta.ToString());

                if (cartaUsada)
                {
                    return BadRequest("La carta ya esta siendo usada en esta rifa");
                }

                var carta = await dbContext.Cartas.FirstAsync(x => x.Id == idCarta);
                var rifa = await dbContext.Rifas.FirstAsync(x => x.Id == idRifa);
                var participante = await dbContext.Participantes.FirstAsync(x => x.Id == idParticipante);
                var nuevaRelacion = new ParticipantesRifasCartas()
                {
                    IdParticipante = idParticipante.ToString(),
                    IdRifa = idRifa.ToString(),
                    IdCarta = idCarta.ToString(),
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

        //PUT

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutPrueba(Participantes participante, int id)
        {
            if(participante.Id != id)
            {
                return BadRequest();
            }

            dbContext.Update(participante);

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("Modificar_Participante/{id:int}")]
        public async Task<ActionResult> Put(CreacionParticipanteDTO creacionParticipanteDTO, int id)
        {
            var existeParticipante = await dbContext.Participantes.AnyAsync(x => x.Id == id);

            if (!existeParticipante)
            {
                return BadRequest("El participante no existe");
            }

            var participante = mapper.Map<Participantes>(creacionParticipanteDTO);
            participante.Id = id;

            dbContext.Update(participante);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        //DELETE

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await dbContext.Participantes.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            dbContext.Remove(new Participantes()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
