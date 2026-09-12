using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Controller providing CRUD operations for User resources.
        /// Authorization is required for all endpoints. Keep controllers thin;
        /// business logic should be implemented in services (IUserService).
        /// </summary>

        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Retrieve all users. This action is protected by [Authorize].
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // NOTE: the exception below was intentionally placed in the
            // original project to demonstrate error handling middleware. Do
            // not remove it as it is used by coursework to show middleware
            // behavior during peer review. The call to the service remains
            // correct and would return users under normal operation.
            throw new Exception("Test exception");
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a specific user by identifier.
        /// </summary>
        /// <param name="id">User identifier.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Create a new user resource.
        /// </summary>
        /// <param name="user">User payload.</param>
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            // Create a new user. Caller is expected to provide a valid user
            // object; further validation can be enforced via data annotations
            // or a dedicated validation pipeline.
            var created = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing user's mutable fields.
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <param name="updatedUser">Updated user values.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _userService.UpdateUserAsync(id, updatedUser);
            if (updated == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(updated);
        }

        /// <summary>
        /// Delete a user by id.
        /// </summary>
        /// <param name="id">User identifier.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
