
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
        private readonly IAuthorService _authorService;

        public BookService(IMongoDBSettings settings, IAuthorService authorService)
        {
            _settings = settings;
            _authorService = authorService;

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
            bookIn.Authors = new List<string>();
            bookIn.Id = id;
            for(int i=0; i<bookIn.AuthorList.Count; i++)
            {
                Author searchedAuthor = await _authorService.GetAuthorByName(bookIn.AuthorList.ElementAt<Author>(i).FirstName, bookIn.AuthorList.ElementAt<Author>(i).LastName);
                if(searchedAuthor == null)
                {                    
                    bookIn.AuthorList.ElementAt<Author>(i).Id = await _authorService.CreateAuthor(bookIn.AuthorList.ElementAt<Author>(i));
                    bookIn.Authors.Add(bookIn.AuthorList.ElementAt<Author>(i).Id);
                }
                else
                {
                    bookIn.AuthorList.ElementAt<Author>(i).Id = searchedAuthor.Id;
                    bookIn.Authors.Add(searchedAuthor.Id);
                }
            }

            var updateResult = await _books.ReplaceOneAsync(book => book.Id == id, bookIn);
        }

        public async Task RemoveAllBooks()
        {
            await _books.DeleteManyAsync<Book>(a => a.Id.Length > 1);
        }
    }
}