using Gateway.App;


//Building of application and registering services.

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();


//Setuping middleware and start application.

app.SetupMiddleware()
   .Start();
