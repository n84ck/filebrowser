using FileBrowserAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

namespace FileBrowserApiTests.Repository
{
    [TestFixture]
    internal class FileRepositoryTests
    {
        Mock<IConfiguration> configurationMock;
        MockFileSystem fileSystemMock;

        [SetUp]
        public void Setup()
        {
            configurationMock = new Mock<IConfiguration>();

            Mock<IConfigurationSection> filesPathMock = new Mock<IConfigurationSection>();
            filesPathMock.SetupGet(x => x.Value).Returns(@"c:\testPath");
            Mock<IConfigurationSection> maxFileSizeMock = new Mock<IConfigurationSection>();
            maxFileSizeMock.SetupGet(x => x.Value).Returns("10"); // 10 byte

            configurationMock.Setup(c => c.GetSection("FilePath")).Returns(filesPathMock.Object);
            configurationMock.Setup(c => c.GetSection("MaxFileSize")).Returns(maxFileSizeMock.Object);

            fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\testPath\myfile.txt", new MockFileData("Example") },
                { @"c:\testPath\jQuery.js", new MockFileData("data")},
                { @"c:\testPath\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            }, @"c:\testPath");
        }

        [Test]
        public void WhenGetAllFiles_ThenAllFilesAreListed()
        {
            // Arrange
            var fileRepository = new FileRepository(configurationMock.Object, fileSystemMock);

            // Act
            var files = fileRepository.GetAllFiles();
            
            // Assert
            Assert.That(files.IsSuccessful, Is.EqualTo(true));
            Assert.That(files.Result.Length, Is.EqualTo(3));
            Assert.That(files.Result[2].Length, Is.EqualTo(9));
        }

        [Test]
        public async Task WhenExistingFileRequested_ThenTheFileReturns()
        {
            // Arrange
            var fileRepository = new FileRepository(configurationMock.Object, fileSystemMock);

            // Act
            var file = await fileRepository.GetFileAsync(@"c:\testPath\image.gif");

            // Assert
            Assert.That(file.IsSuccessful, Is.EqualTo(true));
            Assert.That(file.Result.Filename, Is.EqualTo("image.gif"));
            Assert.That(file.Result.ContentType, Is.EqualTo("image/gif"));
            Assert.That(file.Result.FileContent, Is.EqualTo(new byte[] { 0x12, 0x34, 0x56, 0xd2 }));
        }

        [Test]
        public async Task WhenNonExistingFileRequested_ThenErrorMessageReturns()
        {
            // Arrange
            var fileRepository = new FileRepository(configurationMock.Object, fileSystemMock);

            // Act
            var file = await fileRepository.GetFileAsync(@"c:\testPath\image2.gif");

            // Assert
            Assert.That(file.IsSuccessful, Is.EqualTo(false));
            Assert.That(file.ErrorMessage, Is.EqualTo("The file does not exists"));
        }

        [Test]
        public async Task WhenFileStoreRequested_ThenFileIsStored()
        {
            // Arrange
            Mock<IFormFile> formFileMock = new Mock<IFormFile>();
            formFileMock.SetupGet(r => r.Length).Returns(5);
            formFileMock.SetupGet(r => r.FileName).Returns("image2.gif");
            var fileRepository = new FileRepository(configurationMock.Object, fileSystemMock);

            // Act
            var file = await fileRepository.StoreFileAsync(formFileMock.Object);

            // Assert
            Assert.That(file.IsSuccessful, Is.EqualTo(true));
            
        }

        [Test]
        public async Task WhenFileRequestedAsBase64String_ThenTheConvertedFileReturns()
        {
            // Arrange
            Mock<IFormFile> formFileMock = new Mock<IFormFile>();
            formFileMock.SetupGet(r => r.Length).Returns(5);
            formFileMock.SetupGet(r => r.FileName).Returns("image.gif");
            var fileRepository = new FileRepository(configurationMock.Object, fileSystemMock);

            // Act
            var file = await fileRepository.GetFileAsBase64Async(@"c:\testPath\image.gif");
            
            
            // Assert
            Assert.That(file.IsSuccessful, Is.EqualTo(true));
            Assert.That(file.Result.Filename, Is.EqualTo("image.gif"));
            Assert.That(file.Result.Content, Is.EqualTo("EjRW0g=="));
        }

        [Test]
        public async Task WhenStoreRequested_AndTheFileIsExsists_ThenFileIsNotStored()
        {
            // Arrange
            Mock<IFormFile> formFileMock = new Mock<IFormFile>();
            formFileMock.SetupGet(r => r.Length).Returns(5);
            formFileMock.SetupGet(r => r.FileName).Returns("image.gif");
            var fileRepository = new FileRepository(configurationMock.Object, fileSystemMock);

            // Act
            var file = await fileRepository.StoreFileAsync(formFileMock.Object);

            // Assert
            Assert.That(file.IsSuccessful, Is.EqualTo(false));
            Assert.That(file.ErrorMessage, Is.EqualTo("The file is already exists"));

        }

        [Test]
        public async Task WhenStoreRequested_AndTheFileIsTooBig_ThenFileIsNotStored()
        {
            // Arrange
            Mock<IFormFile> formFileMock = new Mock<IFormFile>();
            formFileMock.SetupGet(r => r.Length).Returns(15);
            formFileMock.SetupGet(r => r.FileName).Returns("image.gif");
            var fileRepository = new FileRepository(configurationMock.Object, fileSystemMock);

            // Act
            var file = await fileRepository.StoreFileAsync(formFileMock.Object);

            // Assert
            Assert.That(file.IsSuccessful, Is.EqualTo(false));
            Assert.That(file.ErrorMessage, Is.EqualTo("The file is too large"));

        }
    }
}
