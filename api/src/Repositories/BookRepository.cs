using api.src.Data;
using api.src.Models;
using Microsoft.EntityFrameworkCore;
namespace api.src.Repositories
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

        //create book
        public async Task<Book> CreateBookAsync(Book book)
        {
            await _db.Books.AddAsync(book);
            await _db.SaveChangesAsync();
            return book;
        }
    }
}
