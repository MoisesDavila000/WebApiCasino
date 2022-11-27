using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.Entidades;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("Cartas")]
    public class CartasController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ParticipantesController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;


        public CartasController(ApplicationDbContext dbContext, ILogger<ParticipantesController> logger, IWebHostEnvironment env, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.env = env;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<CartasLoteMex>>> Get()
        {
            return await dbContext.Cartas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(CartasLoteMex cartas)
        {
            dbContext.Add(cartas);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(CartasLoteMex cartas, int id)
        {
            if (cartas.Id != id)
            {
                return BadRequest();
            }

            dbContext.Update(cartas);

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await dbContext.Cartas.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            dbContext.Remove(new CartasLoteMex()
            {
                Id = id
            });

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
