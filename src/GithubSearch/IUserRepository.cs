using GithubSearch.Models;
using System.Threading.Tasks;

namespace GithubSearch
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
    }
}
