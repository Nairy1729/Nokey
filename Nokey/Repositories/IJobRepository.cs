using CareerCrafter.Models;
using System.Threading.Tasks;

namespace CareerCrafter.Repositories
{
    public interface IJobRepository
    {
        Task<Job> FindJobByIdAsync(int jobId);
        Task AddApplicationToJobAsync(Job job, Application application);
        Task<IEnumerable<Job>> SearchJobsAsync(string query);
    }
}
