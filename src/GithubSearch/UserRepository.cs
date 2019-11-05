using GithubSearch.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var userResult = await httpClient.GetAsync(username);
            if (!userResult.IsSuccessStatusCode)
            {
                return null;
            }
            
            var userResponse = await userResult.Content.ReadAsAsync<Models.Response.User>();
            var user = new User
            {
                Username = userResponse.Login,
                AvatarUrl = userResponse.Avatar_Url,
                ProfileUrl = userResponse.Html_Url,
            };

            var repoResult = await httpClient.GetAsync(userResponse.Repos_Url);
            if (!repoResult.IsSuccessStatusCode)
            {
                return null;
            }

            var repos = await repoResult.Content.ReadAsAsync<IEnumerable<Models.Response.Repository>>();
            user.Repositories = repos?.Select(r => 
                new Repository
                {
                    Name = r.Name,
                    Description = r.Description,
                    Stargazers = r.StargazersCount
                }).ToList();
            return user;
        }
    }
}
