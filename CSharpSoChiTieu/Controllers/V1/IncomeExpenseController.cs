using System.Net;
using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class IncomeExpenseController : Controller
    {
        private readonly IIncomeExpenseHandler _IncomeExpenseHandler;

        private const string IE_SESSION_KEY = "IncomeExpenseSession";

        public IncomeExpenseController(IIncomeExpenseHandler IncomeExpenseHandler)
        {
            _IncomeExpenseHandler = IncomeExpenseHandler;
        }

        public IActionResult Index()
        {
            var sessionModel = HttpContext.Session.GetObjectFromJson<IncomeExpenseSessionModel>(IE_SESSION_KEY)
                        ?? new IncomeExpenseSessionModel(); // Nếu chưa có thì dùng mặc định

            ViewBag.FormType = sessionModel.FormType;
            ViewBag.RangeType = sessionModel.RangeType;

            return View();
        }

        [HttpPost]
        public IActionResult LoadForm(string type)
        {
            var sessionModel = HttpContext.Session.GetObjectFromJson<IncomeExpenseSessionModel>(IE_SESSION_KEY)
                                 ?? new IncomeExpenseSessionModel();

            sessionModel.FormType = type;
            HttpContext.Session.SetObjectAsJson(IE_SESSION_KEY, sessionModel);

            if (type == "income")
            {
                ViewBag.Title = "Thêm khoản thu mới"; // sửa lại tên đúng nhé
                return PartialView("Income");
            }
            else if (type == "expense")
            {
                ViewBag.Title = "Thêm khoản chi mới"; // sửa lại tên đúng nhé
                return PartialView("Expenses");
            }
            else
            {
                return Content("Form không tồn tại!");
            }
        }




        public async Task<IActionResult> History(string? range = "month")
        {
            var sessionModel = HttpContext.Session.GetObjectFromJson<IncomeExpenseSessionModel>(IE_SESSION_KEY)
                          ?? new IncomeExpenseSessionModel();

            if (!string.IsNullOrEmpty(range)) // chỉ update nếu range có giá trị
            {
                sessionModel.RangeType = range;
                HttpContext.Session.SetObjectAsJson(IE_SESSION_KEY, sessionModel);
            }

            // Nếu không truyền range, thì lấy từ session
            var currentRange = sessionModel.RangeType ?? "month"; // phòng trường hợp null

            ViewBag.RangeType = currentRange;

            var result = await _IncomeExpenseHandler.Gets(0, "", currentRange);

            if (result.Status != HttpStatusCode.OK)
                return Content("Lỗi khi lấy dữ liệu: " + result.Message);

            var data = (result as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

            return PartialView("_History", data);
        }



        public async Task<IActionResult> Category(IncomeExpenseType type = 0)
        {

            var result = await _IncomeExpenseHandler.GetCategorys(type);

            if (result.Status != HttpStatusCode.OK)
                return Content("Lỗi khi lấy dữ liệu: " + result.Message);

            var data = (result as OperationResultList<CategoryViewModel>)?.Data ?? new List<CategoryViewModel>();

            return PartialView("_Category", data);
        }


        

        [HttpGet]
        public async Task<IActionResult> GetSummary(string? range = "month")
        {
            var result = await _IncomeExpenseHandler.GetSummary(range);

            if (result is OperationResult<IncomeExpenseummaryViewModel> summaryResult)
            {
                var model = summaryResult.Data;
                return Json(new
                {
                    totalIncomeFormatted = $"{model.TotalIncome:N0} đ",
                    totalExpenseFormatted = $"{model.TotalExpense:N0} đ",
                    remainingBalanceFormatted = $"{model.Balance:N0} đ"
                });
            }

            return Json(new
            {
                totalIncomeFormatted = "0 đ",
                totalExpenseFormatted = "0 đ",
                remainingBalanceFormatted = "0 đ"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(IncomeExpenseCreateUpdateModel model)
        {
            // Kiểm tra lỗi dữ liệu
            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError(nameof(model.Description), "Vui lòng nhập mô tả");

            if (model.Amount <= 0)
                ModelState.AddModelError(nameof(model.Amount), "Số tiền phải lớn hơn 0");

            if (model.Date == default)
                ModelState.AddModelError(nameof(model.Date), "Ngày tháng không hợp lệ");

            if (model.CategoryId == Guid.Empty)
                ModelState.AddModelError(nameof(model.CategoryId), "Vui lòng chọn loại");

            if (!ModelState.IsValid)
            {
                // Gửi lỗi về dạng JSON cho client
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.First().ErrorMessage
                    );

                return BadRequest(new { errors });
            }

            if (model.Id == Guid.Empty)
            {
                var result = await _IncomeExpenseHandler.Create(model);
                if (result.Status != HttpStatusCode.OK)
                    return Json(new { success = false, message = "Không thể tạo mới khoản chi tiêu." });
            }
            else
            {
                var result = await _IncomeExpenseHandler.Update(model.Id, model);
                if (result.Status != HttpStatusCode.OK)
                    return Json(new { success = false, message = "Không thể cập nhật khoản chi tiêu." });
            }

            return Json(new { success = true });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "ID không hợp lệ" });

            var result = await _IncomeExpenseHandler.Delete(id);

            if (result.Status != HttpStatusCode.OK)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true });
        }

    }
}