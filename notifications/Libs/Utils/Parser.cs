namespace Notifications.Libs.Utils 
{
    public static class Parser
    {
        public static string GetConnectionString()
        {
            var HOST = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            var PORT = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            var USERNAME = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
            var PASSWORD = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var DATABASE = Environment.GetEnvironmentVariable("POSTGRES_DATABASE");

            string connection = $"Host={HOST};Port={PORT};Database={DATABASE};Username={USERNAME};Password={PASSWORD}";

            return "Host=localhost;Port=5432;Database=cloud_storage_db;Username=admin;Password=root";
            return connection;
        }
    }
}