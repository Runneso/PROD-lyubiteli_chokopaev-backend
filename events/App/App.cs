using System.Reflection.Metadata.Ecma335;
using Events.Internal.Interafces;
using Events.Internal.Services;
using Events.Internal.Storage.Data;
using Events.Internal.Storage.Repositories;
using Events.Libs.Utils;
using Microsoft.EntityFrameworkCore;
using Grpc.Core;
using Microsoft.Extensions.Http;
using Grpc.Net.Client;
using MailClient;

namespace Events.App
{
	public static class App
	{
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {

            //Add controllers and swager gen.

            builder.Services.AddControllers()
                            .AddJsonOptions(options => 
                            {
                                options.JsonSerializerOptions.IgnoreNullValues = true;
                            });

            builder.Services.AddEndpointsApiExplorer();

            //Add all services.

            builder.Services.AddScoped<IEventsService, EventsService>();

            builder.Services.AddScoped<IEventsRepository, EventsRepository>();

            builder.Services.AddScoped<IMailSerivice, Internal.Services.MailService>();

            builder.Services.AddScoped<IEventsUsersRepository, EventsUsersRepository>();

            builder.Services.AddScoped<IOrganizerRepositoy, OrganizerRepository>();

            builder.Services.AddScoped<ITemplatesRepository, TemplatesRepository>();


            //Setup url.

            var port = Environment.GetEnvironmentVariable("EVENTS_PORT");

            port ??= "8080";

            builder.WebHost.UseUrls($"http://127.0.0.1:{port}");


            //Setup database connection.

            builder.Services.AddDbContext<DatabaseContext>(options => 
            {
                options.UseNpgsql(Parser.GetConnectionString());
            });

            //Iinit excel-files directory.

            Directory.CreateDirectory(Path.Combine(".", "Files"));


            //Setup logger.

            builder.Logging.ClearProviders().AddConsole();


            //Setup confiuration.

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.Services.AddSingleton(configuration);


            //Return builder.

            return builder;
        }

        public static WebApplication SetupMiddleware(this WebApplication app)
        {
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<DatabaseContext>();    
                context.Database.Migrate();

                Seed.Start(context);
            }


            return app;
        }

        public static void Start(this WebApplication app)
        {
            try
            {
                app.Run();
            }
            catch (Exception err)
            {
                app.Logger.LogError(err.Message);
                
                throw;
            }
        }

    }
}

