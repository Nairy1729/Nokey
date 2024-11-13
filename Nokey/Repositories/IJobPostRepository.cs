// IJobRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Nokey.Models;

namespace Nokey.Repositories
{
    public interface IJobPostRepository
    {
        Task<Job> PostJobAsync(Job job);
        Task<IEnumerable<Job>> GetAllJobsAsync(string keyword);
        Task<Job> GetJobByIdAsync(int jobId);
        Task<IEnumerable<Job>> GetAdminJobsAsync(int adminId);
    }
}
