using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MongoDBTest.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Book name is required")]
        public string BookName { get; set; }
       
        public decimal Price { get; set; }
       
        public string Category { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public List<string> Authors { get; set; }
        
        
        public List<Author> AuthorList { get; set; }
    }
}
