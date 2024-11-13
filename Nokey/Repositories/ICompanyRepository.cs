// ICompanyRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Nokey.Models;

namespace Nokey.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company> RegisterCompanyAsync(Company company);
        Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(int userId);
        Task<Company> GetCompanyByIdAsync(int companyId);
        Task<Company> UpdateCompanyAsync(int companyId, Company updatedCompany);
        Task<bool> CompanyExistsAsync(string companyName);
    }
}
