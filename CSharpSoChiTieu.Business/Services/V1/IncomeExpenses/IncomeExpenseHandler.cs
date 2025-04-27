using System.Net;
using API_HotelManagement.common;
using AutoMapper;
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
                    Type = (int)model.Type,
                    CategoryId = model.CategoryId,

                    //---------
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetExtensions.GetUserId(_httpContextAccessor),
                    //ModifiedBy = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                };

                // Thêm đối tượng vào DbContext
                _context.ct_IncomeExpenses.Add(entity);

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
                var serviceToDelete = await _context.ct_IncomeExpenses.FindAsync(id);

                if (serviceToDelete == null)
                {
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi.");
                }

                _context.ct_IncomeExpenses.Remove(serviceToDelete);

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
                var result = await _context.ct_IncomeExpenses
                    .Where(test => test.Id == id)
                    .Select(test => new IncomeExpenseViewModel
                    {
                        Id = test.Id,
                        Type = (TransactionType)test.Type,
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



        public async Task<OperationResult> Gets(int? Type = null, string search = "")
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                var query = _context.ct_IncomeExpenses
                                    .Where(o => o.CreatedBy.Equals(currentUserId))
                                    .AsQueryable();

                if (Type != null)
                    query = query.Where(o => o.Type == Type);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(o => o.Description.Contains(search.Trim()));

                // Lấy dữ liệu
                var data = await query
                    .OrderByDescending(o => o.Date)
                    .ToListAsync();

                // Nhóm dữ liệu theo ngày
                var grouped = data
                    .GroupBy(o => o.Date.Date)
                    .Select(g => new IEGroupViewModel
                    {
                        Date = g.Key,
                        Items = g.Select(i => new IncomeExpenseViewModel
                        {
                            Amount = i.Amount,
                            Date = i.Date,
                            Type = (TransactionType)i.Type,
                            Description = i.Description
                        }).ToList()
                    }).ToList();

                return new OperationResultList<IEGroupViewModel>(grouped);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        public async Task<OperationResult> GetSummary(int month)
        {
            try
            {
                // Lấy ID của user hiện tại
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                // Lọc các khoản thu chi của user theo tháng
                var transactions = await _context.ct_IncomeExpenses
                    .Where(o => o.CreatedBy == currentUserId
                                && o.Date.Month == month
                                && o.Date.Year == DateTime.Now.Year)
                    .ToListAsync();

                // Tính tổng thu và chi
                var totalIncome = transactions.Where(o => o.Type == (int)TransactionType.Income).Sum(o => o.Amount);
                var totalExpense = transactions.Where(o => o.Type == (int)TransactionType.Expense).Sum(o => o.Amount);

                var summary = new IncomeExpenseSummaryViewModel
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                };

                return new OperationResult<IncomeExpenseSummaryViewModel>(summary);
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
                var incomeExpenseUpdate = await _context.ct_IncomeExpenses.FindAsync(id);

                if (incomeExpenseUpdate == null)
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");

                incomeExpenseUpdate.Amount = model.Amount;
                incomeExpenseUpdate.Type = (int)model.Type;
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