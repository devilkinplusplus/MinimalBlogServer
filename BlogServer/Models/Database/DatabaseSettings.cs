namespace BlogServer.Models.Database
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string BlogsCollectionName { get; set; } = null!;
    }
}
