using Microsoft.AspNetCore.Mvc;

namespace web1.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return Content("TEACHER PORTAL - This is teacher link! Same Firebase DB - here you view attendance");
        }
    }
}