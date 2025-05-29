using AutoMapper;
using BankSystem.Server.Domain.Entities;
using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Dtos;
using BankSystem.Server.Services.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class MockTests
    {
        private Mock<IConfiguration> _mockConfig;
        private AuthService _authService;
        private BankDbContext _dbContext;
        private TransactionService _transactionService;
        private UserService _userService;

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
                Password = HashPassword("testpass", GenerateSalt()),
                FullName = "Test User",
                Role = "test",
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
            _transactionService = new TransactionService(_dbContext, mockRequestService.Object);
            _userService = new UserService(_dbContext, mockRequestService.Object);

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
        [TestMethod]
        public async Task RegisterAsync_WithEmptyUsernameOrPassword_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterServiceDto
            {
                Username = "",
                Password = "",
                Email = "user@example.com",
                FullName = "Test User",
                DateOfBirth = DateTime.UtcNow
            };

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("All fields are required.", result.ErrorMessage);
        }
        [TestMethod]
        public async Task RegisterAsync_WithValidInput_ReturnsCreated()
        {
            // Arrange
            var dto = new RegisterServiceDto
            {
                Username = "validuser",
                Password = HashPassword("securepass", GenerateSalt()),
                Email = "valid@example.com",
                FullName = "Valid User",
                DateOfBirth = DateTime.UtcNow
            };

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsTrue(result.Content.ToString().Contains("Registration successful"));
        }
        [TestMethod]
        public async Task GetAccountByUser_WithValidUserId_ReturnsAccounts()
        {
            // Arrange
            long userId = 42;
            var account = new BankAccount
            {
                UserId = userId,
                AccountNumber = "AC987654321",
                AccountType = "Savings",
                Currency = "USD",
                Balance = 1500,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.BankAccounts.Add(account);
            _dbContext.SaveChanges();

            var requestService = new RequestService(_dbContext);
            var bankAccountService = new BankAccountService(_dbContext, requestService);

            // Act
            var result = await bankAccountService.GetAccountByUser(userId.ToString());

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            var accounts = result.Content as List<BankAccount>;
            Assert.IsNotNull(accounts);
            Assert.AreEqual("Savings", accounts[0].AccountType);
        }
        [TestMethod]
        public async Task GetAccountByUser_WithNoAccounts_ReturnsBadRequest()
        {
            // Arrange
            long userId = 99; // Ensure this user has no accounts
            var requestService = new RequestService(_dbContext);
            var bankAccountService = new BankAccountService(_dbContext, requestService);

            // Act
            var result = await bankAccountService.GetAccountByUser(userId.ToString());

            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("No bank accounts!", result.ErrorMessage);
        }
        [TestMethod]
        public async Task GetAccountByUser_WithInvalidDbContext_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange: manually break context to simulate error
            var brokenService = new BankAccountService(null, new RequestService(_dbContext));

            // Act
            var result = await brokenService.GetAccountByUser("1");

            // Assert
            Assert.AreEqual(500, result.StatusCode);
            Assert.AreEqual("Internal server error!", result.ErrorMessage);
        }
        [TestMethod]
        public async Task CreateAccountAsync_WithValidInput_ReturnsCreated()
        {
            // Arrange
            var userId = 42;
            var requestService = new RequestService(_dbContext);
            var bankAccountService = new BankAccountService(_dbContext, requestService);

            var createDto = new CreateBankAccountServiceDto
            {
                UserId = userId,
                AccountType = "Savings",
                Currency = "USD",
                Balance = 5000
            };

            // Act
            var result = await bankAccountService.CreateAccountAsync(createDto);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.Created, result.StatusCode);
            Assert.IsTrue(result.Content.ToString().Contains("Account created successfully"));
        }
        [TestMethod]
        public async Task CreateAccountAsync_WithMissingFields_ReturnsBadRequest()
        {
            // Arrange
            var requestService = new RequestService(_dbContext);
            var bankAccountService = new BankAccountService(_dbContext, requestService);

            var createDto = new CreateBankAccountServiceDto
            {
                UserId = 43,
                AccountType = "", // Invalid
                Currency = "EUR",
                Balance = 100
            };

            // Act
            var result = await bankAccountService.CreateAccountAsync(createDto);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("All fields are required and balance must be greater than zero.", result.ErrorMessage);
        }
        [TestMethod]
        public async Task Deposit_ValidAccount_ReturnsOk()
        {
            var depositDto = new DepositServiceDto { UserId = "42", AccountNumber = "AC987654321", Amount = 200 };

            var result = await _transactionService.Deposit(depositDto);

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }
        [TestMethod]
        public async Task Deposit_InvalidAccount_ReturnsNotFound()
        { 

            var dto = new DepositServiceDto { UserId = "42", AccountNumber = "INVALID", Amount = 100 };
            var result = await _transactionService.Deposit(dto);

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }
        [TestMethod]
        public async Task Withdraw_ValidAccount_ReturnsOk()
        {
            var dto = new WithdrawServiceDto { UserId = "42", AccountNumber = "AC987654321", Amount = 100 };

            var result = await _transactionService.Withdraw(dto);

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }
        [TestMethod]
        public async Task Withdraw_InvalidAccount_ReturnsNotFound()
        {

            var dto = new WithdrawServiceDto { UserId = "42", AccountNumber = "INVALID", Amount = 50 };
            var result = await _transactionService.Withdraw(dto);

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }
        [TestMethod]
        public async Task GetTransactionsByUser_ShouldReturnBadRequest_WhenUserIdIsNull()
        {

            // Act
            var result = await _transactionService.GetTransactionsByUser(null);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }
        [TestMethod]
        public async Task GetTransactionsByUser_ShouldReturnNotFound_WhenNoTransactionsExist()
        {
            var userId = "43";

            var result = await _transactionService.GetTransactionsByUser(userId);

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }
        [TestMethod]
        public async Task GetTransactionsByUser_ShouldReturnOk_WithValidTransactions()
        {
            var userId = "3";
            var result = await _transactionService.GetTransactionsByUser(userId);

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }
        [TestMethod]
        public async Task GetUserById_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var userId = "1"; // Assuming this user exists in the seeded data
            // Act
            var result = await _userService.GetUserProfile(userId);
            // Assert
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Content);
        }
        [TestMethod]
        public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "999"; // Assuming this user does not exist
            // Act
            var result = await _userService.GetUserProfile(userId);
            // Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.IsNull(result.Content);
        }
        private static byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private static string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combine password and salt
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordWithSalt = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordWithSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordWithSalt, passwordBytes.Length, salt.Length);

                // Compute hash
                byte[] hashBytes = sha256.ComputeHash(passwordWithSalt);

                // Combine salt and hash for storage
                byte[] hashWithSaltBytes = new byte[salt.Length + hashBytes.Length];
                Buffer.BlockCopy(salt, 0, hashWithSaltBytes, 0, salt.Length);
                Buffer.BlockCopy(hashBytes, 0, hashWithSaltBytes, salt.Length, hashBytes.Length);

                // Convert to Base64 for storage
                return Convert.ToBase64String(hashWithSaltBytes);
            }
        }
    }
}

