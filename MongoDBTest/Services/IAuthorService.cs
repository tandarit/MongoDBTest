using MongoDB.Driver;
using MongoDBTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDBTest.Services
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAuthors();

        Task<Author> GetAuthorById(string id);

        Task<Author> GetAuthorByFirstName(string firstName);


        Task<string> CreateAuthor(Author authorIn);

       
        Task UpdateAuthor(string id, Author authorIn);   
       
        
        Task<DeleteResult> RemoveAuthorById(string id);

    }
}
