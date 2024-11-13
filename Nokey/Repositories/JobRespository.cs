using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nokey.Authentication;
using Nokey.Models;

namespace Nokey.Repositories
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
                .Include(j => j.Applications)
                    .ThenInclude(a => a.Applicant)
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task AddApplicationToJobAsync(Job job, Application application)
        {
            job.Applications.Add(application);
            await _context.SaveChangesAsync();
        }
    }
}
