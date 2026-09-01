using Microsoft.AspNetCore.Mvc;

namespace web1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return Content("ADMIN PORTAL - This is admin link! Same Firebase DB - here you manage students");
        }
    }
}