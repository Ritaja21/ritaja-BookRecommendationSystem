using api.src.Models.DTO;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace BookRecommendation.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepo;

        private readonly Mock<IMapper> _mockMapper;

        private readonly Mock<ILogger<AuthService>> _mockLogger;

        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepo = new Mock<IBookRepository>();

            _mockMapper = new Mock<IMapper>();

            _mockLogger = new Mock<ILogger<AuthService>>();

            _bookService = new BookService(
                _mockBookRepo.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var book = new Book
            {
                BookId = 1,
                Title = "Harry Potter",
                Author = "JK Rowling"
            };

            _mockBookRepo
                .Setup(repo => repo.GetBookByIdAsync(1))
                .ReturnsAsync(book);

            // Act
            var result = await _bookService.GetBookByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Harry Potter");
            result.Author.Should().Be("JK Rowling");
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookRepo
                .Setup(repo => repo.GetBookByIdAsync(99))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _bookService.GetBookByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateBookAsync_ShouldCreateBook_WhenBookDoesNotExist()
        {
            // Arrange
            var bookDto = new BookCreateDTO
            {
                Title = "Dance of the Dragons",
                Author = "George R Martin"
            };

            var mappedBook = new Book
            {
                Title = "Dance of the Dragons",
                Author = "George R Martin"
            };

            var createdBook = new Book
            {
                BookId = 1,
                Title = "Dance of the Dragons",
                Author = "George R Martin"
            };

            _mockBookRepo
                .Setup(repo => repo.GetBookByNameAsync(bookDto.Title))
                .ReturnsAsync((Book?)null);

            _mockMapper
                .Setup(mapper => mapper.Map<Book>(bookDto))
                .Returns(mappedBook);

            _mockBookRepo
                .Setup(repo => repo.CreateBookAsync(mappedBook))
                .ReturnsAsync(createdBook);

            // Act
            var result = await _bookService.CreateBookAsync(bookDto);

            // Assert
            result.Should().NotBeNull();

            result.Title.Should().Be("Dance of the Dragons");

            result.BookId.Should().Be(1);
        }
        [Fact]
        public async Task CreateBookAsync_ShouldThrowException_WhenBookAlreadyExists()
        {
            // Arrange
            var bookDto = new BookCreateDTO
            {
                Title = "Dance of the Dragons",
                Author = "George R Martin"
            };

            var existingBook = new Book
            {
                BookId = 1,
                Title = "Dance of the Dragons",
                Author = "George R Martin"
            };

            _mockBookRepo
                .Setup(repo => repo.GetBookByNameAsync(bookDto.Title))
                .ReturnsAsync(existingBook);

            // Act
            Func<Task> act = async () =>
                await _bookService.CreateBookAsync(bookDto);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("A book with this title already exists");
        }
        [Fact]
        public async Task DeleteBookAsync_ShouldReturnTrue_WhenDeleteSucceeds()
        {
            // Arrange
            _mockBookRepo
                .Setup(repo => repo.DeleteBookAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _bookService.DeleteBookAsync(1);

            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeleteBookAsync_ShouldReturnFalse_WhenBookNotFound()
        {
            // Arrange
            _mockBookRepo
                .Setup(repo => repo.DeleteBookAsync(99))
                .ReturnsAsync(false);

            // Act
            var result = await _bookService.DeleteBookAsync(99);

            // Assert
            result.Should().BeFalse();
        }
    }
}