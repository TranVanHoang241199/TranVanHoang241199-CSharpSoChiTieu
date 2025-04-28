using System.Security.Claims;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    public class AccountController : Controller
    {
        private readonly CTDbContext _context;
        private readonly IAccountHandler _accountHandler;

        public AccountController(CTDbContext context, IAccountHandler accountHandler)
        {
            _context = context;
            _accountHandler = accountHandler;
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

                    // Chuyển hướng dựa vào role
                    return result.User.Role == "admin"
                        ? RedirectToAction("Dashboard", "Admin")
                        : RedirectToAction("Index", "IncomeExpense");
                }

                ModelState.AddModelError("", result.ErrorMessage);
            }

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
                    user.PasswordResetTokenExpiry = DateTime.Now.AddHours(1);

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
                            && u.PasswordResetTokenExpiry > DateTime.Now);

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

        
    }

}