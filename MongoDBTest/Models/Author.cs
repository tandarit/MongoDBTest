using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MongoDBTest.Models
{
    public class Author
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }

        /// <summary>
        /// The Author's first name.
        /// </summary>
        /// <example>Thomas</example>  
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        /// <summary>
        /// The Author's last name.
        /// </summary>
        /// <example>Mann</example>  
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        /// <summary>
        /// Is The Author's VIP.
        /// </summary>
        /// <example>false</example>  
        [Required(ErrorMessage = "isVIP is required")]
        public bool isVIP { get; set; }


        /// <summary>
        /// The Author's profit if he/she is a VIP.
        /// </summary>
        /// <example>13.54</example>  
        public double Profit { get; set; }
    }
}