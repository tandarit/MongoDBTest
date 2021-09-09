using System.ComponentModel.DataAnnotations;

namespace MongoDBTest.Models
{
    public class MongoDBSettings : IMongoDBSettings
    {
        [Required(ErrorMessage = "ConnectionString is required in the configure file.")]
        public string ConnectionString { get; set; }
        [Required(ErrorMessage = "Database name is required in the configure file.")]
        public string DatabaseName { get; set; }
        [Required(ErrorMessage = "Collection name is required in the configure file.")]
        public string BooksCollectionName { get; set; }
        [Required(ErrorMessage = "Collection name is required in the configure file.")]
        public string AuthorsCollectionName { get; set; }
        [Required(ErrorMessage = "Collection name is required in the configure file.")]
        public string VIPAuthorsCollectionName { get; set; }
    }
}
