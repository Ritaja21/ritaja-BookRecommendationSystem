using api.src.Data;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    public class UserBookRepository : IUserBookRepository
    {
        private readonly AppDbContext _db;
        public UserBookRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserBook?> GetUserBookAsync(int userId, int bookId)
        {
            return await _db.UserBooks
               .FirstOrDefaultAsync(ub =>
                   ub.UserId == userId &&
                   ub.BookId == bookId);
        }

        public async Task<UserBook> CreateUserBookAsync(UserBook userBook)
        {
            await _db.UserBooks.AddAsync(userBook);
            await _db.SaveChangesAsync();
            return userBook;
        }

        public async Task<UserBook> UpdateUserBookAsync(UserBook userBook)
        {
            _db.UserBooks.Update(userBook);
            await _db.SaveChangesAsync();
            return userBook;
        }

        public async Task<List<UserBook>> GetUserHistoryAsync(int userId)
        {
            return await _db.UserBooks
                .Include(ub => ub.Book)
                .Where(ub => ub.UserId == userId)
                .ToListAsync();
        }

    }
}
