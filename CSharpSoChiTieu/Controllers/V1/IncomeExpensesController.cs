using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class IncomeExpensesController : Controller
    {
        private readonly IIncomeExpenseHandler _incomeExpensesHandler;

        public IncomeExpensesController(IIncomeExpenseHandler incomeExpensesHandler)
        {
            _incomeExpensesHandler = incomeExpensesHandler;
        }

        public IActionResult Index()
        {
            // Mặc định chuyển hướng đến trang Expenses
            //return RedirectToAction("Expenses");
            return View();
        }

        public IActionResult Expenses()
        {
            ViewData["ActivePage"] = "Expense"; // ← Quan trọng
            return View();
        }

        public IActionResult Income()
        {
            ViewData["ActivePage"] = "Income"; // ← Quan trọng (khớp với điều kiện trong Partial View)
            return View();
        }

        [HttpPost]
        public IActionResult Add(IncomeModel model)
        {
            if (ModelState.IsValid)
            {
                // Lưu vào database — ví dụ dùng Entity Framework
                _incomeExpensesHandler.Incomes.Add(model);

                return Json(new { success = true });
            }

            return BadRequest("Dữ liệu không hợp lệ");
        }


        [HttpPost]
        public IActionResult LoadForm(string type)
        {
            if (type == "income")
                return PartialView("Income");
            else if (type == "expense")
                return PartialView("Expenses");
            else
                return Content("Form không tồn tại!");
        }

        public IActionResult History()
        {
            
            var incomeData = new List<IncomeGroupViewModel>
            {
                new IncomeGroupViewModel
                {
                    Date = new DateTime(2025, 4, 16),
                    Items = new List<IncomeItemViewModel>
                    {
                        new IncomeItemViewModel { Type = "payments", Label = "Lương tháng 4", Amount = 10000000, Date = new DateTime(2025, 4, 16) },
                        new IncomeItemViewModel { Type = "military_tech", Label = "Thưởng hiệu suất", Amount = 5000000, Date = new DateTime(2025, 4, 16) }
                    }
                },
                new IncomeGroupViewModel
                {
                    Date = new DateTime(2025, 4, 15),
                    Items = new List<IncomeItemViewModel>
                    {
                        new IncomeItemViewModel { Type = "payments", Label = "Lương tháng 4", Amount = 10000000, Date = new DateTime(2025, 4, 15) },
                        new IncomeItemViewModel { Type = "military_tech", Label = "Thưởng hiệu suất", Amount = 5000000, Date = new DateTime(2025, 4, 15) }
                    }
                }
            };

            return PartialView("_History", incomeData);
        }


    }
}