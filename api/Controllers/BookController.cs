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
            return await _db.Books.ToListAsync();
        }

        //get books by id
    }
}
