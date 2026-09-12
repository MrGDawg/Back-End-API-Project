using UserManagementAPI.Models;

// Simple in-memory authentication service used for the sample application.
// Responsibilities:
// - Validate and register users (demo in-memory store)
// - Verify credentials on login
// - Generate JWTs via ITokenService
// Notes for reviewers: this service intentionally uses an in-memory
// Dictionary for simplicity in the course project. For production, replace
// with a persistent user store and add rate limiting/brute-force protections.
namespace UserManagementAPI.Services
{
    // Authentication contract exposed to controllers. Keep this surface small
    // and focused: higher-level orchestration belongs in application services
    // that are easier to unit test.
    /// <summary>
    /// Contract for authentication operations such as registering and logging in users.
    /// Implementations should return an <see cref="AuthResponse"/> describing the
    /// operation outcome and issued tokens when appropriate.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Register a new user based on the provided request.
        /// </summary>
        /// <param name="request">Registration details (username and password).</param>
        /// <returns>An <see cref="AuthResponse"/> with the result of the operation.</returns>
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Authenticate a user using supplied credentials.
        /// </summary>
        /// <param name="request">Login details (username and password).</param>
        /// <returns>An <see cref="AuthResponse"/> containing a JWT on success.</returns>
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }

    /// <summary>
    /// Simple in-memory authentication service used for the sample application.
    /// This service is intended for demonstration and tests only and stores users
    /// in memory. Do not use in production.
    /// </summary>
    public class AuthService : IAuthService
    {
        // In-memory user store (id -> user). This is intentionally simple so the
        // course focuses on API mechanics. Replace with a DbContext-backed
        // repository in real projects.
        private readonly Dictionary<int, User> _users;
        private int _nextId = 1;
        private readonly ITokenService _tokenService;

        public AuthService(ITokenService tokenService)
        {
            _tokenService = tokenService;
            _users = new Dictionary<int, User>();
        }

        // Register a new user. Returns an AuthResponse containing a token and
        // a minimal user DTO on success. This method performs simple input
        // validation and uniqueness checking against the in-memory store.
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            await Task.CompletedTask;

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Username and password are required."
                };
            }

            // Check if user already exists (case-insensitive)
            if (_users.Values.Any(u =>
                u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "A user with this username already exists."
                };
            }

            // Hash password using the provided helper. Salt is randomly generated.
            var (hash, salt) = PasswordHasher.HashPassword(request.Password);

            // Create user in the in-memory store
            var user = new User
            {
                Id = _nextId++,
                Username = request.Username,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _users[user.Id] = user;

            // Generate JWT using the token service
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                User = MapToDto(user)
            };
        }

        // Authenticate existing user and return token on success.
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            await Task.CompletedTask;

            var user = _users.Values.FirstOrDefault(u =>
                u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));

            if (user == null ||
                !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = MapToDto(user)
            };
        }

        private string GenerateJwtToken(User user)
        {
            return _tokenService.GenerateToken(user.Username);
        }

        // Map domain user to a DTO intended for public responses (do not expose
        // password hash/salt or other sensitive properties).
        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username
            };
        }
    }
}
