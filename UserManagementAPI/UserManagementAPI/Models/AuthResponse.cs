namespace UserManagementAPI.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        /// <summary>
        /// Friendly status message or error description related to the
        /// authentication operation.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// JWT issued when authentication succeeds. Treat as a secret and do not
        /// log this value or expose it in error messages.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Minimal user details returned after a successful authentication.
        /// </summary>
        public UserDto User { get; set; }
    }
}
