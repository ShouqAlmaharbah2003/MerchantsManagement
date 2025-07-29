using ApiLib.Controllers;
using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using CommonLib.Interfaces;
using CommonLib.Resources;
using DataLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;

namespace NUnitTesting.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IStringLocalizer<SharedResources>> _localizerMock;
        private Mock<ILogger<AuthController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _localizerMock = new Mock<IStringLocalizer<SharedResources>>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            // Mock localizer to return key as value
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns<string>(key => new LocalizedString(key, key));
        }

        [Test]
        public async Task Register_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "shouqAlmaharbah",
                Password = "SHOUQ*shouq!2003$",
                Email = "shouqalmaharbah@gmail.com"
            };
            var user = new User
            {
                Id = 1,
                Username = "shouqAlmaharbah"
            };
            _userServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(user);
            var controller = new AuthController(_userServiceMock.Object, _loggerMock.Object, _localizerMock.Object);

            // Act
            var result = await controller.Register(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
        }

        [Test]
        public async Task Register_InvalidRequest_ReturnsValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "",
                Password = "",
                Email = ""
            };
            var controller = new AuthController(_userServiceMock.Object, _loggerMock.Object, _localizerMock.Object);

            // Act
            var result = await controller.Register(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Register_UserAlreadyExists_ReturnsError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "shouqAlmaharbah",
                Password = "SHOUQ*shouq!2003$",
                Email = "shouqalmaharbah@gmail.com"
            };
            _userServiceMock.Setup(s => s.RegisterAsync(request)).ThrowsAsync(new InvalidOperationException());
            var controller = new AuthController(_userServiceMock.Object, _loggerMock.Object, _localizerMock.Object);

            // Act
            var result = await controller.Register(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Login_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "shouqAlmaharbah",
                Password = "SHOUQ*shouq!2003$"
            };
            var userResponse = new UserWithTokenResponse
            {
                Id = 1,
                Token = "tokenTokennnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn.tokemmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmn.tokenmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmn"
            };
            _userServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(userResponse);
            var controller = new AuthController(_userServiceMock.Object, _loggerMock.Object, _localizerMock.Object);

            // Act
            var result = await controller.Login(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
        }

        [Test]
        public async Task Login_UnauthorizedAccess_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "shouqAlmaharbah",
                Password = "SHOUQ*shouq"
            };
            _userServiceMock.Setup(s => s.LoginAsync(request)).ThrowsAsync(new UnauthorizedAccessException());
            var controller = new AuthController(_userServiceMock.Object, _loggerMock.Object, _localizerMock.Object);

            // Act
            var result = await controller.Login(request);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }
    }
}