using System;
using System.Net.Http.Headers;

namespace GithubSearch.Models.Response
{
    class User
    {
        public string Login { get; set; }
        public Uri Avatar_Url { get; set; }
        public Uri Repos_Url { get; set; }
        public Uri Html_Url { get; set; }
        public EntityTagHeaderValue ETag { get; set; }
    }
}