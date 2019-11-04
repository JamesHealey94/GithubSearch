using GithubSearch.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubSearch
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient httpClient;

        public UserRepository(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://api.github.com/users/");
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", "jameshealey94"); // https://developer.github.com/v3/#user-agent-required
            this.httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json"); // https://developer.github.com/v3/media
        }

        public async Task<User> GetUser(string username)
        {
            var result = await httpClient.GetAsync(username);
            return result.IsSuccessStatusCode ? await result.Content.ReadAsAsync<User>() : null;
        }
    }
}
