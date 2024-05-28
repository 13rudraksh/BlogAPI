namespace BlogAPI.Persistence.Services
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string? ConnectionString { get; set; }
        public string? Database { get; set; }
    }

    public interface IDatabaseSettings
    {
        string? ConnectionString { get; set; }
        string? Database { get; set; }
    }

}
