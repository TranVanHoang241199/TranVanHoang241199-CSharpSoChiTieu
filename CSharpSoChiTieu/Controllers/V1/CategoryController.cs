using System.Net;
using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSharpSoChiTieu.Controllers
{
    /// <summary>
    /// Truy cập điều khiển xử lý danh mục 
    /// </summary>
    [Authorize]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string CATEGORY_SEARCH = "SearchCategoryCondition";
        private readonly ICategoryHandler _ICategoryHandler;
        private readonly IEmojiHandler _emojiHandler;

        public CategoryController(ICategoryHandler incomeExpenseHandler, IEmojiHandler emojiHandler)
        {
            _ICategoryHandler = incomeExpenseHandler;
            _emojiHandler = emojiHandler;
        }

        /// <summary>
        /// 1.1 Truy cập view hiển thị danh mục
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 1.2 bộ điều kiển trả danh sách dựa trên thông tin lấy từ index
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 2.1 truy cập view tạo danh mục
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung loại sản phẩm";
            var data = new CategoryViewModel()
            {
                Order = 0,
                Color = "Vàng",
                Icon = "ABC",
                Type = IncomeExpenseType.Expense,
            };

            await ShowViewBag();

            return View("Edit", data);
        }

        /// <summary>
        /// 2.2 Truy cập view edit danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(Guid? id = null)
        {

            if (id == Guid.Empty)
                return RedirectToAction("Index");

            var operationResultData = await _ICategoryHandler.Get(id);
            var data = (operationResultData as OperationResult<CategoryViewModel>)?.Data ?? new CategoryViewModel();

            if (data == null)
                return RedirectToAction("Index");

            await ShowViewBag();

            ViewBag.Title = "Cập nhật loại sản phẩm";
            return View(data);
        }

        /// <summary>
        /// 2.3 View hiển thị danh sách icon và màu
        /// </summary>
        /// <returns></returns>
        private async Task ShowViewBag()
        {
            var IconExpenseList = new List<EmojiViewModel>();
            var IconIncomeList = new List<EmojiViewModel>();

            IconExpenseList = (await _emojiHandler.Gets(IncomeExpenseType.Expense, "") as OperationResultList<EmojiViewModel>)?.Data ?? new List<EmojiViewModel>();
            IconIncomeList = (await _emojiHandler.Gets(IncomeExpenseType.Income, "") as OperationResultList<EmojiViewModel>)?.Data ?? new List<EmojiViewModel>();

            ViewBag.IconExpenseList = IconExpenseList;
            ViewBag.IconIncomeList = IconIncomeList;

            ViewBag.ColorList = new List<SelectListItem>
            {
                new SelectListItem { Text = "🔴 Đỏ", Value = "#FF3B30" },
                new SelectListItem { Text = "🟢 Xanh lá", Value = "#34C759" },
                new SelectListItem { Text = "🔵 Xanh dương", Value = "#007AFF" },
                new SelectListItem { Text = "🟡 Vàng", Value = "#FFCC00" },
                new SelectListItem { Text = "🟠 Cam", Value = "#FF9500" },
                new SelectListItem { Text = "🟣 Tím", Value = "#AF52DE" },
                new SelectListItem { Text = "⚫ Đen", Value = "#1C1C1E" },
                new SelectListItem { Text = "⚪ Trắng", Value = "#FFFFFF" },
                new SelectListItem { Text = "🟤 Nâu", Value = "#A0522D" },
                new SelectListItem { Text = "🌸 Hồng pastel", Value = "#FFB6C1" },
                new SelectListItem { Text = "🌿 Xanh bạc hà", Value = "#98FF98" },
                new SelectListItem { Text = "🌊 Xanh biển", Value = "#00CED1" },
                new SelectListItem { Text = "🧁 Hồng kem", Value = "#FADADD" },
                new SelectListItem { Text = "🧊 Xanh băng", Value = "#D0F0FD" },
                new SelectListItem { Text = "🌫️ Xám nhẹ", Value = "#E0E0E0" },
                new SelectListItem { Text = "⛰️ Nâu đá", Value = "#8B8680" },
                new SelectListItem { Text = "🌅 Cam hoàng hôn", Value = "#FFA07A" }
            };

        }

        /// <summary>
        /// 2.4 Xử lý lưu khi chỉnh sửa và cài đặt
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Save(CategoryInputModel data)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Title = data.Id == Guid.Empty ? "Bổ sung loại sản phẩm" : "Cập nhật loại sản phẩm";

                // Chuyển đổi CategoryInputModel thành CategoryViewModel để phù hợp với view
                var viewModel = new CategoryViewModel
                {
                    Id = data.Id,
                    Name = data.Name,
                    Text = data.Text,
                    Icon = data.Icon,
                    Color = data.Color,
                    Order = data.Order,
                    Type = data.Type
                };

                await ShowViewBag();
                return View("Edit", viewModel);
            }

            if (data.Id == Guid.Empty)
            {
                await _ICategoryHandler.Add(data);
            }
            else
            {
                await _ICategoryHandler.Update(data);
            }

            PaginationSearchInput condition = HttpContext.Session.GetObjectFromJson<PaginationSearchInput>(CATEGORY_SEARCH);

            // Kiểm tra condition có null không để tránh lỗi NullReferenceException
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            HttpContext.Session.SetObjectAsJson(CATEGORY_SEARCH, new PaginationSearchInput()
            {
                Page = condition.Page,
                PageSize = PAGE_SIZE,
                SearchValue = data.Name,
            });

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 3.1 ĐK xử lý xoá danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(Guid? id = null)
        {
            if (id == null || id == Guid.Empty)
                return Json(new { success = false, message = "ID không hợp lệ" });

            try
            {
                var result = await _ICategoryHandler.Delete(id);
                if (result.Status == HttpStatusCode.OK)
                {
                    return Json(new { success = true, message = "Xóa danh mục thành công" });
                }
                return Json(new { success = false, message = result.Message ?? "Có lỗi xảy ra khi xóa danh mục" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa danh mục" });
            }
        }
    }
}
