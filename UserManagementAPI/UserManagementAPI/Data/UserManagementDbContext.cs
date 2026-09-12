using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Models;

namespace UserManagementAPI.Data
{
    public class UserManagementDbContext : DbContext
    {
        /// <summary>
        /// EF Core DbContext for the sample user management system. The DbSet
        /// properties represent persisted tables in the SQLite database.
        /// </summary>
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options)
        {
        }
        // DbSets represent tables in the SQLite database. Keep the model
        // definitions minimal for the course; expand with indexes and
        // constraints as your schema matures.
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
