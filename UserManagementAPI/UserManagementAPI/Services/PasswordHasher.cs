using System.Security.Cryptography;

// Minimal password hashing helper using PBKDF2 (Rfc2898DeriveBytes).
// This helper produces a salt and a derived key using SHA-256 and a
// high iteration count. It is suitable for demonstration and teaching
// but in production consider using a well-reviewed library and making
// iteration counts configurable.
/// <summary>
/// Minimal password hashing helper using PBKDF2 (Rfc2898DeriveBytes).
/// Provides simple Hash and Verify helpers used by the sample AuthService.
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// Hash a plaintext password and return (hash, salt) where both values
    /// are encoded as Base64 strings. The salt length and iteration count
    /// provide a reasonable work factor for development; tune for production.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>Tuple of (hash, salt) as Base64 strings.</returns>
    public static (string Hash, string Salt) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        var hash = Convert.ToBase64String(pbkdf2.GetBytes(32));

        return (hash, salt);
    }

    /// <summary>
    /// Verify a plaintext password against stored hash and salt.
    /// </summary>
    /// <param name="password">Plaintext password to verify.</param>
    /// <param name="storedHash">Stored Base64 hash value.</param>
    /// <param name="storedSalt">Stored Base64 salt value.</param>
    /// <returns>True when the password matches the stored hash.</returns>
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        var computedHash = Convert.ToBase64String(pbkdf2.GetBytes(32));
        return computedHash == storedHash;
    }
}
