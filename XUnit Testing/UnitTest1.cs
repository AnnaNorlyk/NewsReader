using Xunit;
using NewsReader;
using System;

namespace XUnit_Testing
{
    public class NntpClientTests
    {
        [Fact]
        public void Connect_InvalidServerDetails_ShouldThrowException()
        {
            // Arrange
            var client = new NntpClient();

            // Act & Assert
            Assert.Throws<Exception>(() => client.Connect("", -1));
        }

        [Fact]
        public void Connect_ValidServerDetails_ShouldNotThrowException()
        {
            // Arrange
            var client = new NntpClient();

            // Act
            client.Connect("valid-host", 119); // Replace with mock or test server

            // Assert - No exception expected
        }

        [Fact]
        public void Authenticate_InvalidCredentials_ShouldThrowException()
        {
            // Arrange
            var client = new NntpClient();
            client.Connect("valid-host", 119);

            // Act & Assert
            Assert.Throws<Exception>(() => client.Authenticate("invalid-username", "invalid-password"));
        }

        [Fact]
        public void GetNewsgroups_WhenConnected_ShouldReturnListOfNewsgroups()
        {
            // Arrange
            var client = new NntpClient();
            client.Connect("valid-host", 119);
            client.Authenticate("valid-username", "valid-password");

            // Act
            var newsgroups = client.GetNewsgroups();

            // Assert
            Assert.NotEmpty(newsgroups);
            Assert.Contains("comp.lang.csharp", newsgroups); // Adjust as needed
        }

        [Fact]
        public void GetArticleHeaders_InvalidNewsgroup_ShouldThrowException()
        {
            // Arrange
            var client = new NntpClient();
            client.Connect("valid-host", 119);
            client.Authenticate("valid-username", "valid-password");

            // Act & Assert
            Assert.Throws<Exception>(() => client.GetArticleHeaders("nonexistent.group"));
        }

        [Fact]
        public void GetArticleHeaders_ValidNewsgroup_ShouldReturnHeaders()
        {
            // Arrange
            var client = new NntpClient();
            client.Connect("valid-host", 119);
            client.Authenticate("valid-username", "valid-password");

            // Act
            var headers = client.GetArticleHeaders("comp.lang.csharp"); // Replace with actual newsgroup name

            // Assert
            Assert.NotEmpty(headers);
            Assert.All(headers, header => Assert.False(string.IsNullOrWhiteSpace(header.Subject)));
        }
    }
}
