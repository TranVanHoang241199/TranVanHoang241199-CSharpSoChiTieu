using System.Net;
using System.Threading.Tasks;
using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class IncomeExpensesController : Controller
    {
        private readonly IIncomeExpenseHandler _incomeExpensesHandler;

        private const string IE_CICKTHUCHI = "ClickThuChiConditon";

        public IncomeExpensesController(IIncomeExpenseHandler incomeExpensesHandler)
        {
            _incomeExpensesHandler = incomeExpensesHandler;
        }

        public IActionResult Index()
        {
            var value = HttpContext.Session.GetString(IE_CICKTHUCHI) ?? "income"; // Mặc định là thu
            ViewBag.FormType = value;



            return View();
        }

        public async Task<IActionResult> History()
        {
            var result = await _incomeExpensesHandler.Gets(); // hoặc truyền userId nếu cần

            if (result.Status != HttpStatusCode.OK)
                return Content("Lỗi khi lấy dữ liệu: " + result.Message);

            var data = (result as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

            return PartialView("_History", data);
        }

        public IActionResult Category(int type)
        {
            var data = new List<CategoryViewModel>();

            if (type == 1) // income
            {
                data = new List<CategoryViewModel>
        {
            new CategoryViewModel { Id = 1, Name = "salary", Text = "Lương", Icon = "attach_money", Color = "success" },
            new CategoryViewModel { Id = 2, Name = "bonus", Text = "Thưởng", Icon = "card_giftcard", Color = "info" },
            new CategoryViewModel { Id = 3, Name = "other", Text = "Khác", Icon = "more_horiz", Color = "secondary" },
        };
            }
            else if (type == 2) // expense
            {
                data = new List<CategoryViewModel>
        {
            new CategoryViewModel { Id = 4, Name = "food", Text = "Ăn uống", Icon = "restaurant", Color = "danger" },
            new CategoryViewModel { Id = 5, Name = "transport", Text = "Di chuyển", Icon = "directions_car", Color = "primary" },
            new CategoryViewModel { Id = 6, Name = "shopping", Text = "Mua sắm", Icon = "shopping_cart", Color = "warning" },
        };
            }

            return PartialView("_Category", data);
        }


        [HttpPost]
        public IActionResult LoadForm(string type)
        {
            if (type == "income")
            {
                HttpContext.Session.SetString(IE_CICKTHUCHI, "income");
                return PartialView("Income");
            }
            else if (type == "expense")
            {
                HttpContext.Session.SetString(IE_CICKTHUCHI, "expense");
                return PartialView("Expenses");
            }
            else
            {
                return Content("Form không tồn tại!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSummary()
        {
            var month = DateTime.Now.Month;
            var result = await _incomeExpensesHandler.GetSummary(month);

            if (result is OperationResult<IncomeExpenseSummaryViewModel> summaryResult)
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


        //public IActionResult Expenses()
        //{
        //    ViewData["ActivePage"] = "Expense"; // ← Quan trọng
        //    return View();
        //}

        //public IActionResult Income()
        //{
        //    ViewData["ActivePage"] = "Income"; // ← Quan trọng (khớp với điều kiện trong Partial View)
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Save(IncomeExpenseCreateUpdateModel model)
        {
            // Kiểm tra lỗi dữ liệu
            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError(nameof(model.Description), "Tên sản phẩm không được để trống");

            if (model.Amount <= 0)
                ModelState.AddModelError(nameof(model.Amount), "Số tiền phải lớn hơn 0");

            if (model.Date == default)
                ModelState.AddModelError(nameof(model.Date), "Ngày tháng không hợp lệ");

            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ");

            // Nếu là tạo mới
            if (model.Id == Guid.Empty)
            {
                var result = await _incomeExpensesHandler.Create(model);
                if (result.Status != HttpStatusCode.OK)
                    return Json(new { success = false, message = "Không thể tạo mới khoản chi tiêu." });
            }
            else // Cập nhật
            {
                var result = await _incomeExpensesHandler.Update(model.Id, model);
                if (result.Status != HttpStatusCode.OK)
                    return Json(new { success = false, message = "Không thể cập nhật khoản chi tiêu." });
            }

            return Json(new { success = true });
        }




    }
}