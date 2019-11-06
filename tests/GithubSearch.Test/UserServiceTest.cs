using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GithubSearch.Tests.Controllers
{
    [TestClass]
    public class UserServiceTest
    {
        readonly UserService UserService = new UserService(new UserRepository(new HttpClient()));
        
        [DataTestMethod]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_For_Valid_User(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_With_Same_Username(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.AreEqual(username, result.Username, true);
        }

        [DataTestMethod]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_With_Repositories(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.IsTrue(result.Repositories.Any());
        }

        [DataTestMethod]
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

        [DataTestMethod]
        public async Task Should_Go_To_Api_If_Not_Cached()
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
            var httpClient = new HttpClient(handlerMock.Object);
            var userService = new UserService(new UserRepository(httpClient));


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
    }
}
