using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubSearch.Models.Domain
{
    public class User
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public Uri AvatarUrl { get; set; }
        public IEnumerable<Repository> Repositories { get; set; }
        public Uri ProfileUrl { get; set; }

        internal static User From(Response.User user, IEnumerable<Response.Repository> repos)
        {
            if (user == null)
            {
                return null;
            }
            else
            {
                return new User
                {
                    Username = user.Login,
                    Name = user.Name,
                    AvatarUrl = user.Avatar_Url,
                    ProfileUrl = user.Html_Url,
                    Repositories = repos?.Select(r =>
                        new Repository
                        {
                            Name = r.Name,
                            Description = r.Description,
                            Stargazers = r.StargazersCount
                        })
                };
            }
        }
    }
}