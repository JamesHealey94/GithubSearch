using GithubSearch.Models.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace GithubSearch
{
    public class UserRepository : IUserRepository
    {
        private readonly ObjectCache cache;
        private readonly HttpClient httpClient;

        public UserRepository(ObjectCache cache, HttpClient httpClient)
        {
            this.cache = cache;
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://api.github.com/users/");
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", "jameshealey94"); // https://developer.github.com/v3/#user-agent-required
            this.httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json"); // https://developer.github.com/v3/media
        }

        public async Task<User> GetUser(string username)
        {
            var user = await GetUserInternal(username);
            var repos = await GetRepositories(user?.Repos_Url);

            return User.From(user, repos);
        }

        private async Task<Models.Response.User> GetUserInternal(string username)
        {
            if (String.IsNullOrWhiteSpace(username)) return null;

            var user = cache.Get(username) as Models.Response.User;
            if (user != null)
            {
                Console.WriteLine($"Found '{username}' in cache with ETag '{user.ETag?.Tag}'");
                httpClient.DefaultRequestHeaders.Add("If-None-Match", user.ETag?.Tag);
            }

            var response = await httpClient.GetAsync(username);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Console.WriteLine($"'{username}' modified, updating cache");
                    user = await response.Content.ReadAsAsync<Models.Response.User>();
                    user.ETag = response.Headers.ETag;
                    cache.Set(username, user, new CacheItemPolicy());
                    break;

                case HttpStatusCode.NotModified:
                    Console.WriteLine($"'{username}' not modified, using existing cache result");
                    break;

                default:
                    Console.WriteLine($"Unexpected response status code ({response.StatusCode}) for user '{username}'");
                    break;
            }

            return user;
        }

        private async Task<IEnumerable<Models.Response.Repository>> GetRepositories(Uri url)
        {
            if (url == null || String.IsNullOrWhiteSpace(url.AbsoluteUri)) return null;

            var repos = cache.Get(url.AbsoluteUri) as Models.Response.RepositoryCacheValue;
            if (repos != null)
            {
                Console.WriteLine($"Found repos for '{url.AbsoluteUri}' in cache with ETag '{repos.ETag?.Tag}'");
                httpClient.DefaultRequestHeaders.Add("If-None-Match", repos.ETag?.Tag);
            }
            else
            {
                repos = new Models.Response.RepositoryCacheValue();
            }

            var response = await httpClient.GetAsync(url);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Console.WriteLine($"Repos for '{url}' modified, updating cache");
                    repos.Repositories = await response.Content.ReadAsAsync<IEnumerable<Models.Response.Repository>>();
                    repos.ETag = response.Headers.ETag;
                    cache.Set(url.AbsoluteUri, repos, new CacheItemPolicy());
                    break;

                case HttpStatusCode.NotModified:
                    Console.WriteLine($"Repos for '{url}' not modified, using existing cache result");
                    break;

                default:
                    Console.WriteLine($"Unexpected response status code ({response.StatusCode}) for '{url}'");
                    break;
            }

            return repos.Repositories;
        }
    }
}
