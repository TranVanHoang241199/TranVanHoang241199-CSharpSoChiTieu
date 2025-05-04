using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string CATEGORY_SEARCH = "SearchCategoryCondition";
        private readonly ICategoryHandler _ICategoryHandler;

        public CategoryController(ICategoryHandler IncomeExpenseHandler)
        {
            _ICategoryHandler = IncomeExpenseHandler;
        }

        public ActionResult Index()
        {
            PaginationSearchInput condition = HttpContext.Session.GetObjectFromJson<PaginationSearchInput>(CATEGORY_SEARCH);

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
            var operationResultCount = await _ICategoryHandler.Count(condition.SearchValue);
            int rowCount = (operationResultCount as OperationResult<int>)?.Data ?? 0;

            // Lấy danh sách
            var operationResultData = await _ICategoryHandler.Gets(condition.Page, condition.PageSize, condition.SearchValue);
            var data = (operationResultData as OperationResultList<CategoryViewModel>)?.Data ?? new List<CategoryViewModel>();

            // Khởi tạo kết quả trả về view
            var result = new CategorySearchOutput
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };

            HttpContext.Session.SetObjectAsJson(CATEGORY_SEARCH, condition);

            return View(result);
        }
    }
}
