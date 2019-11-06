using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace GithubSearch.Tests.Controllers
{
    [TestClass]
    public class UserServiceTest
    {
        UserService UserService;
        
        [TestInitialize]
        public void Setup()
        {
            UserService = new UserService(new UserRepository(new MemoryCache("cache"), new HttpClient()));
        }

        [DataTestMethod]
        [TestCategory("Basic")]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_For_Valid_User(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [TestCategory("Basic")]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_With_Same_Username(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.AreEqual(username, result.Username, true);
        }

        [DataTestMethod]
        [TestCategory("Basic")]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_With_Repositories(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.IsTrue(result.Repositories.Any());
        }

        [DataTestMethod]
        [TestCategory("Basic")]
        [DataRow("norepos")] // https://api.github.com/users/norepos/repos
        public async Task Should_Return_Result_Without_Repositories(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.IsFalse(result.Repositories.Any());
        }

        //According to the form validation messages on https://github.com/join
        //Username may only contain alphanumeric characters or hyphens.
        //Username cannot have multiple consecutive hyphens.
        //Username cannot begin or end with a hyphen.
        //Maximum of 39 characters.
        [DataTestMethod]
        [TestCategory("Validation")]
        [DataRow("")]
        [DataRow("a_b")]
        //[DataRow("a--b")] Looks like this may have once been valid: https://api.github.com/users/hello--world
        //[DataRow("a-b-")] Looks like this may have once been valid: https://api.github.com/users/a-b-
        [DataRow("-a-b")]
        [DataRow("thisaccountdoesnotexist")]
        public async Task Should_Return_Null_For_Invalid_User(string search)
        {
            var result = await UserService.GetUser(search);

            Assert.IsNull(result);
        }

        private static Mock<HttpMessageHandler> GetHandlerMock()
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
        public async Task Should_Return_Result_For_Cached_User(string username)
        {
            var result1 = await UserService.GetUser(username);
            var result2 = await UserService.GetUser(username);

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
        }
        
        [DataTestMethod]
        [TestCategory("Caching")]
        [DataRow("jameshealey94")]
        public async Task Should_Return_Result_For_Cached_Users_Repositories(string username)
        {
            var result1 = await UserService.GetUser(username);
            var result2 = await UserService.GetUser(username);

            Assert.IsNotNull(result1.Repositories);
            Assert.IsNotNull(result2.Repositories);
        }

        [DataTestMethod]
        [TestCategory("Caching")]
        [DataRow("jameshealey94")]
        public async Task Should_Return_Updated_Result_For_Modified_User(string username)
        {
            var handlerMock = GetHandlerMock();
            var httpClient = new HttpClient(handlerMock.Object);
            var cache = new MemoryCache("cache");
            var userService = new UserService(new UserRepository(cache, httpClient));

            var result1 = await userService.GetUser(username);
            var result2 = await userService.GetUser(username);

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
        }
    }
}
