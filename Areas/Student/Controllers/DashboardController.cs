using Microsoft.AspNetCore.Mvc;

namespace web1.Areas.Student.Controllers
{
    [Area("Student")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return Content("STUDENT PORTAL - This is student link! Same Firebase DB - here QR will be generated");
        }
    }
}