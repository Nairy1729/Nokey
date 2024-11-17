// CompanyRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nokey.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Nokey.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Nokey.Controllers.AuthenticationController;

namespace Nokey.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> RegisterCompanyAsync(Company company, string personId)
        {
            if (string.IsNullOrEmpty(personId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            company.PersonId = personId;

            _context.Companies.Add(company);

            await _context.SaveChangesAsync();

            return company;
        }




        public async Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(string userId)
        {
            return await _context.Companies.Where(c => c.PersonId == userId).ToListAsync();
        }

        //public async Task<Company> GetCompanyByIdAsync(int companyId)
        //{
        //    return await _context.Companies.FindAsync(companyId);
        //}

        public async Task<Company> UpdateCompanyAsync(int companyId, Company updatedCompany)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
                return null;

            company.Name = updatedCompany.Name;
            company.Description = updatedCompany.Description;
            company.Website = updatedCompany.Website;
            company.Location = updatedCompany.Location;
            company.Logo = updatedCompany.Logo;

            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> CompanyExistsAsync(string companyName)
        {
            return await _context.Companies.AnyAsync(c => c.Name == companyName);
        }

        public async Task<Company> GetCompanyByIdAsync(int companyId)
        {
            return await _context.Companies
                                 .Where(c => c.Id == companyId)
                                 .FirstOrDefaultAsync();
        }

    }
}
