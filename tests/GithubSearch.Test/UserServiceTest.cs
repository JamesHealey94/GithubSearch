using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GithubSearch.Tests.Controllers
{
    [TestClass]
    public class UserServiceTest
    {
        [DataTestMethod]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public void Should_Return_Result_For_Valid_User(string username)
        {
            // Arrange
            var service = new UserService();

            // Act
            var result = service.GetUser(username);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
