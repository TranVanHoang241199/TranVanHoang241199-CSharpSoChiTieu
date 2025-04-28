using System;
using System.Net;
using API_HotelManagement.common;
using AutoMapper;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CSharpSoChiTieu.Business.Services
{
    public class IncomeExpenseHandler : IIncomeExpenseHandler
    {
        private readonly CTDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IncomeExpenseHandler(CTDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OperationResult> Create(IncomeExpenseCreateUpdateModel model)
        {
            try
            {
                // Kiểm tra model và thực hiện các kiểm tra khác nếu cần thiết
                if (model == null)
                {
                    return new OperationResultError(HttpStatusCode.NotFound, "Không có thông tin nào được truyền đi.");
                }

                var entity = new ct_IncomeExpense
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Amount = model.Amount,
                    Description = model.Description,
                    Type = model.Type,
                    CategoryId = model.CategoryId,

                    //---------
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetExtensions.GetUserId(_httpContextAccessor),
                    //ModifiedBy = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                };

                // Thêm đối tượng vào DbContext
                _context.ct_IncomeExpense.Add(entity);

                // Lưu thay đổi vào database
                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var result = _mapper.Map<IncomeExpenseViewModel>(model);
                    return new OperationResultObject<IncomeExpenseViewModel>(result);
                }

                return new OperationResultError(HttpStatusCode.NotFound, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<OperationResult> Delete(Guid id)
        {
            try
            {
                var serviceToDelete = await _context.ct_IncomeExpense.FindAsync(id);

                if (serviceToDelete == null)
                {
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi.");
                }

                _context.ct_IncomeExpense.Remove(serviceToDelete);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    return new OperationResult<bool>(HttpStatusCode.OK, true, "Xoá thành công.");
                }

                return new OperationResult<bool>(HttpStatusCode.NotFound, false, "Xoá thất bại.");
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<OperationResult> GetIncomeExpenseById(Guid id)
        {
            try
            {
                // Truy vấn dữ liệu từ cơ sở dữ liệu sử dụng LINQ
                var result = await _context.ct_IncomeExpense
                    .Where(test => test.Id == id)
                    .Select(test => new IncomeExpenseViewModel
                    {
                        Id = test.Id,
                        Type = test.Type,
                        Date = test.Date,
                        Description = test.Description,
                        Amount = test.Amount,
                        CategoryId = test.CategoryId,

                        ModifiedDate = test.ModifiedDate,
                        ModifiedBy = test.ModifiedBy,
                        CreatedBy = test.CreatedBy,
                        CreatedDate = test.CreatedDate,
                    })
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    return new OperationResultError(HttpStatusCode.BadRequest, "The Service does not exist by Id: " + id);
                }

                return new OperationResult<IncomeExpenseViewModel>(result);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.BadRequest, "Đã xảy ra lỗi: " + ex.Message);
            }
        }



        public async Task<OperationResult> Gets(IncomeExpenseType Type = 0, string search = "", string range = "month")
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                var now = DateTime.Now;

                // Câu truy vấn cơ bản
                var query = _context.ct_IncomeExpense
                    .Where(o => o.CreatedBy.Equals(currentUserId));

                // Xử lý filter theo range
                DateTime startDate, endDate;

                switch (range)
                {
                    case "today":
                        startDate = now.Date;
                        endDate = now.Date.AddDays(1).AddTicks(-1);
                        break;

                    case "week":
                        var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                        startDate = now.AddDays(-diff).Date;
                        endDate = startDate.AddDays(7).AddTicks(-1);
                        break;

                    case "month":
                        startDate = new DateTime(now.Year, now.Month, 1);
                        endDate = startDate.AddMonths(1).AddTicks(-1);
                        break;

                    case "year":
                        startDate = new DateTime(now.Year, 1, 1);
                        endDate = new DateTime(now.Year, 12, 31).AddDays(1).AddTicks(-1);
                        break;

                    default:
                        startDate = now.Date;
                        endDate = now.Date.AddDays(1).AddTicks(-1);
                        break;
                }

                query = query.Where(o => o.Date >= startDate && o.Date <= endDate);

                // Filter theo Type nếu có
                if (Type != 0) query = query.Where(o => o.Type == Type);

                // Filter theo tìm kiếm
                if (!string.IsNullOrEmpty(search)) query = query.Where(o => o.Description.Contains(search.Trim()));

                // Lấy dữ liệu
                var data = await query.OrderByDescending(o => o.Date).ToListAsync();

                // Nhóm dữ liệu theo ngày và kết hợp thông tin Category
                var grouped = data
                    .GroupBy(o => o.Date.Date)
                    .Select(g => new IEGroupViewModel
                    {
                        Date = g.Key,
                        Items = g.Select(i => new IncomeExpenseViewModel
                        {
                            Amount = i.Amount,
                            Date = i.Date,
                            Type = i.Type,
                            Description = i.Description,
                            CategoryId = i.CategoryId,
                            CategoryName = i.CategoryId != null ? _context.ct_IncomeExpenseCategories
                                                            .Where(c => c.Id == i.CategoryId)
                                                            .Select(c => c.Name)
                                                            .FirstOrDefault() : "Unknown",
                            CategoryColor = i.CategoryId != null ? _context.ct_IncomeExpenseCategories
                                                            .Where(c => c.Id == i.CategoryId)
                                                            .Select(c => c.Color)
                                                            .FirstOrDefault() : "#000000", // Default color
                            CategoryIcon = i.CategoryId != null ? _context.ct_IncomeExpenseCategories
                                                            .Where(c => c.Id == i.CategoryId)
                                                            .Select(c => c.Icon)
                                                            .FirstOrDefault() : "default-icon" // Default icon
                        }).ToList()
                    }).ToList();

                return new OperationResultList<IEGroupViewModel>(grouped);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }



        public async Task<OperationResult> GetSummary(string? range = "month")
        {
            try
            {
                // Lấy ID của user hiện tại
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                // Xử lý filter theo range
                DateTime now = DateTime.Now;
                DateTime startDate = now.Date;
                DateTime endDate = now.Date.AddDays(1).AddTicks(-1); // cuối ngày hôm nay (23:59:59)

                if (range == "today")
                {
                    startDate = now.Date;
                    endDate = now.Date.AddDays(1).AddTicks(-1);
                }
                else if (range == "week")
                {
                    int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = now.AddDays(-diff).Date;
                    endDate = startDate.AddDays(7).AddTicks(-1);
                }
                else if (range == "month")
                {
                    startDate = new DateTime(now.Year, now.Month, 1);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                }
                else if (range == "year")
                {
                    startDate = new DateTime(now.Year, 1, 1);
                    endDate = new DateTime(now.Year, 12, 31).AddDays(1).AddTicks(-1);
                }

                // Lọc các khoản thu chi theo khoảng thời gian
                var transactions = await _context.ct_IncomeExpense
                    .Where(o => o.CreatedBy == currentUserId
                                && o.Date >= startDate
                                && o.Date <= endDate)
                    .ToListAsync();

                // Tính tổng thu và chi
                var totalIncome = transactions
                    .Where(o => o.Type == IncomeExpenseType.Income)
                    .Sum(o => o.Amount);

                var totalExpense = transactions
                    .Where(o => o.Type == IncomeExpenseType.Expense)
                    .Sum(o => o.Amount);

                var summary = new IncomeExpenseummaryViewModel
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                };

                return new OperationResult<IncomeExpenseummaryViewModel>(summary);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }



        public async Task<OperationResult> Update(Guid id, IncomeExpenseCreateUpdateModel model)
        {
            try
            {
                var incomeExpenseUpdate = await _context.ct_IncomeExpense.FindAsync(id);

                if (incomeExpenseUpdate == null)
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");

                incomeExpenseUpdate.Amount = model.Amount;
                incomeExpenseUpdate.Type = model.Type;
                incomeExpenseUpdate.Date = model.Date;
                incomeExpenseUpdate.CategoryId = model.CategoryId;
                incomeExpenseUpdate.Description = model.Description;

                //-------------
                incomeExpenseUpdate.ModifiedDate = DateTime.UtcNow;
                incomeExpenseUpdate.ModifiedBy = GetExtensions.GetUserId(_httpContextAccessor);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    return new OperationResult<bool>(HttpStatusCode.OK, true, "Thêm mới thành công");
                }

                return new OperationResult<bool>(HttpStatusCode.NotFound, false, "Thêm mới thất bại");
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}