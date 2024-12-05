using CareerCrafter.Controllers;
using CareerCrafter.Models;
using CareerCrafter.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CareerCrafterTests
{
    internal class RegisterComanyTest
    {


        private Mock<ICompanyRepository> _mockCompanyRepository;
        private CompanyController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();


            _controller = new CompanyController(_mockCompanyRepository.Object);


            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("UserId", "test-user-id")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task RegisterCompany_ValidData_ReturnsCreatedAtActionResult()
        {
            var company = new Company
            {
                Id = 0,
                Name = "Test Company",
                Description = "A test description",
                Website = "https://example.com",
                Location = "Test Location",
                LogoUrl = "https://example.com/logo.png",
                PersonId = "test-user-id"
            };

            _mockCompanyRepository
                .Setup(repo => repo.RegisterCompanyAsync(It.IsAny<Company>(), "test-user-id"))
                .ReturnsAsync(new Company
                {
                    Id = 1, 
                    Name = company.Name,
                    Description = company.Description,
                    Website = company.Website,
                    Location = company.Location,
                    LogoUrl = company.LogoUrl,
                    PersonId = company.PersonId
                });

            var result = await _controller.RegisterCompany(company);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);

            var returnedCompany = createdAtActionResult.Value as Company;
            Assert.IsNotNull(returnedCompany);
            Assert.AreEqual("Test Company", returnedCompany.Name);
        }

        [Test]
        public async Task RegisterCompany_InvalidLogoUrl_ReturnsBadRequest()
        {
            var company = new Company
            {
                Id = 0,
                Name = "Test Company",
                Description = "A test description",
                Website = "https://example.com",
                Location = "Test Location",
                LogoUrl = "invalid-url", 
                PersonId = "test-user-id"
            };

            var result = await _controller.RegisterCompany(company);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task RegisterCompany_NoUserId_ReturnsUnauthorized()
        {
            var company = new Company
            {
                Id = 0,
                Name = "Test Company",
                Description = "A test description",
                Website = "https://example.com",
                Location = "Test Location",
                LogoUrl = "https://example.com/logo.png"
            };

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            var result = await _controller.RegisterCompany(company);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
    }
}