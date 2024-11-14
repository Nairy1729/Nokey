using Nokey.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nokey.Repositories
{
    public interface IApplicationRepository
    {
        Task<Application> FindApplicationAsync(int jobId, string userId);
        //Task<Application> FindApplicationAsync(int jobId, int userId);
        Task<Application> CreateApplicationAsync(Application application);
        Task<IEnumerable<Application>> GetApplicationsByApplicantAsync(string userId);
        Task<Application> GetApplicationByIdAsync(int applicationId);
        Task UpdateApplicationStatusAsync(Application application, string status);
    }
}
