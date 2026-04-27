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
            return await _db.Books.FirstOrDefaultAsync(u => u.BookId == id);
        }

        //create book
        public async Task<Book> CreateBookAsync(Book book)
        {
            await _db.Books.AddAsync(book);
            await _db.SaveChangesAsync();
            return book;
        }

        //update book
        public async Task<Book?> UpdateBookAsync( Book book)
        {

           var existingBook =  await _db.Books.FirstOrDefaultAsync(u => u.BookId == book.BookId);

            if (existingBook == null)
            {
                return null;
            }
            _db.Entry(existingBook).CurrentValues.SetValues(book);
            await _db.SaveChangesAsync();

            return existingBook;
        }

        //delete book 
        public async Task<bool> DeleteBookAsync(int id)
        {
            var givenBook = await _db.Books.FirstOrDefaultAsync(u=> u.BookId == id);

            if (givenBook == null)
            {
                return false;
            }
            _db.Books.Remove(givenBook);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
