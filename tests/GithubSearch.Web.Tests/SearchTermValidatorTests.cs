using GithubSearch.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GithubSearch.Web.Tests
{
    [TestClass]
    public class SearchTermValidatorTests
    {
        [TestCategory("Validation")]
        [TestMethod]
        [DataRow("jameshealey94")]
        [DataRow("hello-world")]
        [DataRow("test-hello-world")]
        [DataRow("1234567")]
        [DataRow("james")]
        public void Should_Accept_Valid_Searches(string searchTerm)
        {
            var searchTermValidator = new GithubSearchTermValidator();

            var result = searchTermValidator.IsValid(searchTerm);

            Assert.IsTrue(result);
        }

        [TestCategory("Validation")]
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("1234567890123456789012345678901234567890")]
        [DataRow("test?")]
        [DataRow("test_")]
        public void Should_Not_Accept_Invalid_Searches(string searchTerm)
        {
            var searchTermValidator = new GithubSearchTermValidator();

            var result = searchTermValidator.IsValid(searchTerm);

            Assert.IsFalse(result);
        }
    }
}
