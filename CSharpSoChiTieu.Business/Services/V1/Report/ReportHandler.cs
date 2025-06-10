using API_HotelManagement.common;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
            var result = new ChartDataViewModel();
            var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
            var query = _context.ct_IncomeExpense
                .Where(x => x.CreatedBy == currentUserId)
                .AsQueryable();

            switch (period.ToLower())
            {
                case "quarter":
                    // Nhóm theo quý
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
                    // Nhóm theo năm
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

                default:
                    // Mặc định nhóm theo tháng
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

                    result.Labels = monthlyData.Select(x => $"{x.Year}/{x.Month}").ToList();
                    result.Income = monthlyData.Select(x => x.Income).ToList();
                    result.Expense = monthlyData.Select(x => x.Expense).ToList();
                    break;
            }

            return result;
        }
    }
}