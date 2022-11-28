using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;
using WebApiCasino.Filtros;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("Rifas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class RifasController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ParticipantesController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;

        public RifasController(ApplicationDbContext dbContext, ILogger<ParticipantesController> logger, IWebHostEnvironment env, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.env = env;
            this.mapper = mapper;
        }

        [HttpGet("Obtener_Premios_De_Rifa/Rifa/{id:int}", Name = "GetPremiosRifa")]
        public async Task<ActionResult<RifaconPremiosDTO>> GetPremiosRifaId([FromRoute] int id)
        {
            var rifa = await dbContext.Rifas
                .Include(x => x.Premios)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (rifa == null)
            {
                return NotFound("No se encontro una rifa con el Id ingresado");
            }

            logger.LogInformation("Se obtiene el listado de premios de una rifa.");
            return mapper.Map<RifaconPremiosDTO>(rifa);

        }

        [HttpPost("CrearUnaRifa")]
        [ServiceFilter(typeof(FiltroRegistro))]
        public async Task<ActionResult> Post(CreacionRifaDTO rifaCreacionDTO)
        {
            var existe = await dbContext.Rifas.AnyAsync(x => x.Nombre == rifaCreacionDTO.Nombre);

            if (existe)
            {
                return BadRequest("Ya existe una rifa con este nombre");
            }

            var nuevoElemento = mapper.Map<Rifas>(rifaCreacionDTO);

            dbContext.Add(nuevoElemento);


            await dbContext.SaveChangesAsync();

            var rifaDTO = mapper.Map<GETRifasDTO>(nuevoElemento);

            return CreatedAtRoute("GetPremiosRifa", new { id = nuevoElemento.Id }, rifaDTO);

        }

        [HttpPut("ModificarRifa/{id:int}")]
        [ServiceFilter(typeof(FiltroRegistro))]
        public async Task<ActionResult> Put(CreacionRifaDTO creacionRifaDTO, int id)
        {
            var existeRifa = await dbContext.Rifas.AnyAsync(x => x.Id == id);

            if (!existeRifa)
            {
                return BadRequest("La rifa no existe");
            }

            var rifa = mapper.Map<Rifas>(creacionRifaDTO);
            rifa.Id = id;

            dbContext.Update(rifa);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("EliminarRifa/{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var existe = await dbContext.Rifas.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound("No se encontro una rifa con ese id");
            }

            var relaciones = await dbContext.ParticipantesRifasCartas.Where(c => c.IdRifa == id.ToString()).ToListAsync();


            foreach (var i in relaciones)
            {
                dbContext.Remove(i);
                await dbContext.SaveChangesAsync();
            }

            var rifa = await dbContext.Rifas.Include(x => x.Premios).FirstAsync(x => x.Id == id);
            dbContext.Remove(rifa);

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
