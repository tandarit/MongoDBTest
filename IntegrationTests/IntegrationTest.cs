using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDBTest;
using MongoDBTest.Models;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Text.Json;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTests
{
    public class IntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;
        private List<Book> _actualBookList;
        private List<string> _bookIdList;
        private Book _book;
       

        public IntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _actualBookList = new List<Book>();
            _bookIdList = new List<string>();
            _book = new Book()
            {
                BookName = "TestName",
                AuthorList = new List<Author>
                {
                    new Author()
                    {
                        FirstName = "Elek",
                        LastName = "Test",
                        isVIP = true,
                        Profit = 10.00
                    }
                },
                Category = "TestCategory",
                Price = 4096
            };

        }

        [Theory]
        [InlineData("https://localhost:5001/api/health")]
        [InlineData("https://localhost:5001/api/books")]
        [InlineData("https://localhost:5001/api/MongoDBTest")]
        public async Task Test1(string url)
        {
            // Arrange & Act
            var response = await _client.GetAsync(url);

            // Assert
            Xunit.Assert.True(response.EnsureSuccessStatusCode().StatusCode.Equals(HttpStatusCode.OK));
        }

        [Theory]
        [InlineData("https://localhost:5001/api/books")]
        [InlineData("https://localhost:5001/api/MongoDBTest")]
        public async Task Test2_Post(string url)
        {
            // Arrange & Act
            var jsonOption = new JsonSerializerOptions();
            jsonOption.IgnoreNullValues = true;
            jsonOption.WriteIndented = true;

            var jsonString = JsonSerializer.Serialize(_book, jsonOption);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");            

            var response = await _client.PostAsync(url, content);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.StatusCode==HttpStatusCode.Created);

            var bookId = await response.Content.ReadAsStringAsync();
            var returnedBook = await GetBookById(bookId);
            Assert.AreEqual(returnedBook.BookName, _book.BookName);

            //Clean up
            await DeleteBookById(bookId);
        }

        [Theory]
        [InlineData("https://localhost:5001/api/books")]
        [InlineData("https://localhost:5001/api/MongoDBTest")]
        public async Task Test3_Get(string url)
        {
            // Arrange & Act
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var response = await _client.GetAsync(url);
            var bodyStringContent = await response.Content.ReadAsStringAsync();
            _actualBookList = JsonSerializer.Deserialize<List<Book>>(bodyStringContent, options);

            //Assert
            if (_actualBookList.Count > 0) {                
                CollectionAssert.AllItemsAreInstancesOfType(_actualBookList, typeof(Book));
                CollectionAssert.AllItemsAreUnique(_actualBookList);
                foreach(var book in _actualBookList)
                {                    
                    Assert.IsFalse(string.IsNullOrEmpty(book.BookName));
                    Assert.IsFalse(string.IsNullOrEmpty(book.Category));
                    Assert.IsTrue(book.Price > 0);
                    Assert.IsTrue(book.AuthorList.Count > 0);
                }
            }
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("https://localhost:5001/api/books/")]
        [InlineData("https://localhost:5001/api/MongoDBTest/")]
        public async Task Test4_Put(string url)
        {
            // Arrange & Act
            var bookId = await PostABook(_book);

            var jsonOption = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                WriteIndented = true
            };

            _book.BookName = "New Test Book";
            _book.Price = 8192;

            var jsonString = JsonSerializer.Serialize(_book, jsonOption);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(url + bookId, content);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Accepted);

            await DeleteBookById(bookId);
        }

        [Theory]
        [InlineData("https://localhost:5001/api/books/")]
        [InlineData("https://localhost:5001/api/MongoDBTest/")]
        public async Task Test5_Delete(string url)
        {
            // Arrange & Act
            var bookId = await PostABook(_book);

            var response = await _client.DeleteAsync(url + bookId);

            //Assert
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            
        }

        private async Task<string> PostABook(Book book)
        {
            var jsonOption = new JsonSerializerOptions() {
                IgnoreNullValues = true,
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(book, jsonOption);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://localhost:5001/api/MongoDBTest", content);

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<Book> GetBookById(string id)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var response = await _client.GetAsync("https://localhost:5001/api/MongoDBTest/"+id);
            var bodyStringContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Book>(bodyStringContent, options);
           
        }

        private async Task<HttpStatusCode> DeleteBookById(string id)
        {
            var response = await _client.DeleteAsync("https://localhost:5001/api/MongoDBTest/" + id);
            return response.StatusCode;
        }

    }
}