using GithubSearch.Web.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GithubSearch.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly ISearchTermValidator searchTermValidator;

        public HomeController(IUserService userService, ISearchTermValidator searchTermValidator)
        {
            this.userService = userService;
            this.searchTermValidator = searchTermValidator;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async new Task<ActionResult> User(string search, int take = 5)
        {
            if (!searchTermValidator.IsValid(search))
            {
                return RedirectToAction("Invalid");
            }

            try
            {
                Models.User user = await userService.GetUser(search, take);

                if (user == null)
                {
                    return RedirectToAction("NotFound");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public ActionResult Invalid()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NotFound()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }
    }
}