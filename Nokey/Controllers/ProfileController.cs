using CareerCrafter.Models;
using CareerCrafter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace CareerCrafter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var profile = _profileRepository.GetProfileByUserId(userId);

            if (profile == null)
                return NotFound("Profile not found.");

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProfile([FromForm] ProfileCreateUpdateDto profileDto)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var updatedProfile = await _profileRepository.CreateOrUpdateProfileAsync(
                userId,
                new Profile
                {
                    Bio = profileDto.Bio,
                    Skills = profileDto.Skills.Split(',').ToList()
                },
                profileDto.Resume,
                profileDto.ProfilePhoto);

            return Ok("Profile updated successfully.");
        }

        [HttpDelete]
        public IActionResult DeleteProfile()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var success = _profileRepository.DeleteProfile(userId);

            if (!success)
                return NotFound("Profile not found.");

            return Ok("Profile deleted successfully.");
        }

        [HttpGet("DownloadResume")]
        public IActionResult DownloadResume()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var resume = _profileRepository.GetResume(userId);

            if (resume == null)
                return NotFound("Resume not found.");

            return File(resume, "application/pdf", "Resume.pdf");
        }

        [HttpGet("DownloadProfilePhoto")]
        public IActionResult DownloadProfilePhoto()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var filePath = _profileRepository.GetProfilePhotoPath(userId);

            if (filePath == null || !System.IO.File.Exists(filePath))
                return NotFound("Profile photo not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/jpeg", Path.GetFileName(filePath));
        }
    }

    public class ProfileCreateUpdateDto
    {
        public string Bio { get; set; }
        public string Skills { get; set; }
        public IFormFile Resume { get; set; }
        public IFormFile ProfilePhoto { get; set; }
    }
}
