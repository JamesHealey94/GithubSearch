using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GithubSearch.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly IUserService UserService = new UserService(new UserRepository(new MemoryCache("cache"), new HttpClient()));

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async new Task<ActionResult> User(string search, int take = 5)
        {
            if (search == null)
            {
                return RedirectToAction("Index");
            }
            
            Models.User user = await UserService.GetUser(search);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.Repositories = user.Repositories.OrderByDescending(r => r.Stargazers).Take(take);

            return View(user);
        }
    }
}