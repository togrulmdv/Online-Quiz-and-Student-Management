using Microsoft.AspNetCore.Mvc;

namespace BSUStudentManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
