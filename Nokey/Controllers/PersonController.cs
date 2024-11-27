using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CareerCrafter.Repositories;
using CareerCrafter.Models;
using CareerCrafter.Authentication;
using Microsoft.AspNetCore.Cors;

namespace CareerCrafter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;
        private readonly IAuthenticationService _authService;

        public PersonController(IPersonRepository personRepository, IAuthenticationService authService)
        {
            _personRepository = personRepository;
            _authService = authService;
        }

        [HttpGet("search/email")]
        public async Task<IActionResult> SearchByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new Response { Status = "Error", Message = "Email is required." });

            var person = await _personRepository.GetPersonByEmailAsync(email);
            if (person == null)
                return NotFound(new Response { Status = "Error", Message = "Person with this email not found." });

            return Ok(person);
        }

        [HttpGet("search/phone")]
        public async Task<IActionResult> SearchByPhoneNumber(long phoneNumber)
        {
            var person = await _personRepository.GetPersonByPhoneNumberAsync(phoneNumber);
            if (person == null)
                return NotFound(new Response { Status = "Error", Message = "Person with this phone number not found." });

            return Ok(person);
        }

        [HttpGet("search/username")]
        public async Task<IActionResult> SearchByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest(new Response { Status = "Error", Message = "Username is required." });

            var person = await _personRepository.GetPersonByUserNameAsync(userName);
            if (person == null)
                return NotFound(new Response { Status = "Error", Message = "Person with this username not found." });

            return Ok(person);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Person updatedPerson)
        {
            var currentUserId = User.FindFirst("UserId")?.Value;

            if (updatedPerson.Id != currentUserId)
                return Unauthorized(new Response { Status = "Error", Message = "You can only edit your own profile." });

            var person = await _personRepository.GetPersonByIdAsync(currentUserId);
            if (person == null)
                return NotFound(new Response { Status = "Error", Message = "Person not found." });

            if (await _personRepository.ExistsByEmailAsync(updatedPerson.Email, currentUserId))
                return BadRequest(new Response { Status = "Error", Message = "Email is already in use." });

            if (await _personRepository.ExistsByPhoneNumberAsync(updatedPerson.PhoneNumber, currentUserId))
                return BadRequest(new Response { Status = "Error", Message = "Phone number is already in use." });

            if (await _personRepository.ExistsByUserNameAsync(updatedPerson.UserName, currentUserId))
                return BadRequest(new Response { Status = "Error", Message = "User name is already in use." });

            var updated = await _personRepository.UpdatePersonAsync(updatedPerson);
            if (updated == null)
                return BadRequest(new Response { Status = "Error", Message = "Failed to update the profile." });

            await _personRepository.SaveAsync();
            return Ok(new Response { Status = "Success", Message = "Profile updated successfully." });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            var currentUserId = User.FindFirst("UserId")?.Value;
            var person = await _personRepository.GetPersonByIdAsync(currentUserId);
            if (person == null)
                return NotFound(new Response { Status = "Error", Message = "Person not found." });

            await _personRepository.DeletePersonAsync(currentUserId);
            await _authService.Logout();

            return Ok(new Response { Status = "Success", Message = "Account deleted (soft delete) and logged out successfully." });
        }
    }
}
