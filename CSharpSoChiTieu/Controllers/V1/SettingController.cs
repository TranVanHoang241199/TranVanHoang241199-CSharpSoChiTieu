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
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Không tìm thấy UserId trong claims");
                return RedirectToAction("Login", "Account");
            }

            var userProfile = _settingHandler.GetUserProfile(userId);
            if (userProfile == null)
            {
                _logger.LogWarning($"Không tìm thấy profile cho user {userId}");
                return RedirectToAction("Login", "Account");
            }

            var settings = new SettingViewModel
            {
                UserId = userProfile.Id,
                DarkMode = userProfile.DarkMode,
                Notifications = userProfile.Notifications,
                Language = userProfile.Language ?? "vi"
            };

            return View(settings);
        }

        [HttpPost]
        public IActionResult UpdateSettings(SettingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
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
    }
}
