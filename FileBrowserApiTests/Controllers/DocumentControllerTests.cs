using FileBrowserAPI.Controllers;
using FileBrowserAPI.Models;
using FileBrowserAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBrowserApiTests.Controllers
{
    [TestFixture]
    public class DocumentControllerTests
    {
        [Test]
        public void WhenFilelistRequested_ThenFilenamesReturnAsStringArray()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var fileList = new string[] { "one.jpg", "two.jpg" };
            var fileHandlingResultMock = new FileHandlingResult<string[]>(true, fileList);
            dataRepositoryMock.Setup(r => r.GetAllFiles()).Returns(fileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = controller.Get();
            var okResult = resposne as OkObjectResult;

            // Assert
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(fileList));
        }

        [Test]
        public void WhenFilelistRequested_AndErrorHappens_ThenBadRequestReturns()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var fileHandlingResultMock = new FileHandlingResult<string[]?>(false, null, "Error");

            dataRepositoryMock.Setup(r => r.GetAllFiles()).Returns(fileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = controller.Get();
            var badRequestResult = resposne as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Error"));
        }

        [Test]
        public async Task WhenFileRequested_ThenTheFileReturnsAsBase64()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var base64Result = new Base64Result("abc", "two.jpg");
            var fileHandlingResultMock = new FileHandlingResult<Base64Result?>(true, base64Result);

            dataRepositoryMock.Setup(r => r.GetFileAsBase64Async("two.jpg")).ReturnsAsync(fileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = await controller.GetAsBase64Async("two.jpg");
            var okResult = resposne as OkObjectResult;

            // Assert
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(base64Result));
        }

        [Test]
        public async Task WhenFileRequestedAsBase64_AndErrorHappens_ThenBadRequestReturns()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var fileHandlingResultMock = new FileHandlingResult<Base64Result?>(false, null, "Error");

            dataRepositoryMock.Setup(r => r.GetFileAsBase64Async("two.jpg")).ReturnsAsync(fileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = await controller.GetAsBase64Async("two.jpg");
            var badRequestResult = resposne as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Error"));
        }


        [Test]
        public async Task WhenFileRequested_ThenTheFileReturns()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var result = new ResultFile(new byte[] {1,2,3}, "two.jpg", "image/jpeg");
            var fileHandlingResultMock = new FileHandlingResult<ResultFile?>(true, result);

            dataRepositoryMock.Setup(r => r.GetFileAsync("two.jpg")).ReturnsAsync(fileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = await controller.GetAsync("two.jpg");
            var okResult = resposne as FileContentResult;

            // Assert
            Assert.That(okResult.ContentType, Is.EqualTo(result.ContentType));
            Assert.That(okResult.FileContents, Is.EqualTo(result.FileContent));
            Assert.That(okResult.FileDownloadName, Is.EqualTo(result.Filename));
        }

        [Test]
        public async Task WhenFileRequested_AndErrorHappens_ThenBadRequestReturns()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var fileHandlingResultMock = new FileHandlingResult<ResultFile?>(false, null, "Error");

            var base64FileHandlingResultMock = new FileHandlingResult<Base64Result?>(false, null, "Error");

            dataRepositoryMock.Setup(r => r.GetFileAsync("two.jpg")).ReturnsAsync(fileHandlingResultMock);
            dataRepositoryMock.Setup(r => r.GetFileAsBase64Async("two.jpg")).ReturnsAsync(base64FileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = await controller.GetAsBase64Async("two.jpg");
            var badRequestResult = resposne as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Error"));
        }


        [Test]
        public async Task WhenFilePosted_ThenOkMessageReturns()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var fileHandlingResultMock = new FileHandlingResult<Type?>(true, null);

            var fileList = new List<IFormFile>() { Mock.Of<IFormFile>() };

            dataRepositoryMock.Setup(r => r.StoreFileAsync(fileList[0])).ReturnsAsync(fileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = await controller.PostAsync(fileList);
            var okResult = resposne as OkResult;

            // Assert
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }


        [Test]
        public async Task WhenFilesPosted_ThenBadRequestMessageReturns()
        {
            // Arrange
            var fileList = new List<IFormFile>() { Mock.Of<IFormFile>(), Mock.Of<IFormFile>() };

            var controller = new DocumentController(Mock.Of<IDataRepository>());

            // Act
            var resposne = await controller.PostAsync(fileList);
            var badRequestResult = resposne as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Currently only one file is accepted for uploading"));
        }

        [Test]
        public async Task WhenFileIsNotPosted_ThenBadRequestMessageReturns()
        {
            // Arrange
            var fileList = new List<IFormFile>() { };

            var controller = new DocumentController(Mock.Of<IDataRepository>());

            // Act
            var resposne = await controller.PostAsync(fileList);
            var badRequestResult = resposne as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("No file is attached. The parameter name must be 'files' "));
        }

        [Test]
        public async Task WhenFilePosted_AndCannotSave_ThenBadRequestMessageReturns()
        {
            // Arrange
            Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
            var fileHandlingResultMock = new FileHandlingResult<Type?>(false, null, "Error");

            var fileList = new List<IFormFile>() { Mock.Of<IFormFile>() };

            dataRepositoryMock.Setup(r => r.StoreFileAsync(fileList[0])).ReturnsAsync(fileHandlingResultMock);

            var controller = new DocumentController(dataRepositoryMock.Object);

            // Act
            var resposne = await controller.PostAsync(fileList);
            var badRequestResult = resposne as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Error"));
        }
    }
}
