using GithubSearch.Models;
using System.Threading.Tasks;

namespace GithubSearch
{
    public interface IUserService
    {
        Task<User> GetUser(string username);
    }
}
