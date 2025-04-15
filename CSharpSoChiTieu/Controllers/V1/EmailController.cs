using CSharpSoChiTieu.common;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers.V1
{
    public class EmailController : Controller
    {
        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(string email, string subject, string body)
        {
            try
            {
                var gmailService = GmailServiceHelper.GetGmailService();
                GmailServiceHelper.SendEmail(gmailService, email, subject, body);
                return RedirectToAction("EmailSent");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể gửi email: " + ex.Message);
            }

            return View();
        }

        public IActionResult EmailSent()
        {
            return View();
        }
    }
}
