using Microsoft.EntityFrameworkCore;
using Nokey.Authentication;
using Nokey.Models;

namespace Nokey.Repositories
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly ApplicationDbContext _context;

        public JobPostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Job> PostJobAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<Job> GetJobByIdAsync(int jobId)
        {
            return await _context.Jobs
                .Where(j => j.Id == jobId)
                .Select(j => new Job
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Salary = j.Salary,
                    CreatedAt = j.CreatedAt,
                    CompanyId = j.CompanyId,
                    CreatedById = j.CreatedById
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Job>> GetAdminJobsAsync(string adminId)
        {
            return await _context.Jobs
                .Where(j => j.CreatedById == adminId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<Job> UpdateJobAsync(Job job)
        {
            var existingJob = await _context.Jobs.FindAsync(job.Id);
            if (existingJob == null)
                return null;

            existingJob.Title = string.IsNullOrWhiteSpace(job.Title) ? existingJob.Title : job.Title;
            existingJob.Description = string.IsNullOrWhiteSpace(job.Description) ? existingJob.Description : job.Description;
            existingJob.Salary = job.Salary > 0 ? job.Salary : existingJob.Salary;

            _context.Jobs.Update(existingJob);
            await _context.SaveChangesAsync();

            return existingJob;
        }

        public async Task<bool> DeleteJobAsync(int jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
                return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
