using GithubSearch.Models.Domain;
using System.Threading.Tasks;

namespace GithubSearch
{
    public class UserService
    {
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<User> GetUser(string username)
        {
            return await repository.GetUser(username);
        }
    }
}
