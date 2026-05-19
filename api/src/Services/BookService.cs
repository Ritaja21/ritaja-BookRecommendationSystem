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
        private readonly ILogger<AuthService> _logger;
        public BookService(IBookRepository repo, IMapper mapper, ILogger<AuthService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
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
            _logger.LogInformation("Creating new book: {Title}",bookDTO.Title);

            var duplicateBook = await _repo.GetBookByNameAsync(bookDTO.Title);
            if (duplicateBook != null)
            {
                _logger.LogWarning("Duplicate book creation attempted: {Title}",bookDTO.Title);
                throw new Exception("A book with this title already exists");
            }
            Book book = _mapper.Map<Book>(bookDTO);

            var createdBook = await _repo.CreateBookAsync(book);

            _logger.LogInformation("Book created successfully: {Title}",createdBook.Title);

            return createdBook;
        }

        public async Task<Book?> UpdateBookAsync(int id, BookUpdateDTO bookDTO)
        {
            _logger.LogInformation("Updating book with ID: {BookId}",id);
            var existingBook = await _repo.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                _logger.LogWarning("Book update failed. Book not found: {BookId}",id);
                return null;
            }

            var duplicateBook = await _repo.GetBookByNameAsync(bookDTO.Title);
            if (duplicateBook != null && duplicateBook.BookId != id)
            {
                _logger.LogWarning("Duplicate title detected while updating book: {Title}",bookDTO.Title);
                throw new Exception("A book with this title already exists");
            }

            _mapper.Map(bookDTO, existingBook);
            
            existingBook.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Book updated successfully: {BookId}",existingBook.BookId);

            return await _repo.UpdateBookAsync(existingBook);

        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            _logger.LogInformation(
                "Deleting book with ID: {BookId}",
                id);

            var deleted = await _repo.DeleteBookAsync(id);

            if (!deleted)
            {
                _logger.LogWarning(
                    "Delete failed. Book not found: {BookId}",
                    id);

                return false;
            }

            _logger.LogInformation(
                "Book deleted successfully: {BookId}",
                id);

            return true;
        }

        public async Task<List<Book>> SearchBooksAsync(BookSearchDTO searchDTO)
        {
            return await _repo.SearchBooksAsync(searchDTO);
        }
     }
}
