using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
namespace api.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _db;
        public BookRepository(AppDbContext db)
        {
            _db = db;
        }

        //get all books
        public async Task<List<Book>> GetBooksAsync()
        {
             return await _db.Books.ToListAsync();
        }

        //get book by id
        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _db.Books.FirstOrDefaultAsync(u=>u.BookId ==id);
        }
    }
}
