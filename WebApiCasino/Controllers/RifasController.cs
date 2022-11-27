using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.Entidades;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("Rifas")]
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

        [HttpGet]
        public async Task<ActionResult<List<Rifas>>> Get()
        {
            return await dbContext.Rifas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Rifas rifa)
        {
            dbContext.Add(rifa);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Rifas rifa, int id)
        {
            if (rifa.Id != id)
            {
                return BadRequest();
            }

            dbContext.Update(rifa);

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await dbContext.Rifas.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            dbContext.Remove(new Rifas()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
