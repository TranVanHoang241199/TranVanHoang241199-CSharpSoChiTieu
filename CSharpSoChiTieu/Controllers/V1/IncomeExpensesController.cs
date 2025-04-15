using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return RedirectToAction("Expenses");
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

        // Thêm mới thu chi
        public IActionResult Create()
        {
            return View();
        }

    }
}
