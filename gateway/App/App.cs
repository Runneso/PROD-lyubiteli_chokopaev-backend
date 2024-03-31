using Gateway.Internal.Interfaces;
using Gateway.Internal.Services;

namespace Gateway.App
{
	public static class App
	{
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {

            //Add controllers and swager gen.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();


            //Add all services.

            builder.Services.AddScoped<IUsersService, UsersService>();


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

