using AutoMapper;
using BankSystem.Server.Domain.Entities;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;

namespace UnitTest
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IConfiguration> _mockConfig;
        private AuthService _authService;
        private BankDbContext _dbContext;

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory EF Core DB
            var options = new DbContextOptionsBuilder<BankDbContext>()
                        .UseNpgsql("Server=bank-application-server-001.postgres.database.azure.com;Database=postgres;Port=5432;User Id=app_bank;Password=admin1234!;Ssl Mode=Require;")
                        .Options;

            _dbContext = new BankDbContext(options);

            // Seed a test user
            var user = new User
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "testpass",
                FullName = "Test User",
                Role = "User",
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Mock configuration
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["JwtSettings:Key"]).Returns("MySuperDuperSecureSecretKey123456789!");
            mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
            mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
            mockConfig.Setup(c => c["JwtSettings:ExpireMinutes"]).Returns("30");

            // Create AuthService instance
            var mockRequestService = new Mock<RequestService>(_dbContext);
            var mockMapper = new Mock<IMapper>();

            _authService = new AuthService(_dbContext, mockMapper.Object, mockRequestService.Object, mockConfig.Object);

        }

        [TestMethod]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginServiceDto
            {
                UserName = "testuser",
                Password = "testpass"
            };

            // Act
            var result = await _authService.Login(loginDto);

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Content as LoginResponseServiceDto;
            Assert.IsNotNull(response);
            Assert.AreEqual("testuser", response.Username);
            Assert.IsFalse(string.IsNullOrEmpty(response.Token));
        }

        [TestMethod]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginServiceDto
            {
                UserName = "wrong",
                Password = "wrong"
            };

            // Act
            var result = await _authService.Login(loginDto);

            // Assert
            Assert.AreEqual(400, result.StatusCode);
        }
    }

}
