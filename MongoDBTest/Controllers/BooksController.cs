using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MongoDBTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDBTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            return Redirect("/api/MongoDBTest");
        }

        [HttpGet("{bookId}")]
        public IActionResult GetBookById(string bookId)
        {
            return RedirectToAction("GetBookById", new RouteValueDictionary(new { controller = "MongoDBTest", action = "GetBookById", Id = bookId }));
        }

        [HttpPost]
        public IActionResult CreateBook(Book book)
        {
            return RedirectToAction("CreateBook", new RouteValueDictionary(new { controller = "MongoDBTest", action = "CreateBook", Book = book }));
        }

        [HttpPut]
        public IActionResult CreateBook(string bookId, Book updatedBook)
        {
            return RedirectToAction("UpdateBook", new RouteValueDictionary(new { controller = "MongoDBTest", action = "UpdateBook", Id = bookId, Book = updatedBook }));
        }

        //cut out
        [HttpDelete]
        public IActionResult DeleteAllBooks()
        {
            return Redirect("/api/MongoDBTest");
        }

        [HttpDelete("{bookId}")]
        public IActionResult DeleteBookById(string bookId)
        {
            return RedirectToAction("DeleteBookById", new RouteValueDictionary(new { controller = "MongoDBTest", action = "DeleteBookById", Id = bookId }));
        }

        [HttpGet("author")]
        public IActionResult GetAllAuthors()
        {
            return Redirect("/api/MongoDBTest/author");
        }

        [HttpPost("author")]
        public IActionResult CreateAuthor(Author author)
        {
            return RedirectToAction("CreateAuthor", new RouteValueDictionary(new { controller = "MongoDBTest", action = "CreateAuthor", Author = author }));
        }

        [HttpDelete("author")]
        public IActionResult DeleteAll()
        {
            return Redirect("/api/MongoDBTest/author");
        }
    }
}
