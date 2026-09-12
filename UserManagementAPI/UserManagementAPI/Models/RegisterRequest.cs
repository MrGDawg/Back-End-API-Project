namespace UserManagementAPI.Models
{
    public class RegisterRequest
    {
        /// <summary>
        /// Payload for user registration containing credentials.
        /// Validate these values on the server before using them.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Plaintext password. Transport security (HTTPS) is required to
        /// protect this value in transit.
        /// </summary>
        public string Password { get; set; }
    }
}
