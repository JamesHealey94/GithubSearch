using GithubSearch.Web.Services;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace GithubSearch.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterInstance<ObjectCache>(new MemoryCache("cache"));
            container.RegisterSingleton<GithubClient>();
            container.RegisterType<HttpClient, GithubClient>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<ISearchTermValidator, GithubSearchTermValidator>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}