using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nokey.Models;
using Nokey.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Nokey.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostController : ControllerBase
    {
        private readonly IJobPostRepository _jobRepository;
        private readonly ICompanyRepository _companyRepository;

        public JobPostController(ICompanyRepository companyRepository, IJobPostRepository jobRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
        }

        [HttpPost("postJob")]
        public async Task<IActionResult> PostJob([FromBody] Job job)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(job.Title) || string.IsNullOrWhiteSpace(job.Description) ||
                    job.Salary <= 0 || job.CompanyId <= 0)
                {
                    return BadRequest(new { message = "Invalid job details provided.", success = false });
                }

                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User is not authenticated.", success = false });
                }

                var company = await _companyRepository.GetCompanyByIdAsync(job.CompanyId);
                if (company == null || company.PersonId != userId)
                {
                    return BadRequest(new { message = "You can only post jobs for a company you own.", success = false });
                }

                job.CreatedById = userId;

                if (!string.IsNullOrWhiteSpace(job.RequirementsString))
                {
                    job.Requirements = job.RequirementsString.Split(',').ToList();
                }

                var newJob = await _jobRepository.PostJobAsync(job);
                return Created("", new { message = "New job created successfully.", job = newJob, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
        }

        [HttpGet("getAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobRepository.GetAllJobsAsync();

            if (jobs == null || !jobs.Any())
            {
                return NotFound(new { message = "Jobs not found.", success = false });
            }

            return Ok(new { jobs, success = true });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobRepository.GetJobByIdAsync(id);

            if (job == null)
            {
                return NotFound(new { message = "Job not found.", success = false });
            }

            return Ok(new { job, success = true });
        }

        [HttpGet("adminJobs")]
        public async Task<IActionResult> GetAdminJobs()
        {
            var adminId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(adminId))
            {
                return Unauthorized(new { message = "User is not authenticated.", success = false });
            }

            var jobs = await _jobRepository.GetAdminJobsAsync(adminId);

            if (jobs == null || !jobs.Any())
            {
                return NotFound(new { message = "Jobs not found.", success = false });
            }

            return Ok(new { jobs, success = true });
        }

        [HttpPatch("updateJob/{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] Job job)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new { message = "User is not authenticated.", success = false });
                }

                var existingJob = await _jobRepository.GetJobByIdAsync(id);
                if (existingJob == null)
                {
                    return NotFound(new { message = "Job not found.", success = false });
                }

                if (existingJob.CreatedById != userId)
                {
                    return StatusCode(403, new { message = "You are not authorized to update this job.", success = false });
                }

                job.Id = id;
                var updatedJob = await _jobRepository.UpdateJobAsync(job);

                if (updatedJob == null)
                {
                    return BadRequest(new { message = "Failed to update the job.", success = false });
                }

                return Ok(new { message = "Job updated successfully.", job = updatedJob, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
        }

        [HttpDelete("deleteJob/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new { message = "User is not authenticated.", success = false });
                }

                var existingJob = await _jobRepository.GetJobByIdAsync(id);
                if (existingJob == null)
                {
                    return NotFound(new { message = "Job not found.", success = false });
                }

                if (existingJob.CreatedById != userId)
                {
                    return StatusCode(403, new { message = "You are not authorized to delete this job.", success = false });
                }

                var isDeleted = await _jobRepository.DeleteJobAsync(id);
                if (!isDeleted)
                {
                    return BadRequest(new { message = "Failed to delete the job.", success = false });
                }

                return Ok(new { message = "Job deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
        }
    }
}
