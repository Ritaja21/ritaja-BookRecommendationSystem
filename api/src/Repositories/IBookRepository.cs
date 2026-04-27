using api.src.Models;
namespace api.src.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);

        Task<Book> CreateBookAsync(Book book);

        Task<Book?> UpdateBookAsync(Book book);

        Task<bool> DeleteBookAsync(int id);
    }

}
