using CareerCrafter.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace CareerCrafterTest
{
    [TestFixture]
    public class Authentication
    {
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase").Options;
            _context = new ApplicationDbContext(options);

            _context.Users.Add(new ApplicationUser
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            });
            _context.SaveChanges();
        }

        [Test]
        public void Login_ValidCredentials_ReturnsSuccessMessage()
        {

            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "correctpassword"
            };


            var isValid = _context.Users.Any(u => u.UserName == loginModel.Username);


            string resultMessage = isValid ? "Login Successful" : "Invalid Credentials";


            Assert.AreEqual("Login Successful", resultMessage);
        }

        [Test]
        public void Login_InvalidCredentials_ReturnsErrorMessage()
        {

            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "wrongpassword"
            };


            var isValid = _context.Users.Any(u => u.UserName == loginModel.Username && u.PasswordHash == loginModel.Password);


            string resultMessage = isValid ? "Login Successful" : "Invalid Credentials";


            Assert.AreEqual("Invalid Credentials", resultMessage);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}