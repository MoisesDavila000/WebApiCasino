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
    [Route("Premios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class PremiosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ParticipantesController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;

        public PremiosController(ApplicationDbContext dbContext, ILogger<ParticipantesController> logger, IWebHostEnvironment env, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.env = env;
            this.mapper = mapper;
        }

        [HttpGet("Premios_Por_Rifa/{id:int}", Name = "Premios_Por_Rifa")]
        public async Task<ActionResult<List<GETPremiosDTO>>> GetById([FromRoute] int id)
        {
            var existeRifa = await dbContext.Rifas.AnyAsync(rifaid => rifaid.Id == id);

            if (!existeRifa)
            {
                return NotFound();
            }

            var premios = await dbContext.Premios.Where(rifaDB => rifaDB.RifaId == id).ToListAsync();

            logger.LogInformation("Se obtiene el listado de los premios de una rifa.");
            return mapper.Map<List<GETPremiosDTO>>(premios);
        }

        [HttpPost("Crear_Premio/{rifaId:int}")]
        [ServiceFilter(typeof(FiltroRegistro))]
        public async Task<ActionResult> Post(int rifaId, CreacionPremioDTO creacionPremioDTO)
        {

            var existeRifa = await dbContext.Rifas.AnyAsync(rifaDB => rifaDB.Id == rifaId);

            if (!existeRifa)
            {
                return NotFound();
            }

            var existepremio = await dbContext.Premios.Where(c => c.RifaId == rifaId).ToListAsync();

            foreach (var i in existepremio)
            {
                if (i.Nombre == creacionPremioDTO.Nombre)
                {
                    return BadRequest("Ese premio ya existe en esta rifa");
                }

                if (i.Nivel == creacionPremioDTO.Nivel)
                {
                    return BadRequest("Ese nivel ya existe en otro premio de esta rifa");
                }
            }

            var premio = mapper.Map<Premios>(creacionPremioDTO);
            premio.RifaId = rifaId;
            dbContext.Add(premio);
            await dbContext.SaveChangesAsync();

            var getPremioDTO = mapper.Map<GETPremiosDTO>(premio);

            return CreatedAtRoute("Premios_Por_Rifa", new { id = premio.Id, RifaId = rifaId }, getPremioDTO);

        }

        [HttpPut("Modificar_Premio/{id:int}/{rifaId:int}")]
        [ServiceFilter(typeof(FiltroRegistro))]
        public async Task<ActionResult> Put(int id, int rifaId, CreacionPremioDTO creacionPremioDTO)
        {

            var existerRifa = await dbContext.Rifas.AnyAsync(x => x.Id == rifaId);

            if (!existerRifa)
            {
                return NotFound("La rifa no existe");
            }

            var existePremio = await dbContext.Premios.AnyAsync(x => x.Id == id);

            if (!existePremio)
            {
                return NotFound("El premio no existe");
            }

            var premio = mapper.Map<Premios>(creacionPremioDTO);
            premio.Id = id;
            premio.RifaId = rifaId;

            dbContext.Update(premio);
            await dbContext.SaveChangesAsync();
            return Ok();

        }

        [HttpDelete("Eliminar_Rifa/{id:int}/{rifaId:int}")]
        [ServiceFilter(typeof(FiltroRegistro))]
        public async Task<ActionResult> Delete(int id, int rifaId)
        {

            var existerRifa = await dbContext.Rifas.AnyAsync(x => x.Id == rifaId);

            if (!existerRifa)
            {
                return NotFound("La rifa no existe");
            }

            var existePremio = await dbContext.Premios.AnyAsync(x => x.Id == id);

            if (!existePremio)
            {
                return NotFound("El premio no existe");
            }

            var premio = await dbContext.Premios.Where(c => c.RifaId == rifaId).ToListAsync();

            foreach (var i in premio)
            {
                if(i.Id == id)
                {
                    dbContext.Remove(i);
                    await dbContext.SaveChangesAsync();
                }
            }

            return Ok();

        }

    }
}
