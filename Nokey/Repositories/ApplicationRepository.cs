using Microsoft.EntityFrameworkCore;
using Nokey.Authentication;
using Nokey.Models;

namespace Nokey.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Application> FindApplicationAsync(int jobId, string userId)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == userId);
        }

        public async Task<Application> CreateApplicationAsync(Application application)
        {
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<IEnumerable<Application>> GetApplicationsByApplicantAsync(string userId)
        {
            var applications = await _context.Applications
                .Where(a => a.ApplicantId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return applications;
        }



        public async Task<Application> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.Applications.FindAsync(applicationId);
        }

        //public async Task UpdateApplicationStatusAsync(Application application, string status)
        //{
        //    application.Status = status;
        //    await _context.SaveChangesAsync();
        //}

        public async Task UpdateApplicationStatusAsync(Application application, string status)
        {
            // Convert the string `status` to the enum type `ApplicationStatus`
            if (Enum.TryParse<ApplicationStatus>(status, true, out var parsedStatus))
            {
                application.Status = parsedStatus;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"Invalid status value: {status}");
            }
        }
    }
}
