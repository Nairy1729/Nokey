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
            var person = await _personRepository.GetPersonByEmailAsync(model.Email);
            var user = await userManager.FindByNameAsync(person.UserName);
            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";


            var emailService = HttpContext.RequestServices.GetRequiredService<EmailService>();
           
            var subject = "Reset Password";
            var message = $"Dear User,<br>Your Password {token}  reset link is {resetLink}</strong>.<br>Thank you.";
            await emailService.SendEmailAsync(model.Email, subject, message);

            return Ok("A password reset link has been sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var person = await _personRepository.GetPersonByEmailAsync(model.Email);
            var user = await userManager.FindByNameAsync(person.UserName);

            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }


            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest("Invalid or expired token.");
            }

            return Ok("Your password has been successfully reset.");
        }


    }
}
