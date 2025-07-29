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
    public class MerchantsControllerTests
    {
        private Mock<IMerchantsService> _serviceMock;
        private Mock<IStringLocalizer<SharedResources>> _localizerMock;
        private Mock<ILogger<MerchantsController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IMerchantsService>();
            _localizerMock = new Mock<IStringLocalizer<SharedResources>>();
            _loggerMock = new Mock<ILogger<MerchantsController>>();

            // Mock localizer to return key as value
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns<string>(key => new LocalizedString(key, key));
        }

        [Test]
        public async Task CreateMerchant_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var dto = new MerchantRequest
            {
                Name_Ar = "تاجر الرياض",
                Name_En = "Riyadh Merchant",
                BusinessType = 1,
                MerchantGroupId = 1,
                ManagerName = "أحمد محمد",
                Status = 1
            };
            _serviceMock.Setup(s => s.CreateMerchantAsync(dto)).Returns(Task.CompletedTask);
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateMerchant(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("MerchantCreatedSuccessfully"));
        }

        [Test]
        public async Task CreateMerchant_NullRequest_ReturnsValidationError()
        {
            // Arrange
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateMerchant(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task CreateMerchant_InvalidRequest_ReturnsValidationError()
        {
            // Arrange
            var dto = new MerchantRequest
            {
                Name_Ar = "ت", // Too short (less than 2 characters)
                Name_En = "M", // Too short (less than 2 characters)
                BusinessType = 0, // Invalid (must be positive)
                MerchantGroupId = 0, // Invalid (must be positive)
                Status = -1 // Invalid (must be non-negative)
            };
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateMerchant(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }

        [Test]
        public async Task CreateMerchant_MerchantAlreadyExists_ReturnsError()
        {
            // Arrange
            var dto = new MerchantRequest
            {
                Name_Ar = "تاجر الرياض",
                Name_En = "Riyadh Merchant",
                BusinessType = 1,
                MerchantGroupId = 1,
                Status = 1
            };
            _serviceMock.Setup(s => s.CreateMerchantAsync(dto)).ThrowsAsync(new InvalidOperationException());
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.CreateMerchant(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = (ObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(((dynamic)objectResult.Value).Message, Is.EqualTo("MerchantCreationError"));
        }

        [Test]
        public async Task GetActiveMerchants_Success_ReturnsSuccess()
        {
            // Arrange
            var merchants = new List<MerchantResponse>
            {
                new MerchantResponse
                {
                    Id = 1,
                    Name_Ar = "تاجر الرياض",
                    Name_En = "Riyadh Merchant",
                    BusinessType = 1,
                    ManagerName = "أحمد محمد",
                    Status = "Active"
                },
                new MerchantResponse
                {
                    Id = 2,
                    Name_Ar = "تاجر جدة",
                    Name_En = "Jeddah Merchant",
                    BusinessType = 2,
                    ManagerName = "خالد علي",
                    Status = "Active"
                }
            };
            _serviceMock.Setup(s => s.GetActiveMerchantsAsync()).ReturnsAsync(merchants);
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.GetActiveMerchants();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("MerchantsFetchedSuccessfully"));
            Assert.That(((dynamic)okResult.Value).Data, Is.EqualTo(merchants));
        }

        [Test]
        public async Task SearchMerchants_ValidQuery_ReturnsSuccess()
        {
            // Arrange
            var merchants = new List<MerchantResponse>
            {
                new MerchantResponse
                {
                    Id = 1,
                    Name_Ar = "تاجر الرياض",
                    Name_En = "Riyadh Merchant",
                    BusinessType = 1,
                    Status = "Active"
                }
            };
            _serviceMock.Setup(s => s.SearchMerchantsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(merchants);
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.SearchMerchants("تاجر", "123", 1, "branch");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("MerchantsFetchedSuccessfully"));
            Assert.That(((dynamic)okResult.Value).Data, Is.EqualTo(merchants));
        }

        [Test]
        public async Task SearchMerchantsAsPdf_Success_ReturnsFile()
        {
            // Arrange
            var htmlContent = "<html><body><h1>Search Results</h1></body></html>";
            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // Simulated PDF header
            _serviceMock.Setup(s => s.GenerateSearchResultsHtmlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(htmlContent);
            _serviceMock.Setup(s => s.ConvertToPDF(htmlContent)).Returns(pdfBytes);
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.SearchMerchantsAsPdf("تاجر", "123", 1, "branch");

            // Assert
            Assert.That(result, Is.InstanceOf<FileContentResult>());
            var fileResult = (FileContentResult)result;
            Assert.That(fileResult.ContentType, Is.EqualTo("application/pdf"));
            Assert.That(fileResult.FileContents, Is.EqualTo(pdfBytes));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("SearchResults.pdf"));
        }

        [Test]
        public async Task SearchMerchantsAsPdf_ServiceThrowsException_ReturnsError()
        {
            // Arrange
            _serviceMock.Setup(s => s.GenerateSearchResultsHtmlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error generating PDF"));
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.SearchMerchantsAsPdf("تاجر", "123", 1, "branch");

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = (ObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(((dynamic)objectResult.Value).Message, Is.EqualTo("PDFGenerationError"));
        }

        [Test]
        public async Task ChangeMerchantGroupId_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            _serviceMock.Setup(s => s.ChangeMerchantGroupIdAsync(1, 2)).Returns(Task.CompletedTask);
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.ChangeMerchantGroupId(1, 2);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(((dynamic)okResult.Value).Message, Is.EqualTo("MerchantGroupUpdatedSuccessfully"));
        }

        [Test]
        public async Task ChangeMerchantGroupId_InvalidIds_ReturnsValidationError()
        {
            // Arrange
            var controller = new MerchantsController(_serviceMock.Object, _localizerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.ChangeMerchantGroupId(0, 0);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(((dynamic)badRequestResult.Value).Code, Is.EqualTo(ErrorCodes.ValidationError));
        }
    }
}