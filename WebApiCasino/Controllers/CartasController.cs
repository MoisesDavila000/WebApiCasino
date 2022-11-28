using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;
using WebApiCasino.Filtros;

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

        [HttpGet("Cartas_Por_Rifa/{id:int}/{nombreRifa}")]
        public async Task<ActionResult<List<GETCartasDTO>>> GetById(int id, string nombreRifa)
        {

            var existe = await dbContext.Rifas.AnyAsync(x => x.Id == id && x.Nombre == nombreRifa);

            if (!existe)
            {
                return BadRequest("No existe la rifa a la que se desea acceder");
            }
            
            var cartas = await dbContext.Cartas.ToListAsync();
            var borrar = await dbContext.Cartas.ToListAsync();
            var relaciones = await dbContext.ParticipantesRifasCartas.Where(c => c.IdRifa == id.ToString()).ToListAsync();
            foreach (var i in relaciones)
            {
                int num = Int32.Parse(i.IdCarta);
                num = num - 1;
                cartas.Remove(borrar[num]);
            }
            logger.LogInformation("Se obtiene el listado de participantes.");
            return mapper.Map<List<GETCartasDTO>>(cartas);
        }

    }
}
