using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CareerCrafter.Repositories;
using CareerCrafter.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using CareerCrafter.Services;

namespace CareerCrafter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class JobsController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<JobsController> _logger;
        private readonly IPersonRepository _personRepository;

        public JobsController(IApplicationRepository applicationRepository, IJobRepository jobRepository, IPersonRepository personRepository,IProfileRepository profileRepository, ILogger<JobsController> logger)
        {
            _personRepository = personRepository;
            _applicationRepository = applicationRepository;
            _profileRepository = profileRepository;
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

        [HttpGet("applicant/{userId}/resume")]
        public IActionResult DownloadResume(string userId)
        {
            //var userId = User.FindFirst("UserId")?.Value;
            var resume = _profileRepository.GetResume(userId);

            if (resume == null)
                return NotFound("Resume not found.");

            return File(resume, "application/pdf", "Resume.pdf");
        }

        //[HttpPatch("application/{id}/status")]
        //public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        //{
        //    if (string.IsNullOrEmpty(status))
        //    {
        //        return BadRequest(new { message = "Status is required.", success = false });
        //    }

        //    try
        //    {
        //        var application = await _applicationRepository.GetApplicationByIdAsync(id);
        //        if (application == null)
        //        {
        //            return NotFound(new { message = "Application not found.", success = false });
        //        }

        //        await _applicationRepository.UpdateApplicationStatusAsync(application, status.ToLower());

        //        return Ok(new { message = "Status updated successfully.", success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating application status.");
        //        return StatusCode(500, new { message = "An error occurred.", success = false });
        //    }
        //}

        //[HttpPatch("update-status")]
        //public async Task<IActionResult> UpdateStatus([FromQuery] string applicantId, [FromQuery] int jobId, [FromBody] string status)
        //{
        //    if (string.IsNullOrWhiteSpace(status))
        //    {
        //        return BadRequest(new { message = "Status is required.", success = false });
        //    }

        //    try
        //    {
        //        var application = await _applicationRepository.GetApplicationAsync(applicantId, jobId);
        //        if (application == null)
        //        {
        //            return NotFound(new { message = "Application not found.", success = false });
        //        }

        //        await _applicationRepository.UpdateApplicationStatusAsync(application, status);

        //        return Ok(new { message = "Status updated successfully.", success = true });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        _logger.LogWarning(ex, "Invalid status provided.");
        //        return BadRequest(new { message = ex.Message, success = false });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating application status.");
        //        return StatusCode(500, new { message = "An error occurred while updating status.", success = false });
        //    }
        //}

        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string applicantId, [FromQuery] int jobId, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new { message = "Status is required.", success = false });
            }

            try
            {
                // Validate status
                if (!Enum.TryParse<ApplicationStatus>(status, true, out var parsedStatus))
                {
                    throw new ArgumentException("Invalid status provided. Valid statuses are Pending, Accepted, or Rejected.");
                }

                // Fetch the application
                var application = await _applicationRepository.GetApplicationAsync(applicantId, jobId);
                if (application == null)
                {
                    return NotFound(new { message = "Application not found.", success = false });
                }

                // Update the application status
                await _applicationRepository.UpdateApplicationStatusAsync(application, parsedStatus.ToString());

                // Fetch applicant details for email
                var applicant = await _personRepository.GetPersonByIdAsync(applicantId);
                if (applicant != null)
                {
                    var emailService = HttpContext.RequestServices.GetRequiredService<EmailService>();
                    var subject = "Application Status Update";
                    var message = $"Dear {applicant.Fullname},<br>Your application status for Job ID {jobId} has been updated to: <strong>{parsedStatus}</strong>.<br>Thank you.";
                    await emailService.SendEmailAsync(applicant.Email, subject, message);
                }

                return Ok(new { message = "Status updated successfully and notification sent.", success = true });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid status provided.");
                return BadRequest(new { message = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application status.");
                return StatusCode(500, new { message = "An error occurred while updating status.", success = false });
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
