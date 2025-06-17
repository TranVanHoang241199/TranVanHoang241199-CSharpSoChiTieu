using API_HotelManagement.common;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CSharpSoChiTieu.Business.Services
{
    public class ReportHandler : IReportHandler
    {
        private readonly CTDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportHandler(CTDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ReportViewModel> GetReportData()
        {
            var result = new ReportViewModel();

            var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

            // Lấy tổng thu
            result.TotalIncome = await _context.ct_IncomeExpense
                .Where(x => x.Type == IncomeExpenseType.Income && x.CreatedBy == currentUserId)
                .SumAsync(x => x.Amount);

            // Lấy tổng chi
            result.TotalExpense = await _context.ct_IncomeExpense
                .Where(x => x.Type == IncomeExpenseType.Expense && x.CreatedBy == currentUserId)
                .SumAsync(x => x.Amount);

            // Tính số dư
            result.Balance = result.TotalIncome - result.TotalExpense;

            // Lấy tổng số giao dịch
            result.TotalTransactions = await _context.ct_IncomeExpense
                .Where(x => x.CreatedBy == currentUserId)
                .CountAsync();

            result.IncomeTransactions = await _context.ct_IncomeExpense
                .Where(x => x.CreatedBy == currentUserId && x.Type == IncomeExpenseType.Income)
                .CountAsync();

            result.ExpenseTransactions = await _context.ct_IncomeExpense
                .Where(x => x.CreatedBy == currentUserId && x.Type == IncomeExpenseType.Expense)
                .CountAsync();

            // Lấy thống kê theo tháng
            var monthlyData = await _context.ct_IncomeExpense
                .Where(x => x.CreatedBy == currentUserId)
                .GroupBy(x => new { x.Date.Year, x.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Income = g.Where(x => x.Type == IncomeExpenseType.Income).Sum(x => x.Amount),
                    Expense = g.Where(x => x.Type == IncomeExpenseType.Expense).Sum(x => x.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            // Chuyển đổi dữ liệu sang MonthlyStatViewModel
            result.MonthlyStats = monthlyData.Select(x => new MonthlyStatViewModel
            {
                Month = $"{x.Year}/{x.Month}",
                Income = x.Income,
                Expense = x.Expense
            }).ToList();

            // Lấy thống kê theo danh mục
            var categoryStats = await _context.ct_IncomeExpense
                .Where(x => x.CreatedBy == currentUserId)
                .Join(_context.ct_IncomeExpenseCategories,
                    ie => ie.CategoryId,
                    c => c.Id,
                    (ie, c) => new
                    {
                        CategoryId = ie.CategoryId,
                        CategoryName = c.Name,
                        Type = ie.Type,
                        Amount = ie.Amount
                    })
                .GroupBy(x => new { x.CategoryId, x.CategoryName, x.Type })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    Type = g.Key.Type,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            // Tính tỷ lệ phần trăm cho mỗi danh mục
            var totalAmount = categoryStats.Sum(x => x.Amount);
            result.CategoryStats = categoryStats.Select(x => new CategoryStatViewModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Type = x.Type,
                Amount = x.Amount,
                Percentage = totalAmount > 0 ? (int)((x.Amount * 100) / totalAmount) : 0
            }).ToList();

            return result;
        }

        public async Task<ChartDataViewModel> GetChartData(string period)
        {
            var userId = GetExtensions.GetUserId(_httpContextAccessor);
            var now = DateTime.Now;
            var startDate = now.Date;
            var endDate = now.Date;
            var labels = new List<string>();
            var incomeData = new List<decimal>();
            var expenseData = new List<decimal>();

            switch (period.ToLower())
            {
                case "week":
                    // Tính ngày đầu và cuối của tuần hiện tại
                    startDate = now.Date.AddDays(-(int)now.DayOfWeek + 1); // Thứ 2
                    endDate = startDate.AddDays(6); // Chủ nhật

                    // Tạo danh sách các ngày trong tuần
                    var daysInWeek = new List<string> { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
                    labels = daysInWeek;

                    // Lấy dữ liệu thu chi cho từng ngày trong tuần
                    var dailyData = await _context.ct_IncomeExpense
                        .Where(t => t.CreatedBy == userId &&
                               t.Date.Date >= startDate &&
                               t.Date.Date <= endDate)
                        .GroupBy(t => t.Date.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            Income = g.Where(t => t.Type == IncomeExpenseType.Income).Sum(t => t.Amount),
                            Expense = g.Where(t => t.Type == IncomeExpenseType.Expense).Sum(t => t.Amount)
                        })
                        .ToListAsync();

                    // Khởi tạo mảng dữ liệu với giá trị 0 cho tất cả các ngày
                    incomeData = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
                    expenseData = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };

                    // Cập nhật dữ liệu cho các ngày có giao dịch
                    foreach (var data in dailyData)
                    {
                        var dayIndex = (int)(data.Date - startDate).TotalDays;
                        if (dayIndex >= 0 && dayIndex < 7)
                        {
                            incomeData[dayIndex] = data.Income;
                            expenseData[dayIndex] = data.Expense;
                        }
                    }
                    break;

                case "quarter":
                    // Your existing "quarter" logic
                    var quarterlyData = await _context.ct_IncomeExpense
                        .Where(x => x.CreatedBy == userId)
                        .GroupBy(x => new { x.Date.Year, Quarter = (x.Date.Month - 1) / 3 + 1 })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Quarter = g.Key.Quarter,
                            Income = g.Where(x => x.Type == IncomeExpenseType.Income).Sum(x => x.Amount),
                            Expense = g.Where(x => x.Type == IncomeExpenseType.Expense).Sum(x => x.Amount)
                        })
                        .OrderBy(x => x.Year)
                        .ThenBy(x => x.Quarter)
                        .ToListAsync();

                    labels = quarterlyData.Select(x => $"Q{x.Quarter}/{x.Year}").ToList();
                    incomeData = quarterlyData.Select(x => x.Income).ToList();
                    expenseData = quarterlyData.Select(x => x.Expense).ToList();
                    break;

                case "year":
                    // Your existing "year" logic
                    var yearlyData = await _context.ct_IncomeExpense
                        .Where(x => x.CreatedBy == userId)
                        .GroupBy(x => x.Date.Year)
                        .Select(g => new
                        {
                            Year = g.Key,
                            Income = g.Where(x => x.Type == IncomeExpenseType.Income).Sum(x => x.Amount),
                            Expense = g.Where(x => x.Type == IncomeExpenseType.Expense).Sum(x => x.Amount)
                        })
                        .OrderBy(x => x.Year)
                        .ToListAsync();

                    labels = yearlyData.Select(x => x.Year.ToString()).ToList();
                    incomeData = yearlyData.Select(x => x.Income).ToList();
                    expenseData = yearlyData.Select(x => x.Expense).ToList();
                    break;

                default:
                    // Your existing "month" logic
                    var monthlyData = await _context.ct_IncomeExpense
                        .Where(x => x.CreatedBy == userId)
                        .GroupBy(x => new { x.Date.Year, x.Date.Month })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Income = g.Where(x => x.Type == IncomeExpenseType.Income).Sum(x => x.Amount),
                            Expense = g.Where(x => x.Type == IncomeExpenseType.Expense).Sum(x => x.Amount)
                        })
                        .OrderBy(x => x.Year)
                        .ThenBy(x => x.Month)
                        .ToListAsync();

                    labels = monthlyData.Select(x => $"{x.Month}/{x.Year}").ToList(); // Changed to Month/Year for better display
                    incomeData = monthlyData.Select(x => x.Income).ToList();
                    expenseData = monthlyData.Select(x => x.Expense).ToList();
                    break;
            }

            return new ChartDataViewModel
            {
                Labels = labels,
                Income = incomeData,
                Expense = expenseData
            };
        }
    }
}