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
        public async Task Test2(string url)
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
        }

        [Theory]
        [InlineData("https://localhost:5001/api/books")]
        [InlineData("https://localhost:5001/api/MongoDBTest")]
        public async Task Test3(string url)
        {
            // Arrange & Act
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var response = await _client.GetAsync(url);
            var bodyStringContent = await response.Content.ReadAsStringAsync();
            _actualBookList = JsonSerializer.Deserialize<List<Book>>(bodyStringContent, options);


            Assert.IsTrue(response.StatusCode==HttpStatusCode.OK);
            CollectionAssert.AllItemsAreInstancesOfType(_actualBookList, typeof(Book));
            CollectionAssert.AllItemsAreUnique(_actualBookList);
            var resultList = _actualBookList.FindAll(l => l.BookName == _book.BookName);
            var resultBookIdList = from result in resultList
                                       select result.Id;
            _bookIdList = resultBookIdList.ToList(); //save for later
            
            Assert.IsTrue(resultList.Count == 2);
        }

        //[Theory]
        //[InlineData("https://localhost:5001/api/books/")]
        //[InlineData("https://localhost:5001/api/MongoDBTest/")]
        //public async Task Test4(string url)
        //{          

        //    // Arrange & Act
        //    var jsonOption = new JsonSerializerOptions();
        //    jsonOption.IgnoreNullValues = true;
        //    jsonOption.WriteIndented = true;

        //    _book.BookName = "New Test Book";
        //    _book.Price = 8192;

        //    var jsonString = JsonSerializer.Serialize(_book, jsonOption);
        //    HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");



        //    var response = await _client.PutAsync(url, content);

        //    Assert.IsTrue(response.StatusCode == HttpStatusCode.Accepted);
           


        //}

        private async Task<string> GetBookId(string url)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var response = await _client.GetAsync(url);
            var bodyStringContent = await response.Content.ReadAsStringAsync();
            _actualBookList = JsonSerializer.Deserialize<List<Book>>(bodyStringContent, options);

            var resultList = _actualBookList.FindAll(l => l.BookName == _book.BookName);
            var resultBookIdList = from result in resultList
                                   select result.Id;
            _bookIdList = resultBookIdList.ToList();
            return "";
        }

        //[Theory]
        //[InlineData("https://localhost:5001/api/books/")]
        //[InlineData("https://localhost:5001/api/MongoDBTest/")]
        //public async Task Test5_Delete(string url)
        //{
        //    // Arrange & Act
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true,
        //    };
        //    var response = await _client.GetAsync(url);
        //    var bodyStringContent = await response.Content.ReadAsStringAsync();
        //    _actualBookList = JsonSerializer.Deserialize<List<Book>>(bodyStringContent, options);


        //    CollectionAssert.AllItemsAreInstancesOfType(_actualBookList, typeof(Book));
        //    CollectionAssert.AllItemsAreUnique(_actualBookList);
        //    var resultList = _actualBookList.FindAll(l => l.BookName == _book.BookName && l.Category == _book.Category && l.Price == _book.Price);
        //    Assert.IsTrue(resultList.Count == 2);
        //}
    }
}