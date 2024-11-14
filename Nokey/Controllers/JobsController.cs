using Microsoft.AspNetCore.Mvc;
using Nokey.Models;
using Nokey.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Nokey.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Ensure the user is authenticated
    public class JobsController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;

        public JobsController(IApplicationRepository applicationRepository, IJobRepository jobRepository)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
        }

        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyJob(int id)
        {
            // Retrieve userId from token
            string userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (await _applicationRepository.FindApplicationAsync(id, userId) != null)
            {
                return BadRequest(new { message = "You have already applied for this job.", success = false });
            }

            var job = await _jobRepository.FindJobByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = "Job not found.", success = false });
            }

            var newApplication = new Application
            {
                JobId = id,
                ApplicantId = userId,
            };

            await _applicationRepository.CreateApplicationAsync(newApplication);
            await _jobRepository.AddApplicationToJobAsync(job, newApplication);

            return Created("", new { message = "Job applied successfully.", success = true });
        }

        [HttpGet("applied-jobs")]
        public async Task<IActionResult> GetAppliedJobs()
        {
            // Retrieve userId from token
            string userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var applications = await _applicationRepository.GetApplicationsByApplicantAsync(userId);

            if (applications == null || !applications.Any())
            {
                return NotFound(new { message = "No applications found.", success = false });
            }

            return Ok(new { applications, success = true });
        }

        [HttpGet("{id}/applicants")]
        public async Task<IActionResult> GetApplicants(int id)
        {
            var job = await _jobRepository.FindJobByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = "Job not found.", success = false });
            }

            return Ok(new { job, success = true });
        }

        [HttpPatch("application/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Status is required.", success = false });
            }

            var application = await _applicationRepository.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound(new { message = "Application not found.", success = false });
            }

            await _applicationRepository.UpdateApplicationStatusAsync(application, status.ToLower());

            return Ok(new { message = "Status updated successfully.", success = true });
        }
    }
}
