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
        [DataRow("a1-b2-c3-d4-e5-f6-g7-h8-i9-j0-k1-l2-m3")] // 39 chars
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
        [DataRow("   ")] // String with only spaces
        [DataRow("1234567890123456789012345678901234567890")] // 40 chars
        [DataRow("test?")]
        [DataRow("test_")]
        [DataRow("invalid space")]
        public void Should_Not_Accept_Invalid_Searches(string searchTerm)
        {
            var searchTermValidator = new GithubSearchTermValidator();

            var result = searchTermValidator.IsValid(searchTerm);

            Assert.IsFalse(result);
        }
    }
}
