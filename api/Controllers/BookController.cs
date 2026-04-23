using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BookController(AppDbContext db)
        {
            _db = db;
        }


        //get books api
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Book>>> GetBook()
        {
            return Ok(await _db.Books.ToListAsync());
        }

        //get books by id

        [HttpGet("{id:int}")]

        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Movie ID must be greater than 0");
                }

                var book = await _db.Books.FirstOrDefaultAsync(u=> u.BookId == id);
                if(book == null)
                {
                    return NotFound($"Movie with ID{id} was not found");
                }
                return Ok(book);

            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"An error occured while retireving book with ID {id}: {ex.Message}");
            }
        }
    }
}
