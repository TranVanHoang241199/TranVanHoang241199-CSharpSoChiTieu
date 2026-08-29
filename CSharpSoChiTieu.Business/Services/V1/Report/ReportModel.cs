using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Business.Services
{
    public class ReportViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public int IncomeTransactions { get; set; }
        public int ExpenseTransactions { get; set; }
        public int TotalTransactions { get; set; }

        // Aggregations for different periods
        public List<MonthlyStatViewModel> MonthlyStats { get; set; }
        public List<WeekStatViewModel> WeekStats { get; set; }
        public List<DayStatViewModel> DayStats { get; set; }
        public List<HourlyStatViewModel> HourlyStats { get; set; }

        public List<CategoryStatViewModel> CategoryStats { get; set; }
    }

    public class MonthlyStatViewModel
    {
        public string Month { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }

    public class WeekStatViewModel
    {
        public string Week { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }

    public class DayStatViewModel
    {
        public string DayLabel { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }

    public class HourlyStatViewModel
    {
        public string Hour { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }

    public class CategoryStatViewModel
    {
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public IncomeExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public int Percentage { get; set; }
    }

    public class ChartDataViewModel
    {
        public List<string> Labels { get; set; }
        public List<decimal> Income { get; set; }
        public List<decimal> Expense { get; set; }
    }

    public class ReportFilterModel
    {
        public string? Type { get; set; }
        public string? Currency { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Period { get; set; }
    }
}
