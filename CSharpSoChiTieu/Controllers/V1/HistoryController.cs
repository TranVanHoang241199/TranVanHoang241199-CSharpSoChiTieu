using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
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
            // Xử lý filter nhanh theo thời gian
            if (!string.IsNullOrEmpty(condition.QuickDate))
            {
                var today = DateTime.Today;
                switch (condition.QuickDate.ToLower())
                {
                    case "today":
                        condition.FromDate = today;
                        condition.ToDate = today;
                        break;
                    case "thisweek":
                        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                        condition.FromDate = startOfWeek;
                        condition.ToDate = today;
                        break;
                    case "thismonth":
                        condition.FromDate = new DateTime(today.Year, today.Month, 1);
                        condition.ToDate = today;
                        break;
                    case "thisyear":
                        condition.FromDate = new DateTime(today.Year, 1, 1);
                        condition.ToDate = today;
                        break;
                }
            }

            // Đếm số lượng
            var operationResultCount = await _IncomeExpenseHandler.Count(condition.SearchValue, condition.Year, condition.Month, condition.Day);
            int rowCount = (operationResultCount as OperationResult<int>)?.Data ?? 0;

            // Lấy danh sách group theo ngày
            var operationResultData = await _IncomeExpenseHandler.Gets(condition.Page, condition.PageSize, condition.SearchValue, condition.Year, condition.Month, condition.Day, condition.currency);
            var data = (operationResultData as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

            // Áp dụng các filter bổ sung
            var filteredData = ApplyFilters(data, condition);

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
                Groups = filteredData
            };

            HttpContext.Session.SetObjectAsJson(IncomeExpense_SEARCH, condition);

            return View(result);
        }

        private List<IEGroupViewModel> ApplyFilters(List<IEGroupViewModel> data, PaginationHistorySearchInput condition)
        {
            var filteredData = new List<IEGroupViewModel>();

            foreach (var group in data)
            {
                var filteredItems = group.Items.Where(item =>
                {
                    // Filter theo loại giao dịch
                    if (!string.IsNullOrEmpty(condition.Type) && item.Type.ToString() != condition.Type)
                        return false;

                    // Filter theo khoảng tiền
                    if (condition.AmountFrom.HasValue && item.Amount < condition.AmountFrom.Value)
                        return false;
                    if (condition.AmountTo.HasValue && item.Amount > condition.AmountTo.Value)
                        return false;

                    // Filter theo khoảng ngày
                    if (condition.FromDate.HasValue && item.Date < condition.FromDate.Value)
                        return false;
                    if (condition.ToDate.HasValue && item.Date > condition.ToDate.Value)
                        return false;

                    return true;
                }).ToList();

                if (filteredItems.Any())
                {
                    filteredData.Add(new IEGroupViewModel
                    {
                        Date = group.Date,
                        Items = filteredItems
                    });
                }
            }

            return filteredData;
        }

        [HttpGet]
        public async Task<IActionResult> GetSummary(int? year, int? month, int? day, string searchValue, string currency,
            string type = null, decimal? amountFrom = null, decimal? amountTo = null,
            DateTime? fromDate = null, DateTime? toDate = null, string quickDate = null)
        {
            // Xử lý filter nhanh theo thời gian
            if (!string.IsNullOrEmpty(quickDate))
            {
                var today = DateTime.Today;
                switch (quickDate.ToLower())
                {
                    case "today":
                        fromDate = today;
                        toDate = today;
                        break;
                    case "thisweek":
                        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                        fromDate = startOfWeek;
                        toDate = today;
                        break;
                    case "thismonth":
                        fromDate = new DateTime(today.Year, today.Month, 1);
                        toDate = today;
                        break;
                    case "thisyear":
                        fromDate = new DateTime(today.Year, 1, 1);
                        toDate = today;
                        break;
                }
            }

            var result = await _IncomeExpenseHandler.GetSummary(year, month, day, searchValue, currency);

            var symbol = await _currencyHandler.GetSymbolByCodeAsync(currency);

            if (result is OperationResult<IncomeExpenseummaryViewModel> summaryResult)
            {
                var model = summaryResult.Data;

                // Áp dụng filter cho summary nếu cần
                if (!string.IsNullOrEmpty(type) || amountFrom.HasValue || amountTo.HasValue || fromDate.HasValue || toDate.HasValue)
                {
                    // Lấy dữ liệu gốc để filter
                    var operationResultData = await _IncomeExpenseHandler.Gets(1, 1000000, searchValue, year, month, day, currency);
                    var data = (operationResultData as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

                    // Tạo condition để filter
                    var condition = new PaginationHistorySearchInput
                    {
                        Type = type,
                        AmountFrom = amountFrom,
                        AmountTo = amountTo,
                        FromDate = fromDate,
                        ToDate = toDate
                    };

                    var filteredData = ApplyFilters(data, condition);

                    // Tính toán lại summary từ dữ liệu đã filter
                    decimal totalIncome = 0, totalExpense = 0;
                    foreach (var group in filteredData)
                    {
                        foreach (var item in group.Items)
                        {
                            if (item.Type == IncomeExpenseType.Income)
                                totalIncome += item.Amount;
                            else
                                totalExpense += item.Amount;
                        }
                    }

                    model.TotalIncome = totalIncome;
                    model.TotalExpense = totalExpense;
                }

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
