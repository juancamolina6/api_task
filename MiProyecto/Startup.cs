using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MiProyecto.Conexion;
using MiProyecto.Interfaces;
using MiProyecto.Services;
using MiProyecto.Conexion;


namespace MiProyecto
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConexionService>(new ConexionService("Host=127.0.0.1;Username=postgres;Password=Juanca1993-;Database=list_tasks;Port=5432"));
            services.AddScoped<IDatabaseService, DatabaseService>(); // Registra IDatabaseService con DatabaseService
            services.AddControllers();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
             // Crear las tablas al iniciar la aplicación
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var databaseService = serviceScope.ServiceProvider.GetRequiredService<IDatabaseService>();
                databaseService.CreateTables();
            }
        }
    }
}
