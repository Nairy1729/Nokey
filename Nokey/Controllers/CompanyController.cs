using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nokey.Models;
using Nokey.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IO;

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

        [HttpPost("register-company")]
        public async Task<IActionResult> RegisterCompany([FromForm] Company company, IFormFile logoFile)
        {
            try
            {
                if (logoFile == null || logoFile.Length == 0)
                {
                    return BadRequest(new { message = "Logo file is required.", success = false });
                }

                if (logoFile.ContentType != "image/jpeg" && logoFile.ContentType != "image/jpg")
                {
                    return BadRequest(new { message = "Only JPG or JPEG file formats are allowed for the logo.", success = false });
                }

                if (logoFile.Length > 2 * 1024 * 1024) // 2MB limit
                {
                    return BadRequest(new { message = "Logo file size must be less than 2MB.", success = false });
                }

                using (var memoryStream = new MemoryStream())
                {
                    await logoFile.CopyToAsync(memoryStream);
                    company.Logo = memoryStream.ToArray(); // Store the logo as a byte array
                }

                var personId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(personId))
                {
                    return Unauthorized(new { message = "User is not authenticated." });
                }

                // Register the company
                var result = await _companyRepository.RegisterCompanyAsync(company, personId);
                return CreatedAtAction(nameof(GetCompanyById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            string personId;
            try
            {
                personId = User.FindFirst("UserId")?.Value;
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message, success = false });
            }

            var companies = await _companyRepository.GetCompaniesByUserIdAsync(personId);

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

            if (logoFile != null && logoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await logoFile.CopyToAsync(memoryStream);
                    updatedCompany.Logo = memoryStream.ToArray(); 
                }
            }

            var updated = await _companyRepository.UpdateCompanyAsync(id, updatedCompany);

            if (updated == null)
            {
                return NotFound(new { message = "Company not found.", success = false });
            }

            return Ok(new { message = "Company information updated.", success = true });
        }
    }
}
