using ApiLib.Controllers;
using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using CommonLib.Enums;
using CommonLib.Interfaces;
using CommonLib.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;

namespace NUnitTesting.Controllers
{
    [TestFixture]
    public class MerchantBranchesControllerTests
    {
        private Mock<IMerchantBranchesService> _serviceMock;
        private Mock<IStringLocalizer<SharedResources>> _localizerMock;
        private Mock<ILogger<MerchantBranchesController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IMerchantBranchesService>();
            _localizerMock = new Mock<IStringLocalizer<SharedResources>>();
            _loggerMock = new Mock<ILogger<MerchantBranchesController>>();

            // Mock localizer to return key as value
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns<string>(key => new LocalizedString(key, key));
        }

        [Test]
        public async Task CreateBranch_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var dto = new MerchantBranchRequest
            {
                BranchName_Ar = "فرع الرياض",
                BranchName_En = "Riyadh Branch",
                MerchantId = 1,
                ContactPersonId = 1,
                CityId = 1,
                GovernateId = 1,
                Address = "123 شارع الملك فهد، الرياض",
                Phone = "+966123456789",
                Mobile = "+966987654321",
                Gps = "24.7135517,46.6752957",
                Status = 1
            };
            _serviceMock.Setup(s => s.CreateBranchAsync(dto)).Returns(Task.CompletedTask);
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateBranch(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("BranchCreatedSuccessfully"));
        }

        [Test]
        public async Task CreateBranch_NullRequest_ReturnsValidationError()
        {
            // Arrange
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateBranch(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task CreateBranch_InvalidRequest_ReturnsValidationError()
        {
            // Arrange
            var dto = new MerchantBranchRequest
            {
                BranchName_Ar = "ف", // Too short (less than 2 characters)
                BranchName_En = "B", // Too short (less than 2 characters)
                MerchantId = 0, // Invalid (must be positive)
                ContactPersonId = 0, // Invalid (must be positive)
                CityId = 0, // Invalid (must be positive)
                GovernateId = 0, // Invalid (must be positive)
                Address = "123", // Too short (less than 5 characters)
                Phone = "123", // Invalid phone format
                Status = -1 // Invalid (must be non-negative)
            };
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateBranch(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task GetAllBranches_Success_ReturnsSuccess()
        {
            // Arrange
            var branches = new List<MerchantBranchResponse>
            {
                new MerchantBranchResponse
                {
                    Id = 1,
                    BranchName_Ar = "فرع الرياض",
                    BranchName_En = "Riyadh Branch",
                    Address = "123 شارع الملك فهد، الرياض",
                    Status = "Active"
                },
                new MerchantBranchResponse
                {
                    Id = 2,
                    BranchName_Ar = "فرع جدة",
                    BranchName_En = "Jeddah Branch",
                    Address = "456 شارع الحمراء، جدة",
                    Status = "Active"
                }
            };
            _serviceMock.Setup(s => s.GetAllBranchesAsync()).ReturnsAsync(branches);
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetAllBranches();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("BranchesFetchedSuccessfully"));
            Assert.That(((dynamic)okResult.Value).Data, Is.EqualTo(branches));
        }

        [Test]
        public async Task UpdateBranch_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var dto = new MerchantBranchRequest
            {
                BranchName_Ar = "فرع الرياض المعدل",
                BranchName_En = "Updated Riyadh Branch",
                MerchantId = 1,
                ContactPersonId = 1,
                CityId = 1,
                GovernateId = 1,
                Address = "123 شارع الملك فهد، الرياض",
                Phone = "+966123456789",
                Mobile = "+966987654321",
                Gps = "24.7135517,46.6752957",
                Status = 1
            };
            _serviceMock.Setup(s => s.UpdateBranchAsync(1, dto)).Returns(Task.CompletedTask);
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.UpdateBranch(1, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("BranchUpdatedSuccessfully"));
        }

        [Test]
        public async Task UpdateBranch_NullRequest_ReturnsValidationError()
        {
            // Arrange
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.UpdateBranch(1, null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task DeleteBranch_ValidId_ReturnsSuccess()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteBranchAsync(1)).Returns(Task.CompletedTask);
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.DeleteBranch(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("BranchDeletedSuccessfully"));
        }

        [Test]
        public async Task DeleteBranch_InvalidId_ReturnsError()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteBranchAsync(0)).ThrowsAsync(new Exception("Invalid branch ID"));
            var controller = new MerchantBranchesController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.DeleteBranch(0);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = (ObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(((dynamic)objectResult.Value).Message, Is.EqualTo("BranchDeletionError"));
        }
    }
}