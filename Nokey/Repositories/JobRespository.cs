using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareerCrafter.Authentication;
using CareerCrafter.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerCrafter.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Job> FindJobByIdAsync(int jobId)
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

        public async Task AddApplicationToJobAsync(Job job, Application application)
        {
            application.JobId = job.Id;

            _context.Applications.Add(application);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Job>> SearchJobsAsync(string query)
        {
            query = query.ToLower();

            return await _context.Jobs
                .Where(job => job.Title.ToLower().Contains(query) || job.Description.ToLower().Contains(query))
                .ToListAsync();
        }
    }
}
