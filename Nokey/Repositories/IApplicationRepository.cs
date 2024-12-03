using CareerCrafter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerCrafter.Repositories
{
    public interface IApplicationRepository
    {
        Task<Application> FindApplicationAsync(int jobId, string userId);
        Task<Application> CreateApplicationAsync(Application application);
        //Task<IEnumerable<Application>> GetApplicationsByApplicantAsync(string userId);
        Task<Application> GetApplicationByIdAsync(int applicationId);
        //Task UpdateApplicationStatusAsync(Application application, string status);
        Task<IEnumerable<object>> GetApplicantsByJobAsync(int jobId);

        Task<Application> GetApplicationAsync(string applicantId, int jobId);
        Task UpdateApplicationStatusAsync(Application application, string status);

        Task DeleteApplicationAsync(Application application);

        Task<IEnumerable<object>> GetApplicationsByApplicantAsync(string userId);
    }
}
