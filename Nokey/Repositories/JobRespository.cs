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
                    //Applications = _context.Applications
                    //    .Where(a => a.JobId == j.Id)
                    //    .Select(a => new Application
                    //    {
                    //        Id = a.Id,
                    //        ApplicantId = a.ApplicantId,
                    //        JobId = a.JobId,
                    //        // Add other necessary fields from Application here
                    //        Applicant = _context.Persons.FirstOrDefault(p => p.Id == a.ApplicantId) // You can adjust this based on how you fetch the applicant details
                    //    })
                    //    .ToList()
                })
                .FirstOrDefaultAsync();
        }


        public async Task AddApplicationToJobAsync(Job job, Application application)
        {
            // Manually set the JobId for the application
            application.JobId = job.Id;

            // Add the application to the database (no need to add it to the Job's Applications collection since it's removed)
            _context.Applications.Add(application);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

    }
}
