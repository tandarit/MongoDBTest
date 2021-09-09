
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDBTest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDBTest.Services
{
    public class BookService : IBookService
    {        
        private readonly IMongoCollection<Book> _books;
        private readonly IMongoDBSettings _settings;

        public BookService(IMongoDBSettings settings)
        {
            _settings = settings;

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _books = database.GetCollection<Book>(settings.BooksCollectionName);
        }

        public async Task CreateBook(Book bookIn)
        {
            await _books.InsertOneAsync(bookIn);
        }

        public async Task<Book> GetBook(string id)
        {
            var book = await _books.FindAsync(book => book.Id == id);
            return book.FirstOrDefault();
        }

        public async Task<List<Book>> GetBooks()
        {
            var books = await _books.FindAsync(book => true);
            return books.ToList();
        }

        public async Task RemoveBook(Book bookIn)
        {
            await _books.DeleteOneAsync(book => book.Id == bookIn.Id);
        }

        public async Task RemoveBookById(string id)
        {
            await _books.DeleteOneAsync(book => book.Id == id);
        }

        public async Task UpdateBook(string id, Book bookIn)
        {
            await _books.ReplaceOneAsync(book => book.Id == id, bookIn);
        }

        public async Task RemoveAllBooks()
        {
            await _books.DeleteManyAsync<Book>(a => a.Id.Length > 1);
        }
    }
}