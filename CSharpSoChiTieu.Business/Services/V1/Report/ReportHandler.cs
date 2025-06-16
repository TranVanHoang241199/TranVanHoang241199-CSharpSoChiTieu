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

        public async Task<ChartDataViewModel> GetChartData(string period) // Added yearWeek parameter
        {
            var result = new ChartDataViewModel();
            var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
            var query = _context.ct_IncomeExpense
                .Where(x => x.CreatedBy == currentUserId)
                .AsQueryable();

            switch (period.ToLower())
            {
                case "quarter":
                    // Your existing "quarter" logic
                    var quarterlyData = await query
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

                    result.Labels = quarterlyData.Select(x => $"Q{x.Quarter}/{x.Year}").ToList();
                    result.Income = quarterlyData.Select(x => x.Income).ToList();
                    result.Expense = quarterlyData.Select(x => x.Expense).ToList();
                    break;

                case "year":
                    // Your existing "year" logic
                    var yearlyData = await query
                        .GroupBy(x => x.Date.Year)
                        .Select(g => new
                        {
                            Year = g.Key,
                            Income = g.Where(x => x.Type == IncomeExpenseType.Income).Sum(x => x.Amount),
                            Expense = g.Where(x => x.Type == IncomeExpenseType.Expense).Sum(x => x.Amount)
                        })
                        .OrderBy(x => x.Year)
                        .ToListAsync();

                    result.Labels = yearlyData.Select(x => x.Year.ToString()).ToList();
                    result.Income = yearlyData.Select(x => x.Income).ToList();
                    result.Expense = yearlyData.Select(x => x.Expense).ToList();
                    break;

                case "week": // New case for "week"
                    if (string.IsNullOrEmpty(period))
                    {
                        // Handle case where week parameter is missing, maybe default to current week or throw error
                        // For now, let's just return empty data
                        return result;
                    }

                    var parts = period.Split("-W");
                    if (parts.Length != 2 || !int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int weekNumber))
                    {
                        // Invalid format for yearWeek
                        return result;
                    }

                    // Calculate start and end dates of the week
                    // Using ISO 8601 week calculation (Monday as first day)
                    var cal = CultureInfo.CurrentCulture.Calendar;
                    var jan1 = new DateTime(year, 1, 1);
                    var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek; // Adjust to find the Monday of Jan 1st's week

                    // Get the first Monday of the year (or previous year's Monday for week 1 if it starts in Dec)
                    var firstMonday = jan1.AddDays(daysOffset);
                    if (firstMonday.Year < year) // If the first Monday is in the previous year (e.g., Jan 1st is a Saturday)
                    {
                        firstMonday = firstMonday.AddDays(7);
                    }

                    // Calculate the start date of the target week
                    var startOfWeek = firstMonday.AddDays((weekNumber - 1) * 7);
                    var endOfWeek = startOfWeek.AddDays(6); // End of Sunday

                    // Query for daily data within this week
                    var weeklyDailyData = await query
                        .Where(x => x.Date >= startOfWeek && x.Date <= endOfWeek)
                        .GroupBy(x => x.Date.Date) // Group by exact date
                        .Select(g => new
                        {
                            Date = g.Key,
                            Income = g.Where(x => x.Type == IncomeExpenseType.Income).Sum(x => x.Amount),
                            Expense = g.Where(x => x.Type == IncomeExpenseType.Expense).Sum(x => x.Amount)
                        })
                        .OrderBy(x => x.Date)
                        .ToListAsync();

                    // Populate all days of the week, including those with no transactions
                    var allDatesInWeek = Enumerable.Range(0, 7)
                                                    .Select(offset => startOfWeek.AddDays(offset).Date)
                                                    .ToList();

                    foreach (var date in allDatesInWeek)
                    {
                        var dataForDay = weeklyDailyData.FirstOrDefault(d => d.Date == date);
                        result.Labels.Add(date.ToString("ddd dd/MM", CultureInfo.CurrentCulture)); // E.g., "Mon 15/06"
                        result.Income.Add(dataForDay?.Income ?? 0);
                        result.Expense.Add(dataForDay?.Expense ?? 0);
                    }
                    break;

                default:
                    // Your existing "month" logic
                    var monthlyData = await query
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

                    result.Labels = monthlyData.Select(x => $"{x.Month}/{x.Year}").ToList(); // Changed to Month/Year for better display
                    result.Income = monthlyData.Select(x => x.Income).ToList();
                    result.Expense = monthlyData.Select(x => x.Expense).ToList();
                    break;
            }

            return result;
        }
    }
}