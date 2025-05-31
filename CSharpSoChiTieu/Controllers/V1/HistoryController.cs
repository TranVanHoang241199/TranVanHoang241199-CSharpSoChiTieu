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
            PaginationSearchInput condition = HttpContext.Session.GetObjectFromJson<PaginationSearchInput>(IncomeExpense_SEARCH);

            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""

                };
            }
            return View(condition);
        }

        public async Task<IActionResult> Search(PaginationSearchInput condition)
        {
            // Đếm số lượng
            var operationResultCount = await _IncomeExpenseHandler.Count(condition.SearchValue);
            int rowCount = (operationResultCount as OperationResult<int>)?.Data ?? 0;

            // Lấy danh sách
            var operationResultData = await _IncomeExpenseHandler.Gets(condition.Page, condition.PageSize, condition.SearchValue);
            var data = (operationResultData as OperationResultList<IncomeExpenseViewModel>)?.Data ?? new List<IncomeExpenseViewModel>();

            // Khởi tạo kết quả trả về view
            var result = new IncomeExpenseSearchOutput
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            HttpContext.Session.SetObjectAsJson(IncomeExpense_SEARCH, condition);

            return View(result);
        }
    }
}
