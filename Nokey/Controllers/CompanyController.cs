using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IO;
using CareerCrafter.Repositories;
using CareerCrafter.Models;
using Microsoft.AspNetCore.Cors;

namespace CareerCrafter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpPost("register-company")]
        public async Task<IActionResult> RegisterCompany([FromBody] Company company)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(company.LogoUrl))
                {
                    return BadRequest(new { message = "Logo URL is required.", success = false });
                }

                if (!Uri.IsWellFormedUriString(company.LogoUrl, UriKind.Absolute))
                {
                    return BadRequest(new { message = "Invalid Logo URL format.", success = false });
                }

                var personId = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(personId))
                {
                    return Unauthorized(new { message = "User is not authenticated." });
                }

                company.PersonId = personId;

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
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] Company updatedCompany)
        {
            if (updatedCompany == null)
            {
                return BadRequest(new { message = "Updated company details are required.", success = false });
            }

            if (!string.IsNullOrWhiteSpace(updatedCompany.LogoUrl) &&
                !Uri.IsWellFormedUriString(updatedCompany.LogoUrl, UriKind.Absolute))
            {
                return BadRequest(new { message = "Invalid Logo URL format.", success = false });
            }

            try
            {
                var updated = await _companyRepository.UpdateCompanyAsync(id, updatedCompany);

                if (updated == null)
                {
                    return NotFound(new { message = "Company not found.", success = false });
                }

                return Ok(new { message = "Company information updated.", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
        }

    }
}
