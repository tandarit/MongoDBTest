using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDBTest.Models;
using MongoDBTest.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDBTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MongoDBTestController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly ILogger _logger;

        public MongoDBTestController(IAuthorService authorService, IBookService bookService, ILogger<MongoDBTestController> logger)
        {
            _authorService = authorService;
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Get all books from databases
        /// </summary>
        /// <returns>JSON list of the books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            var books = await _bookService.GetBooks();
            
            //author documents readout
            foreach(Book book in books)
            {
                book.AuthorList = new List<Author>();
                foreach(string authorId in book.Authors)
                {
                    var resultAuthor = await _authorService.GetAuthorById(authorId);
                    book.AuthorList.Add(resultAuthor);
                }               
            }

            return Ok(books);
        }


        /// <summary>
        /// Get books by id from databases
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetBookById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<Book>> GetBookById(string id)
        {
            var book = await _bookService.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }

            if (book.Authors.Count > 0)
            {
                var tempList = new List<Author>();
                foreach (var authorId in book.Authors)
                {
                    var author = await _authorService.GetAuthorById(authorId);
                    if (author != null)
                        tempList.Add(author);
                }
                book.AuthorList = tempList;
            }

            return Ok(book);
        }


        /// <summary>
        /// Create a book 
        /// </summary>    
        [HttpPost(Name = "CreateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> CreateBook(Book book)
        {
            if (book.AuthorList.Count > 0)
            {
                book.Authors = new List<string>();
                foreach (Author author in book.AuthorList)
                {                    
                    var result = await _authorService.GetAuthorByFirstName(author.FirstName);
                    if (result != null)
                    {
                        book.Authors.Add(result.Id);
                        if (author.isVIP)
                        {
                            book.Price = Convert.ToDecimal(Convert.ToDouble(book.Price) * ((result.Profit/100)+1));
                        }
                    }
                    else
                    {
                        var objectId = await _authorService.CreateAuthor(author);
                        book.Authors.Add(objectId);
                        if (author.isVIP) { 
                            book.Price = Convert.ToDecimal(Convert.ToDouble(book.Price) * ((author.Profit/100)+1));
                        }
                    }
                }
            }

            await _bookService.CreateBook(book);
            return new ObjectResult(book.Id) { StatusCode = StatusCodes.Status201Created }; 
        }

        /// <summary>
        /// Modify a book by id
        /// </summary>
        [HttpPut("{id}", Name ="UpdateBook")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> UpdateBook(string id, Book updatedBook)
        {           
            var book = await _bookService.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            await _bookService.UpdateBook(id, updatedBook);
            return new ObjectResult(book) { StatusCode = StatusCodes.Status202Accepted };
        }

        /// <summary>
        /// Delete all books from databases
        /// </summary>
        [HttpDelete(Name = "DeleteAllBooks")]
        public virtual async Task<IActionResult> DeleteAllBooks()
        {
            await _bookService.RemoveAllBooks();
            return NoContent();
        }

        /// <summary>
        /// Delete a books by rid
        /// </summary>
        [HttpDelete("{id}", Name = "DeleteBookById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> DeleteBookById(string id)
        {
            var book = await _bookService.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            await _bookService.RemoveBook(book);
            return Ok();
        }

        /// <summary>
        /// Delete author by id from databases
        /// </summary>
        /// <returns></returns>
        [HttpDelete("author/{id}", Name = "DeleteAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> DeleteAuthor([FromRoute] string id)
        {
            var deleteResult = await _authorService.RemoveAuthorById(id);
            if(deleteResult.IsAcknowledged && deleteResult.DeletedCount == 1)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get author from databases
        /// </summary>
        /// <returns></returns>
        [HttpGet("author", Name = "GetAllAuthors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<List<Author>>> GetAllAuthors()
        {
            var authorList = await _authorService.GetAuthors();
            return Ok(authorList);
        }

        /// <summary>
        /// Create author 
        /// </summary>
        /// <returns>Id of the Author</returns>
        [HttpPost("author", Name = "CreateAuthor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> CreateAuthor(Author author)
        {
            var resultId = await _authorService.CreateAuthor(author);
            if(resultId == null)
            {
                return BadRequest();
            }
            _logger.LogInformation($"The new author id: {resultId}.");
            return new ObjectResult(resultId) { StatusCode = StatusCodes.Status201Created };
        }

        /// <summary>
        /// Modify a author.
        /// </summary>
        [HttpPut("author/{id}", Name = "UpdateAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> UpdateAuthor([FromRoute]string id, [FromBody]Author updateAuthor)
        {
            var author = await _authorService.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }
            await _authorService.UpdateAuthor(id, updateAuthor);
            return Ok(author);
        }
    }
}
