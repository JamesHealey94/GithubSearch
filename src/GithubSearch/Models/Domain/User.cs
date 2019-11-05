using System.Collections.Generic;

namespace GithubSearch.Models.Domain
{
    public class User
    {
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public IEnumerable<Repository> Repositories { get; set; }
        public string ProfileUrl { get; set; }
    }
}