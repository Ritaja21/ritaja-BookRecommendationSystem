using api.src.Models;

namespace api.src.Repositories
{
    public interface IUserBookRepository
    {
        Task<UserBook?> GetUserBookAsync(int userId, int bookId);
        Task<UserBook> CreateUserBookAsync(UserBook userBook);

        Task<UserBook> UpdateUserBookAsync(UserBook userBook);

        Task<List<UserBook>> GetUserHistoryAsync(int userId);
    }
}
