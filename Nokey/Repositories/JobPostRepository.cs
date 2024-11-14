// JobRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Job>> GetAllJobsAsync(string keyword)
        {
            return await _context.Jobs
                .Where(j => j.Title.Contains(keyword) || j.Description.Contains(keyword))
                .Include(j => j.CompanyId)
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
                    CreatedById = j.CreatedById,
                    // Manually include applications by joining with the Applications table using the JobId foreign key
                     // Get all related applications
                })
                .FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<Job>> GetAdminJobsAsync(string adminId)
        {
            return await _context.Jobs
                .Where(j => j.CreatedById == adminId)
                .Include(j => j.CompanyId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }
    }
}
