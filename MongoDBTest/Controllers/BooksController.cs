using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MongoDBTest.Models;
using MongoDBTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDBTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : MongoDBTestController
    {
        public BooksController(IAuthorService authorService, IBookService bookService, ILogger<MongoDBTestController> logger) :base(authorService, bookService, logger)
        {

        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            return await base.GetAllBooks();
            //return Redirect("/api/MongoDBTest");
        }

        [HttpGet("{id}")]
        public override async Task<ActionResult<Book>> GetBookById(string id)
        {
            return await base.GetBookById(id);
            //    return RedirectToAction("GetBookById", new RouteValueDictionary(new { controller = "MongoDBTest", action = "GetBookById", Id = bookId }));
        }

        [HttpPost]
        public override async Task<IActionResult> CreateBook(Book book)
        {
            return await base.CreateBook(book);
            //return RedirectToAction("CreateBook", "MongoDBTest", new { Book = book });
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> UpdateBook([FromRoute]string id, [FromBody]Book updatedBook)
        {
            return await base.UpdateBook(id, updatedBook);
            //return RedirectToAction("UpdateBook", new RouteValueDictionary(new { controller = "MongoDBTest", action = "UpdateBook", Id = bookId, Book = updatedBook }));
        }

       
        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteBookById([FromRoute] string id)
        {
            return await base.DeleteBookById(id);
        }

        [HttpGet("author")]
        public override async Task<ActionResult<List<Author>>> GetAllAuthors()
        {
            return await base.GetAllAuthors();
        }

        [HttpPost("author")]
        public override async Task<IActionResult> CreateAuthor([FromBody]Author author)
        {
            return await base.CreateAuthor(author);
        }

        [HttpPut("author/{id}")]
        public override async Task<IActionResult> UpdateAuthor([FromRoute] string id, [FromBody] Author author)
        {
            return await base.UpdateAuthor(id, author);
        }

        [HttpDelete("author/{id}")]
        public override async Task<IActionResult> DeleteAuthor([FromRoute] string id)
        {
            return await base.DeleteAuthor(id);
        }
    }
}
