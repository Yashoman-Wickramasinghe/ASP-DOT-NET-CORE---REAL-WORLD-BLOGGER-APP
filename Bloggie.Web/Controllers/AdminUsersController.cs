using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class AdminUsersController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
