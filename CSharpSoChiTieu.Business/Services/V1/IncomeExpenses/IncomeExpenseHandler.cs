using System.Net;
using API_HotelManagement.common;
using AutoMapper;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CSharpSoChiTieu.Business.Services
{
    /// <summary>
    /// Xử lý phần thu chi
    /// </summary>
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

        /// <summary>
        /// Đếm số lượng thu chi hiện có trong trang
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<OperationResult> Count(string searchValue = "")
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                var query = _context.ct_IncomeExpense
                    .Where(o => o.CreatedBy == currentUserId);

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    var keyword = searchValue.Trim();
                    query = query.Where(o => o.Description.Contains(keyword));
                }

                var count = await query.CountAsync();

                return new OperationResult<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Đếm số lượng phục vụ hiển thị số lượng ở title trong history
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public async Task<OperationResult> Count(string searchValue = "", int? year = null, int? month = null, int? day = null)
        {
            try
            {
                var query = _context.ct_IncomeExpense.AsQueryable();

                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                query = query.Where(o => o.CreatedBy.Equals(currentUserId));

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(x => x.Description.Contains(searchValue));
                }

                // Apply date filtering based on provided parameters
                if (year.HasValue)
                {
                    query = query.Where(o => o.Date.Year == year.Value);

                    if (month.HasValue)
                    {
                        query = query.Where(o => o.Date.Month == month.Value);

                        if (day.HasValue)
                        {
                            query = query.Where(o => o.Date.Day == day.Value);
                        }
                    }
                }

                int count = await query.CountAsync();

                return new OperationResult<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Tạo khoản thu khoản chi mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                    Date = model.Date,
                    Amount = model.Amount,
                    Description = model.Description,
                    Type = model.Type,
                    CategoryId = model.CategoryId,
                    Currency = model.Currency,

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

        /// <summary>
        /// Xoá khoản chi khoản thu 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// đã tối ưu
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<OperationResult> GetCategories(IncomeExpenseType type = IncomeExpenseType.Null)
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                var query = _context.ct_IncomeExpenseCategories
                    .Where(o => o.CreatedBy == currentUserId);

                if (type != IncomeExpenseType.Null)
                {
                    query = query.Where(o => o.Type == type);
                }

                var result = await query
                    .OrderBy(o => o.Order)
                    .Select(x => new CategoryViewModel
                    {
                        Id = x.Id,
                        Type = x.Type,
                        Text = x.Text,
                        Icon = x.Icon,
                        Color = x.Color,
                        Order = x.Order,
                        Name = x.Name,
                        CreatedBy = x.CreatedBy,
                        CreatedDate = x.CreatedDate,
                        ModifiedBy = x.ModifiedBy,
                        ModifiedDate = x.ModifiedDate
                    })
                    .ToListAsync();

                return new OperationResultList<CategoryViewModel>(result);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy khoản thu chi dựa vào id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                        Currency = test.Currency,
                        CategoryId = test.CategoryId,
                        CategoryName = _context.ct_IncomeExpenseCategories
                                            .Where(cate => cate.Id == test.CategoryId)
                                            .Select(cate => cate.Name)
                                            .FirstOrDefault(),
                        CategoryIcon = _context.ct_IncomeExpenseCategories
                                            .Where(cate => cate.Id == test.CategoryId)
                                            .Select(cate => cate.Icon)
                                            .FirstOrDefault(),
                        CategoryColor = _context.ct_IncomeExpenseCategories
                                            .Where(cate => cate.Id == test.CategoryId)
                                            .Select(cate => cate.Color)
                                            .FirstOrDefault(),
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

        /// <summary>
        /// Lấy danh sách thu chi hiển thị điều khiển chính
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="search"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public async Task<OperationResult> Gets(IncomeExpenseType Type = 0, string search = "", string range = "month")
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                var now = DateTime.UtcNow;

                // Xác định khoảng thời gian
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

                // Lấy toàn bộ danh mục ra Dictionary (Id → Thông tin)
                var categoryDict = await _context.ct_IncomeExpenseCategories
                    .AsNoTracking()
                    .ToDictionaryAsync(c => c.Id, c => new
                    {
                        c.Name,
                        c.Color,
                        c.Icon
                    });

                // Truy vấn giao dịch
                var query = _context.ct_IncomeExpense
                    .AsNoTracking()
                    .Where(o => o.CreatedBy == currentUserId &&
                                o.Date >= startDate && o.Date <= endDate);

                if (Type != 0)
                    query = query.Where(o => o.Type == Type);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(o => o.Description.Contains(search.Trim()));

                var data = await query
                    .OrderByDescending(o => o.CreatedDate)
                    .ToListAsync();

                // Dựng danh sách kết quả
                var grouped = data
                    .GroupBy(o => o.Date.Date)
                    .Select(g => new IEGroupViewModel
                    {
                        Date = g.Key,
                        Items = g.Select(i =>
                        {
                            var categoryInfo = i.CategoryId != null && categoryDict.ContainsKey(i.CategoryId.Value)
                                ? categoryDict[i.CategoryId.Value]
                                : null;

                            return new IncomeExpenseViewModel
                            {
                                Id = i.Id,
                                Amount = i.Amount,
                                Date = i.Date,
                                Type = i.Type,
                                Currency = i.Currency,
                                Description = i.Description,
                                CategoryId = i.CategoryId,
                                CategoryName = categoryInfo?.Name ?? "Unknown",
                                CategoryColor = categoryInfo?.Color ?? "#000000",
                                CategoryIcon = categoryInfo?.Icon ?? "default-icon"
                            };
                        }).ToList()
                    }).ToList();

                return new OperationResultList<IEGroupViewModel>(grouped);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public async Task<OperationResult> Gets(int page, int pageSize, string searchValue, /*IncomeExpenseType type = IncomeExpenseType.Null, string range = "",*/ int? year = null, int? month = null, int? day = null)
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                var now = DateTime.UtcNow;

                // Câu truy vấn cơ bản
                var query = _context.ct_IncomeExpense
                    .Where(o => o.CreatedBy.Equals(currentUserId)).AsQueryable();

                // Apply date filtering based on provided parameters
                if (year.HasValue)
                {
                    query = query.Where(o => o.Date.Year == year.Value);

                    if (month.HasValue)
                    {
                        query = query.Where(o => o.Date.Month == month.Value);

                        if (day.HasValue)
                        {
                            query = query.Where(o => o.Date.Day == day.Value);
                        }
                    }
                }

                // Filter theo tìm kiếm
                if (!string.IsNullOrEmpty(searchValue)) query = query.Where(o => o.Description.Contains(searchValue.Trim()));

                // Lấy toàn bộ danh mục ra Dictionary (Id → Thông tin)
                var categoryDict = await _context.ct_IncomeExpenseCategories
                    .AsNoTracking()
                    .ToDictionaryAsync(c => c.Id, c => new
                    {
                        c.Name,
                        c.Color,
                        c.Icon
                    });

                // Lấy dữ liệu phân trang
                var data = await query
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Group theo ngày
                var grouped = data
                    .GroupBy(o => o.Date.Date)
                    .Select(g => new IEGroupViewModel
                    {
                        Date = g.Key,
                        Items = g.Select(i =>
                        {
                            var categoryInfo = i.CategoryId != null && categoryDict.ContainsKey(i.CategoryId.Value)
                                ? categoryDict[i.CategoryId.Value]
                                : null;

                            return new IncomeExpenseViewModel
                            {
                                Id = i.Id,
                                Amount = i.Amount,
                                Date = i.Date,
                                Type = i.Type,
                                Description = i.Description,
                                Currency = i.Currency,
                                CategoryId = i.CategoryId,
                                CategoryName = categoryInfo?.Name ?? "Unknown",
                                CategoryColor = categoryInfo?.Color ?? "#000000",
                                CategoryIcon = categoryInfo?.Icon ?? "default-icon"
                            };
                        }).ToList()
                    }).ToList();

                return new OperationResultList<IEGroupViewModel>(grouped);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// HIenet thị title số lượng ở màn hình chính
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public async Task<OperationResult> GetSummary(string? range = "month")
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                var now = DateTime.UtcNow;

                DateTime startDate, endDate;

                switch (range)
                {
                    case "today":
                        startDate = now.Date;
                        endDate = now.Date.AddDays(1).AddTicks(-1);
                        break;
                    case "week":
                        int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
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

                // Tính toán trực tiếp trên DB, không cần ToList()
                var summaryData = await _context.ct_IncomeExpense
                    .Where(o => o.CreatedBy == currentUserId &&
                                o.Date >= startDate &&
                                o.Date <= endDate)
                    .GroupBy(o => 1) // Gom tất cả vào 1 nhóm
                    .Select(g => new
                    {
                        TotalIncome = g.Where(o => o.Type == IncomeExpenseType.Income).Sum(o => (decimal?)o.Amount) ?? 0,
                        TotalExpense = g.Where(o => o.Type == IncomeExpenseType.Expense).Sum(o => (decimal?)o.Amount) ?? 0
                    })
                    .FirstOrDefaultAsync();

                var summary = new IncomeExpenseummaryViewModel
                {
                    TotalIncome = summaryData?.TotalIncome ?? 0,
                    TotalExpense = summaryData?.TotalExpense ?? 0,
                };

                return new OperationResult<IncomeExpenseummaryViewModel>(summary);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Hiển thị số lượng ở màn hình history
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<OperationResult> GetSummary(int? year, int? month, int? day, string searchValue)
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                // Query cơ bản
                var query = _context.ct_IncomeExpense
                    .Where(o => o.CreatedBy == currentUserId);

                // Filter theo ngày tháng năm
                if (year.HasValue)
                {
                    query = query.Where(o => o.Date.Year == year.Value);

                    if (month.HasValue)
                    {
                        query = query.Where(o => o.Date.Month == month.Value);

                        if (day.HasValue)
                        {
                            query = query.Where(o => o.Date.Day == day.Value);
                        }
                    }
                }

                // Filter theo chuỗi tìm kiếm
                if (!string.IsNullOrEmpty(searchValue))
                {
                    var keyword = searchValue.Trim();
                    query = query.Where(o => o.Description.Contains(keyword));
                }

                // Truy vấn tổng trực tiếp trên database
                var summaryData = await query
                    .GroupBy(o => 1) // Gom tất cả vào 1 nhóm
                    .Select(g => new
                    {
                        TotalIncome = g.Where(o => o.Type == IncomeExpenseType.Income).Sum(o => (decimal?)o.Amount) ?? 0,
                        TotalExpense = g.Where(o => o.Type == IncomeExpenseType.Expense).Sum(o => (decimal?)o.Amount) ?? 0
                    })
                    .FirstOrDefaultAsync();

                var summary = new IncomeExpenseummaryViewModel
                {
                    TotalIncome = summaryData?.TotalIncome ?? 0,
                    TotalExpense = summaryData?.TotalExpense ?? 0
                };

                return new OperationResult<IncomeExpenseummaryViewModel>(summary);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResult> InUsed(Guid? id = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cập nhật khoản thu chi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult> Update(Guid id, IncomeExpenseCreateUpdateModel model)
        {
            try
            {
                var entity = await _context.ct_IncomeExpense.FindAsync(id);
                if (entity == null)
                {
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu.");
                }

                // Cập nhật các thuộc tính từ model
                entity.Amount = model.Amount;
                entity.Date = model.Date;
                entity.Description = model.Description;
                entity.CategoryId = model.CategoryId;
                entity.Type = model.Type;
                entity.Currency = model.Currency;
                entity.ModifiedDate = DateTime.UtcNow;
                entity.ModifiedBy = GetExtensions.GetUserId(_httpContextAccessor);

                var status = await _context.SaveChangesAsync();
                if (status > 0)
                {
                    return new OperationResult<bool>(HttpStatusCode.OK, true, "Cập nhật thành công.");
                }

                return new OperationResult<bool>(HttpStatusCode.NotFound, false, "Cập nhật thất bại.");
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}