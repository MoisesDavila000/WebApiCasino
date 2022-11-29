using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;
using WebApiCasino.Servicios;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("GanadoresRifa")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class GanadoresController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment env;
        private readonly IService service;

        public GanadoresController(ApplicationDbContext dbContext, IWebHostEnvironment env, IService service)
        {
            this.dbContext = dbContext;
            this.env = env;
            this.service = service;
        }

        [HttpGet("Seleccionar_Ganadores_Rifa/{id:int}")]
        public async Task<ActionResult<List<GETGanadorDTO>>> Get([FromRoute] int id)
        {
            var rifa = await dbContext.Rifas
                .Include(x => x.Premios)
                .Include(x => x.participantesRifasCartas)
                .ThenInclude(x => x.Participante)
                .FirstOrDefaultAsync(x => x.Id == id);

            var premios = await dbContext.Premios.Where(x => x.RifaId == id).ToListAsync();

            if (rifa == null)
            {
                return NotFound("No se encontro la rifa solicitada");
            }

            if (rifa.Premios.Count() == 0)
            {
                return BadRequest("No se encontro premios para la rifa solicitada");
            }

            if (rifa.participantesRifasCartas.Count() < rifa.Premios.Count())
            {
                return BadRequest("Existen menos participantes que premios");
            }

            List<CartasLoteMex> cartasUsadas = new List<CartasLoteMex>();
            var baraja = await dbContext.Cartas.ToListAsync();
            var relaciones = await dbContext.ParticipantesRifasCartas.Where(c => c.IdRifa == id.ToString()).ToListAsync();
            foreach (var i in relaciones)
            {
                int num = Int32.Parse(i.IdCarta);
                num = num - 1;
                cartasUsadas.Add(baraja[num]);
            }

            var numganadores = rifa.Premios.Count();

            List<CartasLoteMex> lista = new List<CartasLoteMex>();
            
            lista = service.EjecutarRandom(cartasUsadas, numganadores);

            List<Premios> listapremio = new List<Premios>();

            foreach (var i in rifa.Premios)
            {
                listapremio.Add(i);
            }

            var ganadores = new List<GETGanadorDTO>();

            foreach (var i in lista)
            {
                var participanteGanador = rifa.participantesRifasCartas.FirstOrDefault(x => (x.IdRifa == id.ToString()) && ( x.IdCarta == i.Id.ToString()));
                var participante = await dbContext.Participantes.FirstAsync(x => x.Id.ToString() == participanteGanador.IdParticipante);

                var premioGanador = listapremio.Last();
                listapremio.Remove(premioGanador);

                ganadores.Add(new GETGanadorDTO
                {
                    NombreRifa = rifa.Nombre,
                    Participante = new GETParticipantesDTO()
                    {
                        Id = participante.Id,
                        Nombre = participante.Nombre,
                        Telefono = participante.Telefono,
                        Email = participante.Email
                    },
                    Premio = new GETPremiosDTO()
                    {
                        Id = premioGanador.Id,
                        Nombre = premioGanador.Nombre,
                        Nivel = premioGanador.Nivel
                    }
                });
            }

            RegistroEnTxt escribir = new RegistroEnTxt(env);

            foreach (var i in ganadores)
            {
                escribir.EscribirExt("Rifa: " + i.NombreRifa + " | Ganador: " + i.Participante.Nombre + " | Premio: " + i.Premio.Nombre + " Nivel: " + i.Premio.Nivel);
            }

            return Ok(ganadores);


        }
    }

}
