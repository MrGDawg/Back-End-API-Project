namespace UserManagementAPI.Models
{
    public class UserDto
    {
        /// <summary>
        /// Data transfer object exposing a minimal, non-sensitive view of a user.
        /// Use this DTO in API responses instead of the full <see cref="User"/> entity.
        /// </summary>
        public int Id { get; set; }
        // DTO exposing only non-sensitive fields for public API responses.
        public string Username { get; set; }
    }
}
