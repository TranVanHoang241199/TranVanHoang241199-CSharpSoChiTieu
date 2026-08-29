using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Web.Controllers
{
    /// <summary>
    /// Truy cập điều khiển xử lý danh mục 
    /// </summary>
    [Authorize]
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
