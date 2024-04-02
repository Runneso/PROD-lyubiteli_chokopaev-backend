namespace Events.Libs.Utils
{
    public static class Parser
    {
        public static string GetConnectionString()
        {
            var HOST = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            var PORT = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            var USER = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var PASSWORD = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var DATABASE = Environment.GetEnvironmentVariable("POSTGRES_DB");

            string connection = $"Host={HOST};Port={PORT};Database={DATABASE};Username={USER};Password={PASSWORD}";
            
            return connection;
        }
    }
}