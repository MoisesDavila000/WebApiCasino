using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.Entidades;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("IngresarRifa")]
    public class RelacionController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ParticipantesController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;

        public RelacionController(ApplicationDbContext dbContext, ILogger<ParticipantesController> logger, IWebHostEnvironment env, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.env = env;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ParticipantesRifasCartas>>> Get()
        {
            return await dbContext.ParticipantesRifasCartas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(ParticipantesRifasCartas participantesRifasCartas)
        {
            dbContext.Add(participantesRifasCartas);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(ParticipantesRifasCartas participantesRifasCartas, int id)
        {
            if (participantesRifasCartas.Id != id)
            {
                return BadRequest();
            }

            dbContext.Update(participantesRifasCartas);

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await dbContext.ParticipantesRifasCartas.AnyAsync(x => x.Id == id);
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
