using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDBTest.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IMongoCollection<Author> _authors;
        private readonly IMongoDatabase _database;
        private readonly IMongoDBSettings _settings;

        public AuthorService(IMongoDBSettings settings)
        {
            _settings = settings;
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
            _authors = _database.GetCollection<Author>(settings.AuthorsCollectionName);
        }

        public async Task<string> CreateAuthor(Author authorIn)
        {
            await _authors.InsertOneAsync(authorIn);
            return authorIn.Id;
        }

        public async Task<Author> GetAuthorById(string id)
        {
            var author = await _authors.FindAsync(author => author.Id == id);
            return author.FirstOrDefault();
        }

        public async Task<Author> GetAuthorByFirstName(string firstName)
        {
            var author = await _authors.FindAsync(author => author.FirstName == firstName);
            return author.FirstOrDefault();
        }

        public async Task<List<Author>> GetAuthors()
        {
            var author = await _authors.FindAsync(a => true);
            return author.ToList();
        }

        public async Task RemoveAuthor(Author authorIn)
        {
            await _authors.DeleteOneAsync(authorIn => authorIn.Id == authorIn.Id);
        }

        public async Task RemoveAuthorById(string id)
        {
            await _authors.DeleteOneAsync(a => a.Id == id);
        }

        public async Task UpdateAuthor(string id, Author authorIn)
        {
            await _authors.ReplaceOneAsync(a => a.Id == id, authorIn);
        }

        public async Task RemoveAllAuthors()
        {
            await _authors.DeleteManyAsync<Author>(a=>a.Id.Length>1);
        }

        public async Task<List<Author>> FindAuthors(string findingString)
        {
            //check the string



            return new List<Author>();
        }
    }
}
