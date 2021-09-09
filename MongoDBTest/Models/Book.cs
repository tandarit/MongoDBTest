using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MongoDBTest.Models
{
    public class Book
    {
        /// <summary>
        /// The Book's id.
        /// </summary>
        /// <example>fksjfhweiofh84irziu</example>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]        
        public string Id { get; set; }

        /// <summary>
        /// The Book's name.
        /// </summary>
        /// <example>Mario és a varázsló</example>
        [Required(ErrorMessage = "Book name is required")]
        public string BookName { get; set; }

        /// <summary>
        /// The Book's price.
        /// </summary>
        /// <example>9500</example>
        public decimal Price { get; set; }

        /// <summary>
        /// The Book's category.
        /// </summary>
        /// <example>Drama</example>
        public string Category { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public List<string> Authors { get; set; }

        
        public List<Author> AuthorList { get; set; }
    }
}
