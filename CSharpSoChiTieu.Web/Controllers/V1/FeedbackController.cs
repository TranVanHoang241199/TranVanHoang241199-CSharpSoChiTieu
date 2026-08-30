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

        [HttpPost]
        public async Task<IActionResult> Send(FeedbackInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Description))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ tiêu đề và nội dung." });
            }

            // TODO: Lưu thông tin góp ý vào CSDL hoặc gửi email trực tiếp cho Admin
            /* 
               Ví dụ:
               - Lưu model vào db bảng Feedbacks
               - Nếu model.Attachment != null -> Lưu file ảnh vào thư mục wwwroot/uploads/feedbacks
            */

            return Json(new { success = true });
        }
    }

    public class FeedbackInputModel
    {
        public string Type { get; set; } // Feedback hoặc Bug
        public string Priority { get; set; } // Low, Medium, High
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? Attachment { get; set; }
    }

}
