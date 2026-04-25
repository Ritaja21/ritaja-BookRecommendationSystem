using AutoMapper;
using api.src.Models;
using api.src.Models.DTO;
using api.src.Repositories;

namespace api.src.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;
        private readonly IMapper _mapper;
        public BookService(IBookRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            return await _repo.GetBooksAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _repo.GetBookByIdAsync(id);
        }

        public async Task<Book> CreateBookAsync(BookCreateDTO bookDTO)
        {
            Book book = _mapper.Map<Book>(bookDTO);

                 return  await _repo.CreateBookAsync(book);
        }
    }
}
