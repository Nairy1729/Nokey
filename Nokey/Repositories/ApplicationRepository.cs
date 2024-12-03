using CareerCrafter.Authentication;
using CareerCrafter.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerCrafter.Repositories
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

        public async Task<IEnumerable<object>> GetApplicationsByApplicantAsync(string userId)
        {
            return await _context.Applications
                .Join(
                    _context.Jobs,
                    application => application.JobId,  // foreign key to Jobs
                    job => job.Id,                      // primary key of Jobs
                    (application, job) => new
                    {
                        JobId = job.Id,
                        application.Id,
                        application.ApplicantId,
                        application.Status,
                        application.CreatedAt,
                        application.UpdatedAt,
                        JobTitle = job.Title,           // Job Title
                        JobDescription = job.Description, // Job Description
                        JobSalary = job.Salary,        // Job Salary
                        JobCreatedAt = job.CreatedAt,  // Job Creation Date
                        Requirements = job.Requirements // Job Requirements
                    })
                .Where(a => a.ApplicantId == userId) // Filtering applications by ApplicantId
                .OrderByDescending(a => a.CreatedAt) // Order by application creation date
                .ToListAsync();
        }


        public async Task<Application> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.Applications.FindAsync(applicationId);
        }

        //public async Task UpdateApplicationStatusAsync(Application application, string status)
        //{
        //    if (Enum.TryParse<ApplicationStatus>(status, true, out var parsedStatus))
        //    {
        //        application.Status = parsedStatus;
        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        throw new ArgumentException($"Invalid status value: {status}");
        //    }
        //}

        public async Task<IEnumerable<object>> GetApplicantsByJobAsync(int jobId)
        {
            return await _context.Applications
                .Join(
                    _context.Persons,
                    application => application.ApplicantId,
                    person => person.Id,
                    (application, person) => new { application, person }
                )
                .Join(
                    _context.Profiles,
                    combined => combined.person.Id,
                    profile => profile.PersonId,
                    (combined, profile) => new
                    {
                        ApplicationId = combined.application.Id,
                        combined.application.JobId,
                        combined.application.ApplicantId,
                        ApplicantName = combined.person.Fullname,
                        combined.person.Email,
                        combined.person.PhoneNumber,
                        combined.application.Status,
                        combined.application.CreatedAt,
                        combined.application.UpdatedAt,
                        Bio = profile.Bio,
                        Skills = profile.Skills,
                        Resume = profile.Resume
                    })
                .Where(a => a.JobId == jobId)
                .ToListAsync();
        }


        public async Task<Application> GetApplicationAsync(string applicantId, int jobId)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.ApplicantId == applicantId && a.JobId == jobId);
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


        public async Task DeleteApplicationAsync(Application application)
        {
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
        }
    }
}
