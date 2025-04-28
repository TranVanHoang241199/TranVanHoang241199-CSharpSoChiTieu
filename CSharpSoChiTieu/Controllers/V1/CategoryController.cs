using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
