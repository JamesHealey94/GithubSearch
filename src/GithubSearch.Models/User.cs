using System;
using System.Collections.Generic;

namespace GithubSearch.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public Uri AvatarUrl { get; set; }
        public IEnumerable<Repository> Repositories { get; set; }
        public Uri ProfileUrl { get; set; }
    }
}