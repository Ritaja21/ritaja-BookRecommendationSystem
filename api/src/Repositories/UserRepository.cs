using api.src.Data;
using api.src.Models;

namespace api.src.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _db.Users.FindAsync(id);
        }
    }
}
