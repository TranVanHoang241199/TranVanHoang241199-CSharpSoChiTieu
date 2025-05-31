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

        public ActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại sản phẩm";
            var data = new CategoryViewModel()
            {
                Order = 0,
                Color = "Vàng",
                Icon = "ABC",
                Type = IncomeExpenseType.Expense,
            };

            ShowViewBag();

            return View("Edit", data);
        }


        public async Task<IActionResult> Edit(Guid? id = null)
        {

            if (id == Guid.Empty)
                return RedirectToAction("Index");

            var operationResultData = await _ICategoryHandler.Get(id);
            var data = (operationResultData as OperationResult<CategoryViewModel>)?.Data ?? new CategoryViewModel();

            if (data == null)
                return RedirectToAction("Index");

            ShowViewBag();

            ViewBag.Title = "Cập nhật loại sản phẩm";
            return View(data);
        }
        private void ShowViewBag()
        {
            ViewBag.IconExpenseList = new List<SelectListItem>
            {
                new SelectListItem { Text = "🍔 Ăn uống", Value = "food" },
                new SelectListItem { Text = "🚗 Di chuyển", Value = "car" },
                new SelectListItem { Text = "🏠 Nhà cửa", Value = "home" },
                new SelectListItem { Text = "🎮 Giải trí", Value = "entertainment" },
                new SelectListItem { Text = "💡 Hóa đơn điện", Value = "electric" },
                new SelectListItem { Text = "💧 Nước sinh hoạt", Value = "water" },
                new SelectListItem { Text = "📱 Điện thoại", Value = "phone" },
                new SelectListItem { Text = "🎁 Quà tặng", Value = "gift" },
                new SelectListItem { Text = "🧾 Mua sắm", Value = "shopping" },
                new SelectListItem { Text = "🧘 Sức khỏe", Value = "health" },
                new SelectListItem { Text = "💳 Trả nợ", Value = "debt" }
            };

            ViewBag.IconIncomeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "💰 Lương", Value = "salary" },
                new SelectListItem { Text = "💵 Thưởng", Value = "bonus" },
                new SelectListItem { Text = "🏦 Lãi ngân hàng", Value = "interest" },
                new SelectListItem { Text = "📦 Bán hàng", Value = "sales" },
                new SelectListItem { Text = "🎁 Quà tặng", Value = "gift" },
                new SelectListItem { Text = "💸 Thu nhập thêm", Value = "extra-income" },
                new SelectListItem { Text = "👨‍👩‍👧‍👦 Người thân cho", Value = "family-support" },
                new SelectListItem { Text = "🔄 Hoàn tiền", Value = "refund" }
            };

            ViewBag.ColorList = new List<SelectListItem>
            {
                new SelectListItem { Text = "🔴 Đỏ", Value = "#ff0000" },
                new SelectListItem { Text = "🟢 Xanh lá", Value = "#00ff00" },
                new SelectListItem { Text = "🔵 Xanh dương", Value = "#0000ff" },
                new SelectListItem { Text = "🟡 Vàng", Value = "#ffff00" },
                new SelectListItem { Text = "🟠 Cam", Value = "#ffa500" },
                new SelectListItem { Text = "🟣 Tím", Value = "#800080" },
                new SelectListItem { Text = "⚫ Đen", Value = "#000000" },
                new SelectListItem { Text = "⚪ Trắng", Value = "#ffffff" },
                new SelectListItem { Text = "🟤 Nâu", Value = "#8B4513" },
                new SelectListItem { Text = "🩵 Xanh ngọc", Value = "#40E0D0" }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Save(CategoryInputModel data)
        {
            //Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.Name))
                ModelState.AddModelError(nameof(data.Name), "Tên loại sản phẩm không được để trống");

            //Kiểm tra dữ liệu đầu vào
            if (!Enum.IsDefined(typeof(IncomeExpenseType), data.Type))
                ModelState.AddModelError(nameof(data.Type), "Loại sản phẩm không hợp lệ");


            if (ModelState.IsValid == false)
            {
                ViewBag.Title = data.Id == Guid.Empty ? "Bổ sung loại sản phẩm" : "Cập nhật loại sản phẩm";
                return View("Edit", data);
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

            HttpContext.Session.SetObjectAsJson(CATEGORY_SEARCH, new PaginationSearchInput()
            {
                Page = condition.Page,
                PageSize = PAGE_SIZE,
                SearchValue = data.Name,
            });

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public async Task<IActionResult> Delete(Guid? id = null)
        //{
        //    if (id == Guid.Empty)
        //        return  RedirectToAction("Index");

        //    if (Request.Method == "POST")
        //    {
        //        await _ICategoryHandler.Delete(id);
        //        return RedirectToAction("Index");
        //    }

        //    var data = _ICategoryHandler.Get(id);

        //    if (data == null)
        //        return RedirectToAction("Index");

        //    return View(data);
        //}

        /// <summary>
        /// 
        /// </summary>
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
