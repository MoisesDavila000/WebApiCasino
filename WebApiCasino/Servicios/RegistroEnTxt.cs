namespace WebApiCasino.Servicios
{
    public class RegistroEnTxt : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private Timer timer;
        private readonly string nombreArchivo = "Casino.txt";


        public RegistroEnTxt(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Escribir("Se inicio la aplicacion.");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Escribir("Se cerro la aplicacion.");
            timer.Dispose();

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Escribir("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void Escribir(string msg)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";

            using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(msg); }
        }

    }
}
