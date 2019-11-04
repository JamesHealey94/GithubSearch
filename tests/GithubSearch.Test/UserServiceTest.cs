using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubSearch.Tests.Controllers
{
    [TestClass]
    public class UserServiceTest
    {
        [DataTestMethod]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_For_Valid_User(string username)
        {
            // Arrange
            var service = new UserService(new UserRepository(new HttpClient()));

            // Act
            var result = await service.GetUser(username);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
