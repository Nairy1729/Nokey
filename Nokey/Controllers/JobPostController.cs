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

        public JobPostController(IJobPostRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // POST: api/Job
        //[HttpPost("postJob")]
        //public async Task<IActionResult> PostJob([FromBody] Job job)
        //{
        //    if (string.IsNullOrWhiteSpace(job.Title) || string.IsNullOrWhiteSpace(job.Description) ||
        //        job.Salary <= 0 || job.CompanyId <= 0 || job.CreatedById != null)
        //    {
        //        return BadRequest(new { message = "Something is missing or incorrect.", success = false });
        //    }

        //    // Assign CreatedById to the current user
        //    job.CreatedById = GetUserId();

        //    // Assuming requirements are passed as a comma-separated string in the request
        //    if (job.Requirements == null || !job.Requirements.Any())
        //    {
        //        job.Requirements = new List<JobRequirement>();
        //    }

        //    // Save the job
        //    var newJob = await _jobRepository.PostJobAsync(job);
        //    return Created("", new { message = "New job created successfully.", job = newJob, success = true });
        //}

        [HttpPost("postJob")]
        public async Task<IActionResult> PostJob([FromBody] Job job)
        {
            // Validate the incoming job data
            if (string.IsNullOrWhiteSpace(job.Title) || string.IsNullOrWhiteSpace(job.Description) ||
                job.Salary <= 0 || job.CompanyId <= 0 || job.CreatedById != null)
            {
                return BadRequest(new { message = "Something is missing or incorrect.", success = false });
            }

            // Assign CreatedById to the current user
            job.CreatedById = GetUserId();

            // Validate Requirements, which is now a List<string> or a comma-separated string
            if (job.Requirements == null || !job.Requirements.Any())
            {
                job.Requirements = new List<string>();  // Initialize an empty list if no requirements are provided
            }

            // If Requirements are passed as a comma-separated string, split them into a list
            // (This step may not be necessary if you directly pass a List<string> from the client)
            if (!string.IsNullOrWhiteSpace(job.RequirementsString))
            {
                job.Requirements = job.RequirementsString.Split(',').ToList();
            }

            // Save the job
            var newJob = await _jobRepository.PostJobAsync(job);

            return Created("", new { message = "New job created successfully.", job = newJob, success = true });
        }



        // GET: api/Job
        [HttpGet("getAllJobs")]
        public async Task<IActionResult> GetAllJobs([FromQuery] string keyword = "")
        {
            var jobs = await _jobRepository.GetAllJobsAsync(keyword);

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
            string adminId = GetUserId();
            var jobs = await _jobRepository.GetAdminJobsAsync(adminId);

            if (jobs == null || !jobs.Any())
            {
                return NotFound(new { message = "Jobs not found.", success = false });
            }

            return Ok(new { jobs, success = true });
        }

        private string GetUserId()
        {
            // Extract user ID from JWT token claims.
            return (User.FindFirst("sub")?.Value);
        }
    }
}
