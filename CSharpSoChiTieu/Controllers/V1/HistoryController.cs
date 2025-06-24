using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    /// <summary>
    /// Truy cập điều khiển xử lý danh mục 
    /// </summary>
    [Authorize]
    public class HistoryController : Controller
    {
        private const int PAGE_SIZE = 100000;
        private const string IncomeExpense_SEARCH = "SearchHistoryCondition";
        private readonly ICurrencyHandler _currencyHandler;
        private readonly ISettingHandler _settingHandler;
        private readonly IIncomeExpenseHandler _IncomeExpenseHandler;

        public HistoryController(IIncomeExpenseHandler IncomeExpenseHandler, ICurrencyHandler currencyHandler, ISettingHandler settingHandler)
        {
            _IncomeExpenseHandler = IncomeExpenseHandler;
            _currencyHandler = currencyHandler;
            _settingHandler = settingHandler;
        }

        public async Task<IActionResult> Index()
        {
            PaginationHistorySearchInput condition = HttpContext.Session.GetObjectFromJson<PaginationHistorySearchInput>(IncomeExpense_SEARCH);

            if (condition == null)
            {
                condition = new PaginationHistorySearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Year = DateTime.UtcNow.Year,
                    Month = DateTime.UtcNow.Month,
                    Day = DateTime.UtcNow.Day

                };
            }

            // Lấy danh sách tiền tệ
            var currencyResult = await _currencyHandler.GetAll();
            var currencies = (currencyResult as OperationResultList<CurrencyViewModel>)?.Data ?? new List<CurrencyViewModel>();
            ViewBag.Currencies = currencies;

            // Lấy đơn vị tiền tệ đang lưu trong user setting
            var userSetting = _settingHandler.GetUserSettings();
            ViewBag.SelectedCurrency = userSetting?.Currency ?? "VND";

            return View(condition);
        }

        public async Task<IActionResult> Search(PaginationHistorySearchInput condition)
        {
            // Đếm số lượng
            var operationResultCount = await _IncomeExpenseHandler.Count(condition.SearchValue, condition.Year, condition.Month, condition.Day);
            int rowCount = (operationResultCount as OperationResult<int>)?.Data ?? 0;

            // Lấy danh sách group theo ngày
            var operationResultData = await _IncomeExpenseHandler.Gets(condition.Page, condition.PageSize, condition.SearchValue, condition.Year, condition.Month, condition.Day, condition.currency);
            var data = (operationResultData as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

            // Lấy toàn bộ symbol cho các loại currency
            var allCurrencies = await _currencyHandler.GetAll() as OperationResultList<CurrencyViewModel>;
            var symbolDict = allCurrencies?.Data?.ToDictionary(c => c.Code, c => c.Symbol) ?? new Dictionary<string, string>();
            ViewBag.Symbols = symbolDict;

            // Khởi tạo kết quả trả về view
            var result = new HistorySearchOutput
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Year = condition.Year,
                Month = condition.Month,
                Day = condition.Day,
                // Chuyển sang property mới cho group
                Groups = data
            };

            HttpContext.Session.SetObjectAsJson(IncomeExpense_SEARCH, condition);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSummary(int? year, int? month, int? day, string searchValue, string currency)
        {
            var result = await _IncomeExpenseHandler.GetSummary(year, month, day, searchValue, currency);

            var symbol = await _currencyHandler.GetSymbolByCodeAsync(currency);

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

        public IActionResult FilterDialog()
        {
            return PartialView("_FilterDialog");
        }
    }
}
