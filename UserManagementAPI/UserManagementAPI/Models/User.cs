namespace UserManagementAPI.Models
{
    public class User
    {
        /// <summary>
        /// Domain entity representing a user in the system. This model is used
        /// by EF Core and therefore may contain persistence-oriented fields.
        /// Use DTOs (e.g., <see cref="UserDto"/>) when returning user data
        /// to API clients to avoid exposing sensitive fields.
        /// </summary>
        public int Id { get; set; }
        // User profile fields surfaced by the API. Keep the public user shape
        // minimal to avoid accidentally exposing PII in responses.
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }

        // Authentication fields. These must NEVER be returned to callers. Use a
        // DTO (e.g., UserDto) when returning user information from controllers.
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
