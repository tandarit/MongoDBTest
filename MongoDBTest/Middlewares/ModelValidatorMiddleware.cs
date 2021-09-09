using Microsoft.AspNetCore.Http;
using MongoDBTest.Models;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MongoDBTest.Middlewares
{
    public class ModelValidatorMiddleware
    {
        private readonly RequestDelegate _next;
        

        public ModelValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
           
        }

        public async Task Invoke(HttpContext context)
        {
            string stringJson = "";
            var requestBody = context.Request.Body;
            
            using (var reader = new StreamReader(requestBody))
            {
                stringJson = await reader.ReadToEndAsync();
            }

            

            switch (context.Request.Path.Value)
            {
                
                case "/api/MongoDBTest/author":
                    Author author = JsonSerializer.Deserialize<Author>(stringJson);

                    foreach (PropertyInfo objProp in author.GetType().GetProperties())
                    {
                        if (!objProp.Name.Equals("Id"))
                        {
                            object val = objProp.GetValue(author, null);
                            if (val == null)
                            {
                                string errorJson = "{'message' : 'Author object's properties not definied', 'code' : 400}";
                                await WriteErrorResponse(context, errorJson);
                                await _next(context);
                            }
                        }
                    }
                    break;

                case "/api/MongoDBTest":
                    Book book = JsonSerializer.Deserialize<Book>(stringJson);

                    foreach (PropertyInfo objProp in book.GetType().GetProperties())
                    {
                        if (!objProp.Name.Equals("Id"))
                        {
                            object val = objProp.GetValue(book, null);
                            if (val == null)
                            {
                                string errorJson = "{'message' : 'Book object's properties not definied', 'code' : 400}";
                                await WriteErrorResponse(context, errorJson);
                                await _next(context);
                            }
                        }
                    }
                    break;
            }
            


            //Book inputBook = JsonSerializer.Deserialize<Book>(stringJson);

            //inputBook.Authors = new List<string>();
            //foreach (Author author in inputBook.AuthorList)
            //{
            //    var result = await _authorService.GetAuthorByFirstName(author.FirstName);
            //    if (result != null)
            //    {
            //        inputBook.Authors.Add(result.Id);
            //        if (author.isVIP)
            //        {
            //            inputBook.Price = Convert.ToDecimal(Convert.ToDouble(inputBook.Price) * ((result.Profit / 100) + 1));
            //        }
            //    }
            //    else
            //    {
            //        var objectId = await _authorService.CreateAuthor(author);
            //        inputBook.Authors.Add(objectId);
            //        if (author.isVIP)
            //        {
            //            inputBook.Price = Convert.ToDecimal(Convert.ToDouble(inputBook.Price) * ((author.Profit / 100) + 1));
            //        }
            //    }
            //}

            await _next(context);
        }

        private async Task WriteErrorResponse(HttpContext httpContext, string errorJsonMessage)
        {
            var response = httpContext.Response;
            byte[] bytesForBody = Encoding.ASCII.GetBytes(errorJsonMessage);

            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.ContentType = "application/json";
           
            await response.Body.WriteAsync(bytesForBody);
        }

    }
}
