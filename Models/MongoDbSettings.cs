namespace Backend.Models
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string SlidesCollectionName { get; set; } = string.Empty;

        public string BlogCollectionName { get; set; } = "Blogs";
    }
}
