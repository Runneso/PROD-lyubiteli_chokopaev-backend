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

            var port = Environment.GetEnvironmentVariable("GATEWAY_PORT");

            port ??= "8000";

            builder.WebHost.UseUrls($"http://127.0.0.1:{port}");

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
            app.MapGet("ping", () => "ok");
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
