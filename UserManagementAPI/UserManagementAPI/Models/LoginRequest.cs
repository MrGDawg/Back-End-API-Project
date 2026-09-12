namespace UserManagementAPI.Models
{
    public class LoginRequest
    {
        /// <summary>
        /// Payload for authenticating a user.
        /// Transport security (HTTPS) is required to protect credentials.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Plaintext password provided by the client.
        /// </summary>
        public string Password { get; set; }
    }
}
