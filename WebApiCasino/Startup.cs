using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using WebApiCasino.Entidades;
using WebApiCasino.Filtros;
using WebApiCasino.Middleware;
using WebApiCasino.Servicios;
using WebApiCasino.Utilidades;

namespace WebApiCasino
{
    public class Startup
    {   
        public Startup (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));

            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddHostedService<RegistroEnTxt>();
            //var config = new MapperConfiguration(cfg =>

            //    cfg.CreateMap<TSource, TDestination>()

            //);
            services.AddAutoMapper(typeof(Startup));

            //var mapperConfig = new MapperConfiguration(m =>

            //    m.AddProfile(new AutoMapperProfiles())

            //);
            //IMapper mapper = mapperConfig.CreateMapper();
            //services.AddSingleton(mapper);
            //services.AddMvc();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseResponseHttpMiddleware();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
                
        }
    }
}
