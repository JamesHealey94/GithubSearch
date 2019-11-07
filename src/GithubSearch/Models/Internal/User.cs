using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace GithubSearch.Models.Internal
{
    internal class User
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public Uri Avatar_Url { get; set; }
        public Uri Repos_Url { get; set; }
        public Uri Html_Url { get; set; }
        public EntityTagHeaderValue ETag { get; set; }


        internal static Models.User ToDomain(User user, IEnumerable<Repository> repos)
        {
            if (user == null)
            {
                return null;
            }
            else
            {
                return new Models.User
                {
                    Username = user.Login,
                    Name = user.Name,
                    AvatarUrl = user.Avatar_Url,
                    ProfileUrl = user.Html_Url,
                    Repositories = repos?.Select(r =>
                        new Models.Repository
                        {
                            Name = r.Name,
                            Description = r.Description,
                            Stargazers = r.Stargazers_Count
                        })
                };
            }
        }
    }
}