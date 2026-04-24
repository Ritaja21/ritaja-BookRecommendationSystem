using api.Repositories;
using api.Models;
namespace api.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;
        public BookService(IBookRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            return await _repo.GetBooksAsync();
        }
    }
}
