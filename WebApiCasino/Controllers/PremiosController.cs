using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.Entidades;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("Premios")]
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

        [HttpGet]
        public async Task<ActionResult<List<Premios>>> Get()
        {
            return await dbContext.Premios.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Premios premios)
        {
            dbContext.Add(premios);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Premios premios, int id)
        {
            if (premios.Id != id)
            {
                return BadRequest();
            }

            dbContext.Update(premios);

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await dbContext.Premios.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            dbContext.Remove(new Premios()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
