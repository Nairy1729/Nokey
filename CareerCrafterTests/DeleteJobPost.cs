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
    internal class DeleteJobPost
    {

        private Mock<IJobPostRepository> _mockJobPostRepository;
        private Mock<ICompanyRepository> _mockCompanyRepository;
        private JobPostController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockJobPostRepository = new Mock<IJobPostRepository>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();

            _controller = new JobPostController(_mockCompanyRepository.Object, _mockJobPostRepository.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("UserId", "user123") 
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task DeleteJob_ExistingJobId_ReturnsOk()
        {
            int jobId = 1;
            _mockJobPostRepository
                .Setup(repo => repo.GetJobByIdAsync(jobId))
                .ReturnsAsync(new Job { Id = jobId, CreatedById = "user123" });
            _mockJobPostRepository
                .Setup(repo => repo.DeleteJobAsync(jobId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteJob(jobId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task DeleteJob_UnauthorizedUser_ReturnsForbidden()
        {
            int jobId = 1;
            _mockJobPostRepository
                .Setup(repo => repo.GetJobByIdAsync(jobId))
                .ReturnsAsync(new Job { Id = jobId, CreatedById = "differentUser" });

            var result = await _controller.DeleteJob(jobId) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(403, result.StatusCode);
        }

        [Test]
        public async Task DeleteJob_NonExistingJobId_ReturnsNotFound()
        {
            int jobId = 99;
            _mockJobPostRepository
                .Setup(repo => repo.GetJobByIdAsync(jobId))
                .ReturnsAsync((Job)null);

            var result = await _controller.DeleteJob(jobId) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}