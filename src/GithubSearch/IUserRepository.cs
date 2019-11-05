using GithubSearch.Models.Domain;
using System.Threading.Tasks;

namespace GithubSearch
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
    }
}
