using api.src.Data;
using api.src.Models;
using api.src.Models.DTO;
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

        //get book by title 
        public async Task<Book?> GetBookByNameAsync(string Title)
        {
            return await _db.Books.FirstOrDefaultAsync(u=> u.Title.ToLower() == Title.ToLower());
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

        public async Task<List<Book>> SearchBooksAsync(BookSearchDTO searchDTO)
        {
            var books = _db.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchDTO.Query))
            {
                books = books.Where(b =>
                    b.Title.ToLower().Contains(searchDTO.Query.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(searchDTO.Author))
            {
                books = books.Where(b =>
                    b.Author.ToLower().Contains(searchDTO.Author.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(searchDTO.Genre))
            {
                books = books.Where(b =>
                    b.Genre.ToLower().Contains(searchDTO.Genre.Trim().ToLower()));
            }

            return await books.ToListAsync();
        }
    }
}
