using Microsoft.AspNetCore.Mvc.Filters;
using WebApiCasino.Servicios;

namespace WebApiCasino.Filtros
{
    public class FiltroRegistro : IActionFilter
    {
        private readonly ILogger<FiltroRegistro> log;
        private readonly IWebHostEnvironment env;

        public FiltroRegistro(ILogger<FiltroRegistro> log, IWebHostEnvironment env)
        {
            this.log = log;
            this.env = env;

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            log.LogInformation("Se realizara un registro nuevo.");
            RegistroEnTxt escribir = new RegistroEnTxt(env);
            escribir.EscribirExt("Se realizara un registro nuevo.");

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            log.LogInformation("Se realizo un registro de manera exitosa.");
            RegistroEnTxt escribir = new RegistroEnTxt(env);
            escribir.EscribirExt("Se realizo un registro de manera exitosa.");
        }

    }
}
