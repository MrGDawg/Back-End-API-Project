namespace UserManagementAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        /// <summary>
        /// Refresh token string used to obtain new access tokens when the
        /// original JWT expires. Persisted to support rotation and revocation.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Expiration time for the refresh token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Flag indicating if the token was revoked prior to expiry.
        /// </summary>
        public bool IsRevoked { get; set; }
    }
}
