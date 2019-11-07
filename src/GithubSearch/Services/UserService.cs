using GithubSearch.Models;
using System.Linq;
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

        public async Task<User> GetUser(string username, int limitTopRepos = 5)
        {
            if (string.IsNullOrWhiteSpace(username)) { return null; }

            var user = await repository.GetUser(username.Trim());
            if (user != null && user.Repositories != null)
            {
                user.Repositories = user.Repositories.OrderByDescending(r => r.Stargazers).Take(limitTopRepos);
            }

            return user;
        }
    }
}
