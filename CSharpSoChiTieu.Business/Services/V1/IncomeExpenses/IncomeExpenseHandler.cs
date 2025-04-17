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
                    Name = model.Name,
                    Date = DateTime.Now,
                    Amount = model.Amount,
                    Description = model.Description,
                    Status = (int)model.Type,
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
                        Name = test.Name,
                        Type = (TransactionType)test.Status,
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

        public async Task<OperationResult> Gets(Guid userId, int? status = null, string search = "")
        {
            try
            {
                // Lấy ID của người dùng hiện tại
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                // Truy vấn dữ liệu từ cơ sở dữ liệu sử dụng LINQ
                var query = _context.ct_IncomeExpenses.Where(o => o.CreatedBy.Equals(currentUserId)).AsQueryable();

                // Áp dụng bộ lọc nếu có
                if (status.HasValue)
                {
                    query = query.Where(o => o.Status == status);
                }

                // Áp dụng bộ lọc nếu có
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(o => o.Name.Contains(search.Trim()));
                }

                // Lấy tổng số lượng phần tử
                var totalItems = await query.CountAsync();

                // Phân trang và lấy dữ liệu cho trang hiện tại
                var data = await query
                    .OrderBy(o => o.Name)
                    .ToListAsync();

                var result = _mapper.Map<List<IncomeExpenseViewModel>>(data).ToList();

                return new OperationResultList<IncomeExpenseViewModel>(result);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public Task<OperationResult> GetSummary(Guid userId, int moth)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> Update(Guid id, IncomeExpenseCreateUpdateModel model)
        {
            try
            {
                var incomeExpenseUpdate = await _context.ct_IncomeExpenses.FindAsync(id);

                if (incomeExpenseUpdate == null)
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");

                incomeExpenseUpdate.Name = model.Name;
                incomeExpenseUpdate.Amount = model.Amount;
                incomeExpenseUpdate.Status = (int)model.Type;
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