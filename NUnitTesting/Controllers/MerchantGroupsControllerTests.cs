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
    public class MerchantGroupsControllerTests
    {
        private Mock<IMerchantGroupsService> _serviceMock;
        private Mock<IStringLocalizer<SharedResources>> _localizerMock;
        private Mock<ILogger<MerchantGroupsController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IMerchantGroupsService>();
            _localizerMock = new Mock<IStringLocalizer<SharedResources>>();
            _loggerMock = new Mock<ILogger<MerchantGroupsController>>();

            // Mock localizer to return key as value
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns<string>(key => new LocalizedString(key, key));
        }

        [Test]
        public async Task CreateGroup_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var dto = new MerchantGroupRequest
            {
                Name_Ar = "مجموعة التجار",
                Name_En = "Merchants Group"
            };
            _serviceMock.Setup(s => s.CreateGroupAsync(dto)).Returns(Task.CompletedTask);
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateGroup(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("GroupCreatedSuccessfully"));
        }

        [Test]
        public async Task CreateGroup_NullRequest_ReturnsValidationError()
        {
            // Arrange
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateGroup(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task CreateGroup_InvalidRequest_ReturnsValidationError()
        {
            // Arrange
            var dto = new MerchantGroupRequest
            {
                Name_Ar = "م", // Too short (less than 2 characters)
                Name_En = "G" // Too short (less than 2 characters)
            };
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateGroup(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task GetAllGroups_Success_ReturnsSuccess()
        {
            // Arrange
            var groups = new List<MerchantGroupResponse>
            {
                new MerchantGroupResponse
                {
                    Id = 1,
                    Name_Ar = "مجموعة التجار",
                    Name_En = "Merchants Group",
                    Status = "Active"
                },
                new MerchantGroupResponse
                {
                    Id = 2,
                    Name_Ar = "مجموعة المتاجر",
                    Name_En = "Stores Group",
                    Status = "Active"
                }
            };
            _serviceMock.Setup(s => s.GetAllGroupsAsync()).ReturnsAsync(groups);
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetAllGroups();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("GroupsFetchedSuccessfully"));
            Assert.That(((dynamic)okResult.Value).Data, Is.EqualTo(groups));
        }

        [Test]
        public async Task GetActiveGroups_Success_ReturnsSuccess()
        {
            // Arrange
            var groups = new List<MerchantGroupResponse>
            {
                new MerchantGroupResponse
                {
                    Id = 1,
                    Name_Ar = "مجموعة التجار",
                    Name_En = "Merchants Group",
                    Status = "Active"
                }
            };
            _serviceMock.Setup(s => s.GetActiveGroupsAsync()).ReturnsAsync(groups);
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetActiveGroups();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("ActiveGroupsFetchedSuccessfully"));
            Assert.That(((dynamic)okResult.Value).Data, Is.EqualTo(groups));
        }

        [Test]
        public async Task UpdateGroupName_ValidRequestArabic_ReturnsSuccess()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateGroupName_ArAsync(1, "مجموعة جديدة")).Returns(Task.CompletedTask);
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.UpdateGroupName(1, "مجموعة جديدة", "ar");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("GroupNameUpdatedSuccessfully"));
        }

        [Test]
        public async Task UpdateGroupName_ValidRequestEnglish_ReturnsSuccess()
        {
            // Arrange
            _serviceMock.Setup(s => s.UpdateGroupName_EnAsync(1, "New Group")).Returns(Task.CompletedTask);
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.UpdateGroupName(1, "New Group", "en");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("GroupNameUpdatedSuccessfully"));
        }

        [Test]
        public async Task UpdateGroupName_InvalidLanguage_ReturnsError()
        {
            // Arrange
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.UpdateGroupName(1, "NewName", "fr");

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Message, Is.EqualTo("InvalidLanguage"));
        }

        [Test]
        public async Task UpdateGroupName_NullOrEmptyName_ReturnsValidationError()
        {
            // Arrange
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.UpdateGroupName(1, "", "ar");

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task DeleteGroup_ValidId_ReturnsSuccess()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteGroupAsync(1)).Returns(Task.CompletedTask);
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.DeleteGroup(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("GroupDeletedSuccessfully"));
        }

        [Test]
        public async Task DeleteGroup_InvalidId_ReturnsError()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteGroupAsync(0)).ThrowsAsync(new Exception("Invalid group ID"));
            var controller = new MerchantGroupsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.DeleteGroup(0);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = (ObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(((dynamic)objectResult.Value).Message, Is.EqualTo("GroupDeletionError"));
        }
    }
}