using System.Collections.Generic;
using System.Net.Http.Headers;

namespace GithubSearch.Models.Response
{
    class RepositoryCacheValue
    {
        public EntityTagHeaderValue ETag { get; set; }
        public IEnumerable<Repository> Repositories { get; set; }
    }
}
