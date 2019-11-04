using GithubSearch.Models;
using System.Threading.Tasks;

namespace GithubSearch
{
    public class UserService
    {
        IUserRepository Repository { get; }

        public UserService(IUserRepository repository)
        {
            Repository = repository;
        }

        public async Task<User> GetUser(string username)
        {
            return await Repository.GetUser(username);
        }
    }
}
