using Gateway.Internal.Interfaces;
using Gateway.Internal.Services;

namespace Gateway.App
{
	public static class App
	{
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {

            //Add controllers and swager gen.

            builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();


            //Add all services.

            builder.Services.AddScoped<IUsersService, UsersService>();

            builder.Services.AddScoped<IFilesService, FilessService>();

            builder.Services.AddScoped<ITeamsService, TeamsService>();

            builder.Services.AddScoped<IEventsService, EventsService>();

            //Set port.

            var port = Environment.GetEnvironmentVariable("GATEWAY_PORT");

            port ??= "80";

            builder.WebHost.UseUrls($"http://+:{port}");


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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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
