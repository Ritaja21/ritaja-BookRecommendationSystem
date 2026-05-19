using api.src.Models;
using api.src.Models.DTO;
using api.src.Repositories;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Identity.Client;

namespace api.src.Services
{
    public class UserBookService : IUserBookService
    {
        private readonly IUserBookRepository _repo;
        private readonly ILogger<UserBookService> _logger;
       
        public UserBookService(IUserBookRepository repo, ILogger<UserBookService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<UserBook> MarkBookAsReadAsync(int userId, UserReadDTO userReadDTO)
        {
            _logger.LogInformation(
        "User {UserId} marked book {BookId} as read",
        userId,
        userReadDTO.BookId);
            var existingUserBook = await _repo.GetUserBookAsync(userId, userReadDTO.BookId);
            if (existingUserBook != null)
            {
                existingUserBook.IsRead = true;

                var updatedUserBook =
                    await _repo.UpdateUserBookAsync(existingUserBook);

                _logger.LogInformation(
                    "Updated existing read history for user {UserId} and book {BookId}",
                    userId,
                    userReadDTO.BookId);

                return updatedUserBook;
            }

            UserBook userbook = new()
            {
                UserId = userId,
                BookId = userReadDTO.BookId,
                IsRead = true
            };

            var createdUserBook =
        await _repo.CreateUserBookAsync(userbook);

            _logger.LogInformation(
                "Created new read history for user {UserId} and book {BookId}",
                userId,
                userReadDTO.BookId);

            return createdUserBook;
        }

        public async Task<UserBook> RateBookAsync(int userId, RateBookDTO rateBookDTO)
        {
            _logger.LogInformation(
       "User {UserId} rating book {BookId} with rating {Rating}",
       userId,
       rateBookDTO.BookId,
       rateBookDTO.Rating);
            var existingUserBook = await _repo.GetUserBookAsync(userId, rateBookDTO.BookId);
            if (existingUserBook != null)
            {
                existingUserBook.Rating = rateBookDTO.Rating;
                var updatedUserBook =
            await _repo.UpdateUserBookAsync(existingUserBook);

                _logger.LogInformation(
                    "Updated rating for user {UserId} and book {BookId}",
                    userId,
                    rateBookDTO.BookId);

                return updatedUserBook;
            }

            UserBook userBook = new()
            {
                UserId = userId,
                BookId = rateBookDTO.BookId,
                Rating = rateBookDTO.Rating
            };

            var createdUserBook =
        await _repo.CreateUserBookAsync(userBook);

            _logger.LogInformation(
                "Created new rating for user {UserId} and book {BookId}",
                userId,
                rateBookDTO.BookId);

            return createdUserBook;
        }

        public async Task<List<UserBook>> GetUserBookHistoryAsync(int userId)
        {
            return await _repo.GetUserHistoryAsync(userId);
        }
    }
}
