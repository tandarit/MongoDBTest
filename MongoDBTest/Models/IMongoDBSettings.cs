namespace MongoDBTest.Models
{
    public interface IMongoDBSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string BooksCollectionName { get; set; }
        string AuthorsCollectionName { get; set; }
    }
}