using System.Collections.Generic;
using System.Threading.Tasks;
using CareerCrafter.Models;
using Microsoft.AspNetCore.Mvc;

namespace CareerCrafter.Repositories
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
