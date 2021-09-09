﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDBTest.Models;
using MongoDBTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
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
        public async Task<ActionResult<Book>> GetBookById(string id)
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
        public async Task<IActionResult> CreateBook(Book book)
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
            return Ok(book);
        }

        /// <summary>
        /// Modify a book by id
        /// </summary>
        [HttpPut("{id}", Name ="UpdateBook")]
        public async Task<IActionResult> UpdateBook(string id, Book updatedBook)
        {           
            var book = await _bookService.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            await _bookService.UpdateBook(id, updatedBook);
            return NoContent();
        }

        /// <summary>
        /// Delete all books from databases
        /// </summary>
        [HttpDelete(Name = "DeleteAllBooks")]
        public async Task<IActionResult> DeleteAllBooks()
        {
            await _bookService.RemoveAllBooks();
            return NoContent();
        }

        /// <summary>
        /// Delete a books by rid
        /// </summary>
        [HttpDelete("{id}", Name = "DeleteBookById")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _bookService.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            await _bookService.RemoveBook(book);
            return NoContent();
        }

        /// <summary>
        /// Delete author by id from databases
        /// </summary>
        /// <returns></returns>
        [HttpDelete("author", Name ="DeleteAuthor")]
        public async Task<IActionResult> DeleteAll()
        {
            await _authorService.RemoveAllAuthors();
            return NoContent();
        }

        /// <summary>
        /// Get author from databases
        /// </summary>
        /// <returns></returns>
        [HttpGet("author", Name = "GetAllAuthors")]
        public async Task<ActionResult<List<Author>>> GetAllAuthors()
        {
            var authorList = await _authorService.GetAuthors();
            return Ok(authorList);
        }

        /// <summary>
        /// Create author 
        /// </summary>
        /// <returns></returns>
        [HttpPost("author", Name = "CreateAuthor")]
        public async Task<IActionResult> CreateAuthor(Author author)
        {
            var resultId = await _authorService.CreateAuthor(author);
            _logger.LogInformation($"The new author id: {resultId}.");
            return Ok(author);
        }
    }
}
