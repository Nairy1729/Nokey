using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nokey.Models;

namespace Nokey.Repositories
{
    public interface IJobPostRepository
    {
        Task<Job> PostJobAsync(Job job);
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<Job> GetJobByIdAsync(int jobId);
        Task<IEnumerable<Job>> GetAdminJobsAsync(string adminId);
        Task<Job> UpdateJobAsync(Job job);
        Task<bool> DeleteJobAsync(int jobId);




    }
}
