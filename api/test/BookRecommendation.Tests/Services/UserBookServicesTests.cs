using api.src.Models.DTO;
using Microsoft.Extensions.Logging;

namespace BookRecommendation.Tests.Services
{
    public class UserBookServiceTests
    {
        private readonly Mock<IUserBookRepository> _mockRepo;

        private readonly Mock<ILogger<UserBookService>> _mockLogger;

        private readonly UserBookService _userBookService;

        public UserBookServiceTests()
        {
            _mockRepo = new Mock<IUserBookRepository>();

            _mockLogger = new Mock<ILogger<UserBookService>>();

            _userBookService = new UserBookService(
                _mockRepo.Object,
                _mockLogger.Object
            );
        }
        [Fact]
        public async Task MarkBookAsReadAsync_ShouldCreateNewUserBook_WhenNoExistingEntry()
        {
            // Arrange
            int userId = 1;

            var readDto = new UserReadDTO
            {
                BookId = 10
            };

            _mockRepo
                .Setup(repo => repo.GetUserBookAsync(userId, readDto.BookId))
                .ReturnsAsync((UserBook?)null);

            _mockRepo
                .Setup(repo => repo.CreateUserBookAsync(It.IsAny<UserBook>()))
                .ReturnsAsync((UserBook ub) => ub);

            // Act
            var result = await _userBookService.MarkBookAsReadAsync(userId, readDto);

            // Assert
            result.Should().NotBeNull();

            result.UserId.Should().Be(userId);

            result.BookId.Should().Be(readDto.BookId);

            result.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task MarkBookAsReadAsync_ShouldUpdateExistingUserBook_WhenEntryExists()
        {
            // Arrange
            int userId = 1;

            var readDto = new UserReadDTO
            {
                BookId = 10
            };

            var existingUserBook = new UserBook
            {
                UserId = userId,
                BookId = 10,
                IsRead = false
            };

            _mockRepo
                .Setup(repo => repo.GetUserBookAsync(userId, readDto.BookId))
                .ReturnsAsync(existingUserBook);

            _mockRepo
                .Setup(repo => repo.UpdateUserBookAsync(existingUserBook))
                .ReturnsAsync(existingUserBook);

            // Act
            var result = await _userBookService.MarkBookAsReadAsync(userId, readDto);

            // Assert
            result.Should().NotBeNull();

            result.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task RateBookAsync_ShouldCreateRating_WhenNoExistingEntry()
        {
            // Arrange
            int userId = 1;

            var rateDto = new RateBookDTO
            {
                BookId = 10,
                Rating = 4.5
            };

            _mockRepo
                .Setup(repo => repo.GetUserBookAsync(userId, rateDto.BookId))
                .ReturnsAsync((UserBook?)null);

            _mockRepo
                .Setup(repo => repo.CreateUserBookAsync(It.IsAny<UserBook>()))
                .ReturnsAsync((UserBook ub) => ub);

            // Act
            var result = await _userBookService.RateBookAsync(userId, rateDto);

            // Assert
            result.Should().NotBeNull();

            result.Rating.Should().Be(4.5);
        }

        [Fact]
        public async Task RateBookAsync_ShouldUpdateRating_WhenEntryExists()
        {
            // Arrange
            int userId = 1;

            var rateDto = new RateBookDTO
            {
                BookId = 10,
                Rating = 5
            };

            var existingUserBook = new UserBook
            {
                UserId = userId,
                BookId = 10,
                Rating = 2
            };

            _mockRepo
                .Setup(repo => repo.GetUserBookAsync(userId, rateDto.BookId))
                .ReturnsAsync(existingUserBook);

            _mockRepo
                .Setup(repo => repo.UpdateUserBookAsync(existingUserBook))
                .ReturnsAsync(existingUserBook);

            // Act
            var result = await _userBookService.RateBookAsync(userId, rateDto);

            // Assert
            result.Should().NotBeNull();

            result.Rating.Should().Be(5);
        }
    }
}