using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CareerCrafter.Models;

namespace CareerCrafter.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company> RegisterCompanyAsync(Company company, string personId);
        Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(string userId);
        Task<Company> GetCompanyByIdAsync(int companyId);
        Task<Company> UpdateCompanyAsync(int companyId, Company updatedCompany);
        Task<bool> CompanyExistsAsync(string companyName);
    }
}
