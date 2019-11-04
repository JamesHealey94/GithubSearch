using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubSearch.Tests.Controllers
{
    [TestClass]
    public class UserServiceTest
    {
        readonly UserService UserService;

        public UserServiceTest()
        {
            UserService = new UserService(new UserRepository(new HttpClient()));
        }

        [DataTestMethod]
        [DataRow("JamesHealey94")]
        [DataRow("jameshealey94")]
        [DataRow("robconery")]
        public async Task Should_Return_Result_For_Valid_User(string username)
        {
            var result = await UserService.GetUser(username);

            Assert.IsNotNull(result);
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
    }
}
