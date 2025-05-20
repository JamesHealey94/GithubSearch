using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GithubSearch.Web.Services;
using GithubSearch.Web.Controllers;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace GithubSearch.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private static HomeController GetController()
        {
            return new HomeController(
                new UserService(
                    new UserRepository(
                        new MemoryCache("cache"),
                        new GithubClient())),
                new GithubSearchTermValidator());
        }

        [TestMethod]
        [TestCategory("Controller")]
        [TestCategory("Integration")]
        public void Index()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [TestCategory("Controller")]
        [TestCategory("Integration")]
        [DataRow("jameshealey94")]
        public async Task User(string search)
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.User(search) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("Controller")]
        [TestCategory("Integration")]
        [DataRow("test?:test")] // Invalid character
        [DataRow("thisstringiswaytoolongandshouldfailthevalidationforthesearchterm")] // Too long
        [DataRow("")] // Empty string
        [DataRow(null)] // Null string
        public async Task User_InvalidSearchTerm_ShouldRedirectToInvalidAction(string search)
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.User(search) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Result should be a RedirectToRouteResult.");
            Assert.AreEqual("Invalid", result.RouteValues["action"]);
        }

        [TestMethod]
        [TestCategory("Controller")]
        [TestCategory("Integration")]
        // This username is highly unlikely to exist and should trigger the NotFound path
        // Note: This is an integration test and relies on the actual GitHub API response.
        // For a unit test, IUserService would be mocked.
        [DataRow("nonexistentuser1234567890zxcvbnm")]
        public async Task User_UserNotFound_ShouldRedirectToNotFoundAction(string search)
        {
            // Arrange
            var controller = GetController();

            // Act
            // For this test to reliably check the "NotFound" redirect, we assume the GetUser 
            // service will return null for a truly non-existent user.
            var result = await controller.User(search) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Result should be a RedirectToRouteResult for a non-existent user.");
            Assert.AreEqual("NotFound", result.RouteValues["action"]);
        }
    }
}
