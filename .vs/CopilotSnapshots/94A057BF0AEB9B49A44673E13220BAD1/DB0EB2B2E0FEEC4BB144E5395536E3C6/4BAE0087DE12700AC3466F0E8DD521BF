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

        public async Task<ReportViewModel> GetReportData(ReportFilterModel filter)
        {
            var result = new ReportViewModel();

            var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

            var query = _context.ct_IncomeExpense.Where(x => x.CreatedBy == currentUserId);

            // Lọc theo loại (thu/chi)
            if (!string.IsNullOrEmpty(filter.Type) && filter.Type != "all")
            {
                var incomeExpenseType = filter.Type.ToLower() == "income" ? IncomeExpenseType.Income : IncomeExpenseType.Expense;
                query = query.Where(x => x.Type == incomeExpenseType);
            }

            // Lọc theo đơn vị tiền tệ
            if (!string.IsNullOrEmpty(filter.Currency))
            {
                query = query.Where(x => x.Currency == filter.Currency);
            }

            // Lọc theo khoảng thời gian
            if (filter.DateFrom.HasValue)
            {
                query = query.Where(x => x.Date >= filter.DateFrom.Value);
            }
            if (filter.DateTo.HasValue)
            {
                query = query.Where(x => x.Date <= filter.DateTo.Value.AddDays(1).AddTicks(-1)); // bao gồm cả ngày kết thúc
            }

            // Lấy tổng thu và tổng chi từ query đã lọc
            result.TotalIncome = await query.Where(x => x.Type == IncomeExpenseType.Income).SumAsync(x => x.Amount);
            result.TotalExpense = await query.Where(x => x.Type == IncomeExpenseType.Expense).SumAsync(x => x.Amount);
            result.Balance = result.TotalIncome - result.TotalExpense;
            result.TotalTransactions = await query.CountAsync();
            result.IncomeTransactions = await query.Where(x => x.Type == IncomeExpenseType.Income).CountAsync();
            result.ExpenseTransactions = await query.Where(x => x.Type == IncomeExpenseType.Expense).CountAsync();

            // Lấy thống kê theo tháng
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

            result.MonthlyStats = monthlyData.Select(x => new MonthlyStatViewModel
            {
                Month = $"{x.Month}/{x.Year}", // Format để Chart.js có thể đọc
                Income = x.Income,
                Expense = x.Expense
            }).ToList();

            // Lấy thống kê theo danh mục từ query đã lọc
            var categoryStatsQuery = query
    .Join(_context.ct_IncomeExpenseCategories,
        ie => ie.CategoryId,
        c => c.Id,
        (ie, c) => new { ie, c });

            // Nếu lọc theo loại, chỉ lấy đúng loại đó
            if (!string.IsNullOrEmpty(filter.Type) && filter.Type != "all")
            {
                var incomeExpenseType = filter.Type.ToLower() == "income" ? IncomeExpenseType.Income : IncomeExpenseType.Expense;
                categoryStatsQuery = categoryStatsQuery.Where(x => x.ie.Type == incomeExpenseType);
            }

            var grouped = categoryStatsQuery
                .GroupBy(x => new { x.c.Id, x.c.Name, x.ie.Type })
                .Select(g => new
                {
                    CategoryId = (Guid?)g.Key.Id,
                    CategoryName = g.Key.Name,
                    Type = g.Key.Type,
                    Amount = g.Sum(x => x.ie.Amount)
                });

            var categoryStats = await grouped.ToListAsync();
            var totalAmountForPercentage = categoryStats.Sum(x => x.Amount);

            result.CategoryStats = categoryStats.Select(x => new CategoryStatViewModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Type = x.Type,
                Amount = x.Amount,
                Percentage = totalAmountForPercentage > 0 ? (int)((x.Amount * 100) / totalAmountForPercentage) : 0
            }).ToList();

            return result;
        }
    }
}