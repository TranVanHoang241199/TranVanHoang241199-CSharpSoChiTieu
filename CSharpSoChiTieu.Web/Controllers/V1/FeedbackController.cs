using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CSharpSoChiTieu.Web.Controllers
{
    /// <summary>
    /// Truy cập điều khiển xử lý danh mục 
    /// </summary>
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackHandler _feedbackHandler;

        public FeedbackController(IFeedbackHandler feedbackHandler)
        {
            _feedbackHandler = feedbackHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _feedbackHandler.GetUserFeedbacksAsync();

            // Ép kiểu về OperationResultList để lấy thuộc tính Data
            var listResult = result as OperationResultList<FeedbackViewModel>;
            var model = listResult?.Data ?? new List<FeedbackViewModel>();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Send(FeedbackCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu nhập vào không hợp lệ!" });
            }

            var result = await _feedbackHandler.CreateFeedbackAsync(model);

            // Kiểm tra Status == HttpStatusCode.OK thay vì result.Success
            bool isSuccess = result.Status == HttpStatusCode.OK;

            return Json(new { success = isSuccess, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _feedbackHandler.DeleteFeedbackAsync(id);
            bool isSuccess = result.Status == HttpStatusCode.OK;
            return Json(new { success = isSuccess, message = result.Message });
        }
    }
}