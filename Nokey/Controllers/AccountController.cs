using CareerCrafter.Authentication;
using CareerCrafter.Models;
using CareerCrafter.Repositories;
using CareerCrafter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CareerCrafter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly EmailService _emailService;
        private readonly IPersonRepository _personRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public AccountController(UserManager<ApplicationUser> userManager, IPersonRepository personRepository)
        {
            this.userManager = userManager;  // Use 'this' to assign the class field
            _personRepository = personRepository;
        }




        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            // Check if the email is provided
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest("Email address is required.");
            }

            try
            {
                var person = await _personRepository.GetPersonByEmailAsync(model.Email);
                if (person == null)
                {
                    return BadRequest("No user found with the provided email address.");
                }

                var user = await userManager.FindByNameAsync(person.UserName);
                if (user == null)
                {
                    return BadRequest("User associated with this email does not exist.");
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

                var emailService = HttpContext.RequestServices.GetRequiredService<EmailService>();

                var subject = "Reset Password";
                var message = $"{person.Fullname},<br>Your password reset link is {resetLink} and Password reset token is {token}</strong>.<br>Thank you.";
                await emailService.SendEmailAsync(model.Email, subject, message);

                return Ok("A password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            // Check if the email and password are provided
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.NewPassword) || string.IsNullOrWhiteSpace(model.Token))
            {
                return BadRequest("Email, token, and new password are required.");
            }

            try
            {
                // Get the person associated with the email
                var person = await _personRepository.GetPersonByEmailAsync(model.Email);
                if (person == null)
                {
                    return BadRequest("No user found with the provided email address.");
                }

                var user = await userManager.FindByNameAsync(person.UserName);
                if (user == null)
                {
                    return BadRequest("User associated with this email does not exist.");
                }

                // Reset the password using the token and new password
                var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest("Invalid or expired token.");
                }

                return Ok("Your password has been successfully reset.");
            }
            catch (ArgumentNullException ex)
            {
                // Catch null argument exceptions
                return BadRequest("One or more required arguments are missing.");
            }
            catch (Exception ex)
            {
                // Log exception details if needed for debugging (optional)
                // Log.Error(ex, "An error occurred while resetting the password.");

                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }



    }
}
