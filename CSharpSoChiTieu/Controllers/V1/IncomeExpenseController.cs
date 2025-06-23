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
        private readonly ICurrencyHandler _currencyHandler;
        private readonly ISettingHandler _settingHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string IE_SESSION_KEY = "IncomeExpenseSession";

        public IncomeExpenseController(IIncomeExpenseHandler IncomeExpenseHandler, ICurrencyHandler currencyHandler, ISettingHandler settingHandler, IHttpContextAccessor httpContextAccessor)
        {
            _IncomeExpenseHandler = IncomeExpenseHandler;
            _currencyHandler = currencyHandler;
            _settingHandler = settingHandler;
            _httpContextAccessor = httpContextAccessor;
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
        public async Task<IActionResult> Create(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = "expense"; // Default to expense if type is not provided
            }

            var sessionModel = HttpContext.Session.GetObjectFromJson<IncomeExpenseSessionModel>(IE_SESSION_KEY)
                     ?? new IncomeExpenseSessionModel();

            sessionModel.FormType = type;
            HttpContext.Session.SetObjectAsJson(IE_SESSION_KEY, sessionModel);

            // Lấy danh sách tiền tệ
            var currencyResult = await _currencyHandler.GetAll();
            var currencies = (currencyResult as OperationResultList<CurrencyViewModel>)?.Data ?? new List<CurrencyViewModel>();
            ViewBag.Currencies = currencies;

            // Lấy đơn vị tiền tệ đang lưu trong user setting
            var userSetting = _settingHandler.GetUserSettings();
            ViewBag.SelectedCurrency = userSetting?.Currency ?? "VND";

            // Initialize model with default values
            var model = new IncomeExpenseCreateUpdateModel
            {
                Id = Guid.Empty,
                Date = DateTime.UtcNow,
                Amount = 0,
                Type = type == "income" ? IncomeExpenseType.Income : IncomeExpenseType.Expense
            };

            // Set view data for both types
            ViewBag.Type = type;
            ViewBag.Title = type == "income" ? "Tạo khoản thu" : "Tạo khoản chi";
            ViewBag.CategoryType = type == "income" ? IncomeExpenseType.Income : IncomeExpenseType.Expense;

            return View(model);
        }

        public async Task<IActionResult> Category(IncomeExpenseType type = 0)
        {

            var result = await _IncomeExpenseHandler.GetCategories(type);

            if (result.Status != HttpStatusCode.OK)
                return Content("Lỗi khi lấy dữ liệu: " + result.Message);

            var data = (result as OperationResultList<CategoryViewModel>)?.Data ?? new List<CategoryViewModel>();

            return PartialView("_Category", data);
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

            // Lấy toàn bộ symbol cho các loại currency
            var allCurrencies = await _currencyHandler.GetAll() as OperationResultList<CurrencyViewModel>;
            var symbolDict = allCurrencies?.Data?.ToDictionary(c => c.Code, c => c.Symbol) ?? new Dictionary<string, string>();
            ViewBag.Symbols = symbolDict;

            var userSetting = _settingHandler.GetUserSettings();
            var result = await _IncomeExpenseHandler.Gets(0, "", currentRange, userSetting.Currency);

            if (result.Status != HttpStatusCode.OK)
                return Content("Lỗi khi lấy dữ liệu: " + result.Message);

            var data = (result as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

            return PartialView("_History", data);
        }



        [HttpGet]
        public async Task<IActionResult> GetSummary(string? range = "month")
        {
            // Lấy userId hiện tại
            var userId = _httpContextAccessor.GetUserId();
            var userSetting = _settingHandler.GetUserSettings();
            var currency = userSetting?.Currency ?? "VND";

            // Lấy symbol
            var symbol = await _currencyHandler.GetSymbolByCodeAsync(currency);

            var result = await _IncomeExpenseHandler.GetSummary(range, currency);

            if (result is OperationResult<IncomeExpenseummaryViewModel> summaryResult)
            {
                var model = summaryResult.Data;
                return Json(new
                {
                    totalIncomeFormatted = $"{model.TotalIncome:N0} {symbol}",
                    totalExpenseFormatted = $"{model.TotalExpense:N0} {symbol}",
                    remainingBalanceFormatted = $"{model.Balance:N0} {symbol}"
                });
            }

            return Json(new
            {
                totalIncomeFormatted = $"0 {symbol}",
                totalExpenseFormatted = $"0 {symbol}",
                remainingBalanceFormatted = $"0 {symbol}"
            });
        }



        [HttpPost]
        public async Task<IActionResult> Save(IncomeExpenseCreateUpdateModel model)
        {
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

        [HttpPost]
        public IActionResult SaveFormType(string formType)
        {
            if (string.IsNullOrEmpty(formType))
                return BadRequest(new { message = "FormType không hợp lệ" });

            var sessionModel = HttpContext.Session.GetObjectFromJson<IncomeExpenseSessionModel>(IE_SESSION_KEY)
                     ?? new IncomeExpenseSessionModel();

            sessionModel.FormType = formType;
            HttpContext.Session.SetObjectAsJson(IE_SESSION_KEY, sessionModel);

            return Json(new { success = true, formType = formType });
        }

    }
}