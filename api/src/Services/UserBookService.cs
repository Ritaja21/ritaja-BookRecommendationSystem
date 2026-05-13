using api.src.Models;
using api.src.Models.DTO;
using api.src.Repositories;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace api.src.Services
{
    public class UserBookService : IUserBookService
    {
        private readonly IUserBookRepository _repo;

        public UserBookService(IUserBookRepository repo)
        {
            _repo = repo;
        }

        public async Task<UserBook> MarkBookAsReadAsync(int userId, UserReadDTO userReadDTO)
        {
            var existingUserBook = await _repo.GetUserBookAsync(userId, userReadDTO.BookId);
            if (existingUserBook != null)
            {
                existingUserBook.IsRead = true;
                return await _repo.UpdateUserBookAsync(existingUserBook);
            }

            UserBook userbook = new()
            {
                UserId = userId,
                BookId = userReadDTO.BookId,
                IsRead = true
            };

            return await _repo.CreateUserBookAsync(userbook);
               

        }
    }
}
