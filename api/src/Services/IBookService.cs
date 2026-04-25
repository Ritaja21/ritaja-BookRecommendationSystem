using api.src.Models;
using api.src.Models.DTO;
namespace api.src.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(BookCreateDTO bookDTO);
        Task<Book?> UpdateBookAsync(int id, BookUpdateDTO bookDTO);
    }
}
