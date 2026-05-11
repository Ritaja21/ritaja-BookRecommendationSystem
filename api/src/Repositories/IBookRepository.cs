using api.src.Models;
using api.src.Models.DTO;
namespace api.src.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book?> GetBookByNameAsync(string Title);

        Task<Book> CreateBookAsync(Book book);

        Task<Book?> UpdateBookAsync(Book book);

        Task<bool> DeleteBookAsync(int id);
        Task<List<Book>> SearchBooksAsync(BookSearchDTO searchDTO);
    }

}
