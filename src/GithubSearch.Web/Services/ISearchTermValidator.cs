namespace GithubSearch.Web.Services
{
    public interface ISearchTermValidator
    {
        bool IsValid(string searchTerm);
    }
}