using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly ISettingHandler _settingHandler;
        private readonly ILogger<SettingController> _logger;

        public SettingController(ISettingHandler settingHandler, ILogger<SettingController> logger)
        {
            _settingHandler = settingHandler;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogWarning("Không tìm thấy UserId trong claims");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var settings = _settingHandler.GetUserSettings();
                return View(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy cài đặt cho user {userId}");
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult UpdateSettings(SettingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.First().ErrorMessage
                    );
                return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            try
            {
                var result = _settingHandler.UpdateSettings(model);
                if (result)
                {
                    return Json(new { success = true, message = "Cập nhật cài đặt thành công" });
                }
                return Json(new { success = false, message = "Cập nhật cài đặt thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cài đặt");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật cài đặt" });
            }
        }

        [HttpGet]
        public IActionResult GetUserSettings()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
            }

            try
            {
                var settings = _settingHandler.GetUserSettings();
                return Json(new { success = true, data = settings });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy cài đặt cho user {userId}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy cài đặt" });
            }
        }

        [HttpPost]
        public IActionResult UpdateCurrency(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                return Json(new { success = false, message = "Mã tiền tệ không hợp lệ" });
            }

            try
            {
                var result = _settingHandler.UpdateCurrency(currencyCode);
                if (result)
                {
                    return Json(new { success = true, message = "Cập nhật tiền tệ thành công" });
                }
                return Json(new { success = false, message = "Cập nhật tiền tệ thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật tiền tệ");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật tiền tệ" });
            }
        }
    }
}
