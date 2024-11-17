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
                // Check if all necessary fields for job posting are provided
                if (string.IsNullOrWhiteSpace(job.Title) || string.IsNullOrWhiteSpace(job.Description) ||
                    job.Salary <= 0 || job.CompanyId <= 0)
                {
                    return BadRequest(new { message = "Something is missing or incorrect.", success = false });
                }

                // Get the user ID from the currently authenticated user
                var userId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User is not authenticated." });
                }

                // Ensure the user is posting jobs for a company they have registered
                var company = await _companyRepository.GetCompanyByIdAsync(job.CompanyId);

                if (company == null || company.PersonId != userId)
                {
                    return BadRequest(new { message = "You can only post jobs for a company you have registered.", success = false });
                }

                // Set the CreatedById to the currently authenticated user
                job.CreatedById = userId;

                // If requirements are not provided, initialize as empty list
                if (job.Requirements == null || !job.Requirements.Any())
                {
                    job.Requirements = new List<string>();
                }

                // If RequirementsString is provided, split it into a list
                if (!string.IsNullOrWhiteSpace(job.RequirementsString))
                {
                    job.Requirements = job.RequirementsString.Split(',').ToList();
                }

                // Post the job via the repository
                var newJob = await _jobRepository.PostJobAsync(job);

                // Return the response indicating the job was created
                return Created("", new { message = "New job created successfully.", job = newJob, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
        }




        // GET: api/Job
        [HttpGet("getAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            // Retrieve all jobs, optionally filtered by keyword
            var jobs = await _jobRepository.GetAllJobsAsync();

            if (jobs == null || !jobs.Any())
            {
                return NotFound(new { message = "Jobs not found.", success = false });
            }

            return Ok(new { jobs, success = true });
        }


        // GET: api/Job/{id}
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

        // GET: api/Job/adminJobs
        [HttpGet("adminJobs")]
        public async Task<IActionResult> GetAdminJobs()
        {
            string adminId = User.FindFirst("UserId")?.Value;
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
                string userId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return Unauthorized(new { message = "User is not authenticated.", success = false });

                var existingJob = await _jobRepository.GetJobByIdAsync(id);

                if (existingJob == null)
                    return NotFound(new { message = "Job not found.", success = false });

                if (existingJob.CreatedById != userId)
                    return StatusCode(403, new { message = "You are not authorized to update this job.", success = false });

                job.Id = id;
                var updatedJob = await _jobRepository.UpdateJobAsync(job);

                if (updatedJob == null)
                    return BadRequest(new { message = "Failed to update the job.", success = false });

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
                string userId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return Unauthorized(new { message = "User is not authenticated.", success = false });

                var existingJob = await _jobRepository.GetJobByIdAsync(id);

                if (existingJob == null)
                    return NotFound(new { message = "Job not found.", success = false });

                if (existingJob.CreatedById != userId)
                    return StatusCode(403, new { message = "You are not authorized to delete this job.", success = false });

                var isDeleted = await _jobRepository.DeleteJobAsync(id);

                if (!isDeleted)
                    return BadRequest(new { message = "Failed to delete the job.", success = false });

                return Ok(new { message = "Job deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
        }









    }
}
