using System.Linq;

namespace GithubSearch.Web.Services
{
    public class GithubSearchTermValidator : ISearchTermValidator
    {
        public bool IsValid(string search)
        {
            return search != null
                && search != string.Empty
                && search.Length < 40
                && search.All(ch => char.IsLetterOrDigit(ch) || ch == '-');
        }
    }
}