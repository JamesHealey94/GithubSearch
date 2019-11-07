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
        [DataRow("test?:test")]
        public async Task Invalid(string search)
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.User(search) as ViewResult;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Controller")]
        [TestCategory("Integration")]
        [DataRow("thisaccountdoesnotexist")]
        public async Task NotFound(string search)
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.User(search) as ViewResult;

            // Assert
            Assert.IsNull(result);
        }
    }
}
