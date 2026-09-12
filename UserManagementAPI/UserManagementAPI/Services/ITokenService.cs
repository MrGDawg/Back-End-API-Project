namespace UserManagementAPI.Services
{
    /// <summary>
    /// Token generation abstraction.
    /// Implementations should read signing configuration from <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>
    /// and produce a signed JWT for the specified subject (username).
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generate a JWT for the specified username.
        /// </summary>
        /// <param name="username">The subject (typically the username).</param>
        /// <returns>A signed JWT string.</returns>
        string GenerateToken(string username);
    }
}
