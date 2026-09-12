using UserManagementAPI.Data;
using UserManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManagementDbContext _db;
        /// <summary>
        /// Construct the service with an injected <see cref="UserManagementDbContext"/>.
        /// </summary>
        /// <param name="db">The EF Core DbContext instance.</param>
        public UserService(UserManagementDbContext db)
        {
            // DbContext is injected by DI. Keep the service focused on
            // business operations rather than transport concerns (HTTP).
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Return all users. In larger datasets, add paging/filtering to
            // avoid loading everything into memory.
            return await _db.Users.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByIdAsync(int id)
        {
            // FindAsync is efficient when tracking is not required for updates.
            return await _db.Users.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<User> CreateUserAsync(User user)
        {
            // Persist a new user. Caller is responsible for input validation
            // and ensuring sensitive fields (password) are handled elsewhere.
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        /// <inheritdoc />
        public async Task<User?> UpdateUserAsync(int id, User updatedUser)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return null;

            // Copy allowed updatable fields. Avoid overwriting sensitive
            // properties such as password hash/salt here.
            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.Department = updatedUser.Department;

            await _db.SaveChangesAsync();
            return user;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
