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
                .Include(j => j.Company)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<Job> GetJobByIdAsync(int jobId)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task<IEnumerable<Job>> GetAdminJobsAsync(int adminId)
        {
            return await _context.Jobs
                .Where(j => j.CreatedById == adminId)
                .Include(j => j.Company)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }
    }
}
