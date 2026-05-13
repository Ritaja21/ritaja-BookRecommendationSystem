using api.src.Models;
using api.src.Models.DTO;

namespace api.src.Services
{
    public interface IUserBookService
    {
        Task<UserBook> MarkBookAsReadAsync(int userId, UserReadDTO userReadDTO);
        Task<UserBook> RateBookAsync(int userId, RateBookDTO rateBookDTO);
        //Task<List<UserBook>> GetUserBookHistoryAsync(int userId);

    }
}
