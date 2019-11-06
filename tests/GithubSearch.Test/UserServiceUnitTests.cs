using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace GithubSearch.Tests.Controllers
{
    [TestClass]
    public class UserServiceUnitTests
    {
        static Mock<HttpMessageHandler> GetHandlerMock()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(
                       @"{
                            ""repos_url"": ""https://api.github.com/users/test/repos""
                        }",
                       System.Text.Encoding.UTF8,
                       "application/json"),
               });
            return handlerMock;
        }

        [TestCategory("Caching")]
        [TestMethod]
        public async Task Should_Go_To_Api_If_Not_Cached()
        {
            var handlerMock = GetHandlerMock();
            var httpClient = new HttpClient(handlerMock.Object);
            var userService = new UserService(new UserRepository(new MemoryCache("cache"), httpClient));


            var result = await userService.GetUser("test");


            var expectedUri1 = new Uri("https://api.github.com/users/test");
            var expectedUri2 = new Uri("https://api.github.com/users/test/repos");
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(2),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && (req.RequestUri == expectedUri1 || req.RequestUri == expectedUri2)
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [TestCategory("Caching")]
        [TestMethod]
        public async Task Should_Add_User_To_Cache()
        {
            var handlerMock = GetHandlerMock();
            var httpClient = new HttpClient(handlerMock.Object);
            var cache = new MemoryCache("cache");
            var userService = new UserService(new UserRepository(cache, httpClient));

            await userService.GetUser("test");

            Assert.IsTrue(cache.Contains("test"));
        }

        [DataTestMethod]
        [TestCategory("Caching")]
        [DataRow("jameshealey94")]
        public async Task Should_Return_Cached_Result_For_Unmodified_User(string username)
        {
            var httpClient = new MockHttpClient();
            var userService = new UserService(new UserRepository(new MemoryCache("cache"), httpClient));

            var result1 = await userService.GetUser(username);

            httpClient.Response = new HttpResponseMessage(HttpStatusCode.NotModified);

            var result2 = await userService.GetUser(username);

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsTrue(result1.Name.Equals(result2.Name));
        }

        [DataTestMethod]
        [TestCategory("Caching")]
        [DataRow("jameshealey94")]
        public async Task Should_Return_Updated_Result_For_Modified_User(string username)
        {
            var httpClient = new MockHttpClient();
            var userService = new UserService(new UserRepository(new MemoryCache("cache"), httpClient));

            var result1 = await userService.GetUser(username);

            httpClient.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content =
                    new StringContent(
                        @"{
                            ""repos_url"": ""https://api.github.com/users/test/repos"",
                            ""name"": ""Modified Name""
                         }",
                        System.Text.Encoding.UTF8,
                        "application/json")
            };

            var result2 = await userService.GetUser(username);

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsFalse(result1.Name.Equals(result2.Name));
        }

        [DataTestMethod]
        [TestCategory("Rate Limiting")]
        [DataRow("jameshealey94")]
        public async Task Should_Return_Cached_Result_When_Rate_Limited(string username)
        {
            var httpClient = new MockHttpClient();
            var userService = new UserService(new UserRepository(new MemoryCache("cache"), httpClient));

            var result1 = await userService.GetUser(username);

            httpClient.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);

            var result2 = await userService.GetUser(username);

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsTrue(result1.Name.Equals(result2.Name));
        }
    }

    public class MockHttpClient : HttpClient
    {
        public HttpResponseMessage Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content =
                    new StringContent(
                        @"{
                            ""repos_url"": ""https://api.github.com/users/test/repos"",
                            ""name"": ""Original Name""
                         }",
                        System.Text.Encoding.UTF8,
                        "application/json")
            };

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Mock SendAsync");
            return Task.FromResult(Response);
        }
    }
}
