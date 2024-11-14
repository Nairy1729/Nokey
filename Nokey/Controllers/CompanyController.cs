using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nokey.Models;
using Nokey.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Nokey.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCompany([FromBody] Company company)
        {
            if (string.IsNullOrEmpty(company.Name))
            {
                return BadRequest(new { message = "Company name is required.", success = false });
            }

            if (await _companyRepository.CompanyExistsAsync(company.Name))
            {
                return BadRequest(new { message = "You can't register the same company.", success = false });
            }

            try
            {
                company.PersonId = GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message, success = false });
            }

            var newCompany = await _companyRepository.RegisterCompanyAsync(company);
            return Created("", new { message = "Company registered successfully.", company = newCompany, success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            string userId;
            try
            {
                userId = GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message, success = false });
            }

            var companies = await _companyRepository.GetCompaniesByUserIdAsync(userId);

            if (companies == null)
            {
                return NotFound(new { message = "Companies not found.", success = false });
            }

            return Ok(new { companies, success = true });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(id);

            if (company == null)
            {
                return NotFound(new { message = "Company not found.", success = false });
            }

            return Ok(new { company, success = true });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromForm] Company updatedCompany, IFormFile logoFile)
        {
            if (updatedCompany == null)
            {
                return BadRequest(new { message = "Updated company details are required.", success = false });
            }

            if (logoFile != null)
            {
                var logoUri = await UploadLogoToCloudinary(logoFile);
                updatedCompany.Logo = logoUri;
            }

            var updated = await _companyRepository.UpdateCompanyAsync(id, updatedCompany);

            if (updated == null)
            {
                return NotFound(new { message = "Company not found.", success = false });
            }

            return Ok(new { message = "Company information updated.", success = true });
        }

        private string GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID is not available in token claims.");
            }

            return userIdClaim; // Return as string (no need to parse it as int)
        }


        private async Task<string> UploadLogoToCloudinary(IFormFile file)
        {
            // Placeholder upload logic; replace with actual implementation
            return "logo-url"; // Placeholder for actual Cloudinary URL
        }
    }
}
