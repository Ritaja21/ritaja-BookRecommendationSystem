using api.Models;
namespace api.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync();
    }
}
