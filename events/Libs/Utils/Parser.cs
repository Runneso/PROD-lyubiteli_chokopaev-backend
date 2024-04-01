namespace Events.Libs.Utils
{
    public static class Parser
    {
        public static string GetConnectionString()
        {
            var HOST = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            var PORT = Environment.GetEnvironmentVariable("POSTGRES_PORT");
<<<<<<< HEAD
            var USER = Environment.GetEnvironmentVariable("POSTGRES_USER");
=======
            var USERNAME = Environment.GetEnvironmentVariable("POSTGRES_USER");
>>>>>>> 7fbd5a1 (Update events)
            var PASSWORD = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var DATABASE = Environment.GetEnvironmentVariable("POSTGRES_DB");

            string connection = $"Host={HOST};Port={PORT};Database={DATABASE};Username={USER};Password={PASSWORD}";

            return "Host=localhost;Port=5432;Database=cloud_storage_db;Username=admin;Password=root";
            return connection;
        }
    }
}