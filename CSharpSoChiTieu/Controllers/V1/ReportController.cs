using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
