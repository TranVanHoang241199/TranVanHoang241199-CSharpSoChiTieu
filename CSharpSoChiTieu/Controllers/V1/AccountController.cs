using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using CSharpSoChiTieu.Data.Data.Entitys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSharpSoChiTieu.Controllers
{
    public class AccountController : Controller
    {
        private readonly CTDbContext _context;
        private readonly IAccountHandler _accountHandler;
        private readonly ICategoryHandler _categoryHandler;

        public AccountController(CTDbContext context, IAccountHandler accountHandler, ICategoryHandler categoryHandler)
        {
            _context = context;
            _accountHandler = accountHandler;
            _categoryHandler = categoryHandler;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var result = _accountHandler.Authenticate(model);

                if (result.Success)
                {
                    var claimsIdentity = new ClaimsIdentity(result.Claims, "MyCookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    return result.User.Role == "admin"
                        ? RedirectToAction("Dashboard", "Admin")
                        : RedirectToAction("Index", "IncomeExpense");
                }

                ModelState.AddModelError("", result.ErrorMessage);
            }

            //Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("", "Hình như bạn chưa nhập đầy đủ thông tin");

            return View(model);
        }


        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountHandler.Register(model);

                if (result.Success)
                {
                    // Tự động đăng nhập sau khi đăng ký
                    var claimsIdentity = new ClaimsIdentity(result.Claims, "MyCookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    // Thêm loại mặt định
                    await _categoryHandler.Adddefault(result.Id);

                    // Chuyển hướng dựa vào role
                    return result.User.Role == "admin"
                        ? RedirectToAction("Dashboard", "Admin")
                        : RedirectToAction("Index", "IncomeExpense");
                }

                ModelState.AddModelError("", result.ErrorMessage);
            }

            //Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("", "Hình như bạn chưa nhập đầy đủ thông tin");

            return View(model);
        }

        // View Quên mật khẩu
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.ct_Users.FirstOrDefault(u => u.Email == model.Email && u.IsDeleted == false);
                if (user != null)
                {
                    // Tạo token reset
                    user.PasswordResetToken = Guid.NewGuid().ToString();
                    user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

                    _context.SaveChanges();

                    // Tạo link reset
                    var resetLink = Url.Action("ResetPassword", "Account", new { token = user.PasswordResetToken }, Request.Scheme);
                    SendResetPasswordEmail(user.Email, resetLink);

                    return View("ForgotPasswordConfirmation");
                }

                ModelState.AddModelError("", "Không tìm thấy tài khoản với email này.");
            }

            return View(model);
        }

        // Hỗ trợ gửi email reset mật khẩu
        private void SendResetPasswordEmail(string email, string resetLink)
        {
            var service = GmailServiceHelper.GetGmailService(); // Lấy dịch vụ Gmail

            // Tạo nội dung email
            string subject = "Reset Password";
            string body = $"Click vào link sau để đặt lại mật khẩu: <a href='{resetLink}'>Reset Password</a>";

            // Gửi email qua Gmail API
            GmailServiceHelper.SendEmail(service, email, subject, body);
        }


        // View Đổi mật khẩu
        public IActionResult ResetPassword(string token) => View(new ResetPasswordViewModel { Token = token });

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.ct_Users.FirstOrDefault(u =>
                            u.PasswordResetToken == model.Token
                            && u.PasswordResetTokenExpiry > DateTime.UtcNow);

                if (user != null)
                {
                    user.Password = PasswordHasher.Hash(model.NewPassword);
                    user.PasswordResetToken = null;
                    user.PasswordResetTokenExpiry = null;

                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Token không hợp lệ hoặc đã hết hạn.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToAction("Login");
        }


        // Action yêu cầu quyền admin
        [Authorize(Roles = "admin")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return RedirectToAction("Login");
            }

            var profile = _accountHandler.GetProfile(userId);

            if (profile == null)
            {
                return RedirectToAction("Login");
            }

            return View(profile);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model, IFormFile Image)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var user = _context.ct_Users.FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Update user information
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Phone = model.Phone;

                // Handle image upload if provided
                if (Image != null && Image.Length > 0)
                {
                    // Generate unique filename
                    var fileName = $"{userId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(Image.FileName)}";
                    var filePath = Path.Combine("wwwroot", "uploads", "profiles", fileName);

                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    // Update user's image path
                    user.Image = $"/uploads/profiles/{fileName}";
                }

                _context.SaveChanges();

                // Update claims if name changed
                if (user.FullName != User.Identity.Name)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("UserId", user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                        "MyCookieAuth",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile image)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var user = _context.ct_Users.FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                if (image != null && image.Length > 0)
                {
                    // Generate unique filename
                    var fileName = $"{userId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine("wwwroot", "uploads", "profiles", fileName);

                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Update user's image path
                    user.Image = $"/uploads/profiles/{fileName}";
                    _context.SaveChanges();

                    // Update claims if needed
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("UserId", user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                        "MyCookieAuth",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return Json(new { success = true, imageUrl = user.Image });
                }

                return Json(new { success = false, message = "Không có file ảnh được gửi lên" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }

}