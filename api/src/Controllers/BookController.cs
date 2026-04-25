using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.src.Data;
using api.src.Models.DTO;
using api.src.Models;
using api.src.Services;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;
        public BookController(IBookService service)
        {
            _service = service;
        }


        //get books api
        [HttpGet]

        public async Task<IActionResult> GetBook()
        {
            return Ok(await _service.GetBooksAsync());
        }

        //get books by id

        [HttpGet("{id:int}")]

        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Book ID must be greater than 0");
                }

                var book = await _service.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID{id} was not found");
                }
                return Ok(book);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while retireving book with ID {id}: {ex.Message}");
            }
        }

        //create book
        [HttpPost]

        public async Task<ActionResult<Book>> CreateBook(BookCreateDTO bookDTO)
        {
            try
            {
                if (bookDTO == null)
                {
                    return BadRequest("Book data is required");
                } 

                var book = await _service.CreateBookAsync(bookDTO);

                return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, book);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while creating the book : {ex.Message}");
            }
        }

        ////update the book details
        //[HttpPut("{id:int}")]

        //public async Task<ActionResult<Book>> UpdateBook(int id, BookUpdateDTO bookDTO)
        //{
        //    try
        //    {
        //        if (bookDTO == null)
        //        {
        //            return BadRequest("Book data is required");
        //        }

        //        if (id != bookDTO.Id)
        //        {
        //            return BadRequest("Book ID in URL does not match Book ID in the request body");
        //        }

        //        var existingBook = await _db.Books.FirstOrDefaultAsync(u => u.BookId == id);

        //        if (existingBook == null)
        //        {
        //            return NotFound($"Book with ID {id} was not found");
        //        }

        //        _mapper.Map(bookDTO, existingBook);
        //        await _db.SaveChangesAsync();

        //        return Ok(bookDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            $"An Error occured while updating the book : {ex.Message}");
        //    }
        //}

        ////Delete Book
        //[HttpDelete("{id:int}")]

        //public async Task<ActionResult<Book>> DeleteBook(int id)
        //{
        //    try
        //    { 

        //        var existingBook = await _db.Books.FirstOrDefaultAsync(u => u.BookId == id);

        //        if (existingBook == null)
        //        {
        //            return NotFound($"Book with ID {id} was not found");
        //        }

        //        _db.Books.Remove(existingBook);
        //        await _db.SaveChangesAsync();

        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            $"An Error occured while deleting the book : {ex.Message}");
        //    }
        //}

    }
}
