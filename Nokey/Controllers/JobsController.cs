using Microsoft.AspNetCore.Mvc;
using Nokey.Models;
using Nokey.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Nokey.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IApplicationRepository applicationRepository, IJobRepository jobRepository, ILogger<JobsController> logger)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _logger = logger;
        }

        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyJob(int id)
        {
            try
            {
                string userId = User.FindFirst("UserId")?.Value;

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

                return Created("", new { message = "Job applied successfully.", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while applying for the job.");
                return StatusCode(500, new { message = "An error occurred.", success = false });
            }
        }

        [HttpDelete("{id}/withdraw")]
        public async Task<IActionResult> RemoveApplication(int id)
        {
            try
            {
                string userId = User.FindFirst("UserId")?.Value;

                var application = await _applicationRepository.FindApplicationAsync(id, userId);
                if (application == null)
                {
                    return NotFound(new { message = "You have not applied for this job.", success = false });
                }

                await _applicationRepository.DeleteApplicationAsync(application);

                return Ok(new { message = "Application withdrawn successfully.", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while withdrawing the application.");
                return StatusCode(500, new { message = "An error occurred.", success = false });
            }
        }

        [HttpGet("applied-jobs")]
        public async Task<IActionResult> GetAppliedJobs()
        {
            try
            {
                string userId = User.FindFirst("UserId")?.Value;

                var applications = await _applicationRepository.GetApplicationsByApplicantAsync(userId);

                if (applications == null || !applications.Any())
                {
                    return NotFound(new { message = "No applications found.", success = false });
                }

                return Ok(new { applications, success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching applied jobs.");
                return StatusCode(500, new { message = "An error occurred.", success = false });
            }
        }

        [HttpGet("{id}/applicants")]
        public async Task<IActionResult> GetApplicants(int id)
        {
            try
            {
                var job = await _jobRepository.FindJobByIdAsync(id);
                if (job == null)
                {
                    return NotFound(new { message = "Job not found.", success = false });
                }

                var applicants = await _applicationRepository.GetApplicantsByJobAsync(id);

                return Ok(new { applicants, success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job applicants.");
                return StatusCode(500, new { message = "An error occurred.", success = false });
            }
        }

        [HttpPatch("application/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Status is required.", success = false });
            }

            try
            {
                var application = await _applicationRepository.GetApplicationByIdAsync(id);
                if (application == null)
                {
                    return NotFound(new { message = "Application not found.", success = false });
                }

                await _applicationRepository.UpdateApplicationStatusAsync(application, status.ToLower());

                return Ok(new { message = "Status updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application status.");
                return StatusCode(500, new { message = "An error occurred.", success = false });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new { message = "Query parameter is required.", success = false });
            }

            try
            {
                var jobs = await _jobRepository.SearchJobsAsync(query);

                if (jobs == null || !jobs.Any())
                {
                    return NotFound(new { message = "No jobs found.", success = false });
                }

                return Ok(new { jobs, success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs.");
                return StatusCode(500, new { message = "An error occurred.", success = false });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobDetails(int id)
        {
            try
            {
                var job = await _jobRepository.FindJobByIdAsync(id);

                if (job == null)
                {
                    return NotFound(new { message = "Job not found.", success = false });
                }

                return Ok(new { job, success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job details.");
                return StatusCode(500, new { message = "An error occurred.", success = false });
            }
        }
    }
}
