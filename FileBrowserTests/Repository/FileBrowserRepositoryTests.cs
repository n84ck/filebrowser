using FileBrowser.Repository;
using FileBrowserTests.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowserTests.Repository
{
    [TestFixture]
    internal class FileBrowserRepositoryTests
    {
        private Mock<FakeHttpMessageHandler> httpMessageHandlerMock;
        private HttpClient httpClient;

        public FileBrowserRepositoryTests()
        {
            httpMessageHandlerMock = new Mock<FakeHttpMessageHandler> { CallBase = true };
            httpClient = new HttpClient(httpMessageHandlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost/api/");
        }

        [Test]
        public async Task WhenListFilesAsync_ThenStringArrayReturns()
        {
            // Arrange
            httpMessageHandlerMock.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[\"test.jpg\",\"test2.jpg\"]")
            });

            var repository = new FileBrowserRepository(httpClient);

            // Act
            var res = await repository.ListFilesAsync();

            // Assert
            Assert.That(res.IsSuccessful, Is.EqualTo(true));
            Assert.That(res.Result, Is.EqualTo(new string[] { "test.jpg", "test2.jpg" }));
        }

        [Test]
        public async Task WhenFileAsync_ThenFileReturns()
        {
            // Arrange
            httpMessageHandlerMock.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"content\":\"dGVzdA == \",\"filename\":\"test.txt\"}")
            });

            var repository = new FileBrowserRepository(httpClient);

            // Act
            var res = await repository.GetFileAsync("test.txt");

            // Assert
            Assert.That(res.IsSuccessful, Is.EqualTo(true));
            Assert.That(res.Result.Content, Is.EqualTo("test"));
            Assert.That(res.Result.Filename, Is.EqualTo("test.txt"));
        }

        [Test]
        public async Task WhenFileAsync_AndError_ThenErrorResultReturns()
        {
            // Arrange
            httpMessageHandlerMock.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Test message")
            });

            var repository = new FileBrowserRepository(httpClient);

            // Act
            var res = await repository.GetFileAsync("test.txt");

            // Assert
            Assert.That(res.IsSuccessful, Is.EqualTo(false));
            Assert.That(res.ErrorMessage, Is.EqualTo("Test message"));
        }
    }
}
