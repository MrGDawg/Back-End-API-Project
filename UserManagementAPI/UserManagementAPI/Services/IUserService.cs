using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    /// <summary>
    /// Service contract for user-related operations. Implementations should
    /// encapsulate data access concerns and expose task-based async methods
    /// suitable for use from controllers.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieve all users. Implementations may add paging in the future.
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Find a user by identifier.
        /// </summary>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Create a new user record.
        /// </summary>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Delete a user by identifier. Returns true when the user was removed.
        /// </summary>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Update an existing user's mutable fields.
        /// </summary>
        Task<User?> UpdateUserAsync(int id, User updatedUser);
    }
}
