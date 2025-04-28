using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
