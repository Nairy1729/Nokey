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
            return await _context.Applications
                .Where(a => a.ApplicantId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Application> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.Applications.FindAsync(applicationId);
        }

        public async Task UpdateApplicationStatusAsync(Application application, string status)
        {
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

        public async Task<IEnumerable<object>> GetApplicantsByJobAsync(int jobId)
        {
            return await _context.Applications
                .Join(
                    _context.Persons,
                    application => application.ApplicantId,
                    person => person.Id,
                    (application, person) => new
                    {
                        ApplicationId = application.Id,
                        JobId = application.JobId,
                        ApplicantId = application.ApplicantId,
                        ApplicantName = person.Fullname,
                        Email = person.Email,
                        PhoneNumber = person.PhoneNumber,
                        Status = application.Status,
                        CreatedAt = application.CreatedAt,
                        UpdatedAt = application.UpdatedAt
                    })
                .Where(a => a.JobId == jobId)
                .ToListAsync();
        }

        public async Task DeleteApplicationAsync(Application application)
        {
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
        }
    }
}
