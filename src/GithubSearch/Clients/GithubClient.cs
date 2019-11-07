using System;
using System.Net.Http;

namespace GithubSearch
{
    public class GithubClient : HttpClient
    {
        public GithubClient()
        {
            Setup();
        }
        
        public GithubClient(HttpMessageHandler httpMessageHandler) : base(httpMessageHandler)
        {
            Setup();
        }
        
        private void Setup()
        {
            BaseAddress = new Uri("https://api.github.com/users/");
            DefaultRequestHeaders.Add("User-Agent", "jameshealey94"); // https://developer.github.com/v3/#user-agent-required
            DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json"); // https://developer.github.com/v3/media
        }
    }
}
