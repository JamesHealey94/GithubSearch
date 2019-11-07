using GithubSearch.Models;
using System.Threading.Tasks;

namespace GithubSearch
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<User> GetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) { return null; }

            return await repository.GetUser(username.Trim());
        }
    }
}
