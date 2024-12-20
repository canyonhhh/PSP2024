using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSPOS.ApiService.Services.Interfaces;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;
using Serilog;

namespace PSPOS.ApiService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] string? role = null,
            [FromQuery] string? name = null,
            [FromQuery] string? surname = null,
            [FromQuery] string? businessId = null,
            [FromQuery] int limit = 10,
            [FromQuery] int skip = 0)
        {
            Log.Information("Fetching users with role: {Role}, name: {Name}, surname: {Surname}, businessId: {BusinessId}, limit: {Limit}, skip: {Skip}", role, name, surname, businessId, limit, skip);
            if (limit <= 0 || skip < 0)
            {
                Log.Warning("Invalid pagination parameters: limit={Limit}, skip={Skip}", limit, skip);
                return BadRequest("Invalid pagination parameters.");
            }

            var users = await _userService.GetAllUsersAsync(role, name, surname, limit, skip, businessId);
            Log.Information("Fetched {Count} users", users.Count());

            return Ok(users);
        }

        // GET: /users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            Log.Information("Fetching user with ID: {Id}", id);
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                Log.Warning("User not found with ID: {Id}", id);
                return NotFound(new { message = "User not found." });
            }

            Log.Information("Fetched user with ID: {Id}", id);
            return Ok(user);
        }

        // POST: /users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            Log.Information("Creating user with email: {Email}", userDto.Email);
            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid user data: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            await _userService.AddUserAsync(userDto);
            Log.Information("User created with email: {Email}", userDto.Email);

            return StatusCode(201);
        }

        // PUT: /users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            Log.Information("Updating user with ID: {Id}", id);
            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid user data: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                Log.Warning("User not found with ID: {Id}", id);
                return NotFound(new { message = "User not found." });
            }

            await _userService.UpdateUserAsync(id, userDto);
            Log.Information("User updated with ID: {Id}", id);
            return Ok(new { message = "User updated successfully." });
        }

        // DELETE: /users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            Log.Information("Deleting user with ID: {Id}", id);
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                Log.Warning("User not found with ID: {Id}", id);
                return NotFound(new { message = "User not found." });
            }

            await _userService.DeleteUserAsync(id);
            Log.Information("User deleted with ID: {Id}", id);
            return NoContent();
        }
    }
}