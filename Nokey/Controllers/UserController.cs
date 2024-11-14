using Microsoft.AspNetCore.Mvc;
using Nokey.Models;
using Nokey.Repositories;
using System.Threading.Tasks;

namespace Nokey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest("User data is required.");
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(person.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists with this email.");
            }

            var newUser = await _userRepository.RegisterAsync(person);

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        // Get user by email
        [HttpGet("getByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // Update profile
        [HttpPut("updateProfile/{id}")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] Person updatedPerson)
        {
            var updatedUser = await _userRepository.UpdateProfileAsync(id, updatedPerson);
            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUser);
        }

        // Delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var isDeleted = await _userRepository.DeleteUserAsync(id);
            if (!isDeleted)
            {
                return NotFound("User not found.");
            }

            return NoContent();
        }
    }
}
