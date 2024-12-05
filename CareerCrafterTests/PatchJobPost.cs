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
    internal class PatchJobpost
    {

        private Mock<IJobPostRepository> _mockJobPostRepository;
        private Mock<ICompanyRepository> _mockCompanyRepository;
        private JobPostController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockJobPostRepository = new Mock<IJobPostRepository>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();

            _controller = new JobPostController(
                _mockCompanyRepository.Object,
                _mockJobPostRepository.Object
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("UserId", "12345") 
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task UpdateJob_ExistingJob_ReturnsOk()
        {
            var jobId = 1;
            var existingJob = new Job
            {
                Id = jobId,
                Title = "Software Engineer",
                Description = "Develop software",
                Salary = 60000,
                CreatedById = "12345"
            };

            var updatedJob = new Job
            {
                Id = jobId,
                Title = "Senior Software Engineer",
                Description = "Develop advanced software",
                Salary = 80000
            };

            _mockJobPostRepository.Setup(repo => repo.GetJobByIdAsync(jobId))
                .ReturnsAsync(existingJob);

            _mockJobPostRepository.Setup(repo => repo.UpdateJobAsync(It.IsAny<Job>()))
                .ReturnsAsync(updatedJob);

            var result = await _controller.UpdateJob(jobId, updatedJob);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult?.Value);
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateJob_NonExistingJobId_ReturnsNotFound()
        {
            var jobId = 99; 
            var updatedJob = new Job
            {
                Id = jobId,
                Title = "Senior Software Engineer",
                Description = "Develop advanced software",
                Salary = 80000
            };

            _mockJobPostRepository.Setup(repo => repo.GetJobByIdAsync(jobId))
                .ReturnsAsync((Job)null);

            var result = await _controller.UpdateJob(jobId, updatedJob);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateJob_UnauthorizedUser_ReturnsUnauthorized()
        {
            var jobId = 1;
            var existingJob = new Job
            {
                Id = jobId,
                Title = "Software Engineer",
                Description = "Develop software",
                Salary = 60000,
                CreatedById = "98765" 
            };

            var updatedJob = new Job
            {
                Id = jobId,
                Title = "Senior Software Engineer",
                Description = "Develop advanced software",
                Salary = 80000
            };

            _mockJobPostRepository.Setup(repo => repo.GetJobByIdAsync(jobId))
                .ReturnsAsync(existingJob);

            var result = await _controller.UpdateJob(jobId, updatedJob);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ObjectResult>(result);

            var unauthorizedResult = result as ObjectResult;
            Assert.That(unauthorizedResult?.StatusCode, Is.EqualTo(403));
        }
    }
}