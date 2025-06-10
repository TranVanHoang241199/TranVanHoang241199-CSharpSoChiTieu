using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string IncomeExpense_SEARCH = "SearchHistoryCondition";
        private readonly IIncomeExpenseHandler _IncomeExpenseHandler;

        public HistoryController(IIncomeExpenseHandler IncomeExpenseHandler)
        {
            _IncomeExpenseHandler = IncomeExpenseHandler;
        }

        public ActionResult Index()
        {
            PaginationHistorySearchInput condition = HttpContext.Session.GetObjectFromJson<PaginationHistorySearchInput>(IncomeExpense_SEARCH);

            if (condition == null)
            {
                condition = new PaginationHistorySearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    Day = DateTime.Now.Day

                };
            }
            return View(condition);
        }

        public async Task<IActionResult> Search(PaginationHistorySearchInput condition)
        {
            // Đếm số lượng
            var operationResultCount = await _IncomeExpenseHandler.Count(condition.SearchValue);
            int rowCount = (operationResultCount as OperationResult<int>)?.Data ?? 0;

            // Lấy danh sách
            var operationResultData = await _IncomeExpenseHandler.Gets(condition.Page, condition.PageSize, condition.SearchValue);
            var data = (operationResultData as OperationResultList<IncomeExpenseViewModel>)?.Data ?? new List<IncomeExpenseViewModel>();

            // Lọc theo ngày tháng năm nếu có
            if (condition.Year.HasValue || condition.Month.HasValue || condition.Day.HasValue)
            {
                data = data.Where(x =>
                {
                    if (x.CreatedDate == null) return false;

                    var date = x.CreatedDate.Value;
                    return (!condition.Year.HasValue || date.Year == condition.Year.Value) &&
                           (!condition.Month.HasValue || date.Month == condition.Month.Value) &&
                           (!condition.Day.HasValue || date.Day == condition.Day.Value);
                }).ToList();
            }

            // Khởi tạo kết quả trả về view
            var result = new HistorySearchOutput
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data,
                Year = condition.Year,
                Month = condition.Month,
                Day = condition.Day,
            };

            HttpContext.Session.SetObjectAsJson(IncomeExpense_SEARCH, condition);

            return View(result);
        }
    }
}
