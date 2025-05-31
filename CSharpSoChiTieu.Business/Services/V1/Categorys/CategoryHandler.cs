using System.Net;
using API_HotelManagement.common;
using AutoMapper;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace CSharpSoChiTieu.Business.Services
{
    public class CategoryHandler : ICategoryHandler
    {

        private readonly CTDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryHandler(CTDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<OperationResult> Add(CategoryInputModel data)
        {
            try
            {

                var entity = _mapper.Map<ct_IncomeExpenseCategory>(data);
                entity.CreatedDate = DateTime.UtcNow; // hoặc các trường bổ sung khác
                entity.CreatedBy = GetExtensions.GetUserId(_httpContextAccessor);

                _context.ct_IncomeExpenseCategories.Add(entity);
                await _context.SaveChangesAsync();

                return new OperationResult<CategoryViewModel>(_mapper.Map<CategoryViewModel>(entity));
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        public async Task<OperationResult> Count(string searchValue = "")
        {
            try
            {
                var query = _context.ct_IncomeExpenseCategories.AsQueryable();

                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                query = query.Where(o => o.CreatedBy.Equals(currentUserId));

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(x => x.Name.Contains(searchValue));
                }

                int count = await query.CountAsync();

                return new OperationResult<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        public async Task<OperationResult> Delete(Guid? id = null)
        {
            try
            {
                var entity = await _context.ct_IncomeExpenseCategories.FindAsync(id);
                if (entity == null)
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy loại hàng.");

                _context.ct_IncomeExpenseCategories.Remove(entity);
                await _context.SaveChangesAsync();

                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        public async Task<OperationResult> Get(Guid? id = null)
        {
            try
            {
                var entity = await _context.ct_IncomeExpenseCategories.FindAsync(id);
                if (entity == null)
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy loại hàng.");

                return new OperationResult<CategoryViewModel>(_mapper.Map<CategoryViewModel>(entity));
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        public async Task<OperationResult> Gets(int page, int pageSize, string searchValue = "")
        {
            try
            {
                // Nhưng ngay sau đó lại ghi đè:
                var query = _context.ct_IncomeExpenseCategories.AsQueryable();

                // Đầu tiên bạn tạo query:
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                 query = query.Where(o => o.CreatedBy.Equals(currentUserId));


                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(x => x.Name.Contains(searchValue) || x.Id.ToString().Equals(searchValue));
                }

                var data = await query
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new OperationResultList<CategoryViewModel>(data);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        public async Task<OperationResult> InUsed(Guid? id = null)
        {
            try
            {
                bool inUse = false;
                //bool inUse = await _context.ct_IncomeExpenseCategories.AnyAsync(x => x.CategoryId == id);
                return new OperationResult<bool>(inUse);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        public async Task<OperationResult> Update(CategoryInputModel data)
        {
            try
            {
                var entity = await _context.ct_IncomeExpenseCategories.FindAsync(data.Id);
                if (entity == null)
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy loại hàng.");

                // Update fields
                entity.Name = data.Name;
                entity.Icon = data.Icon;
                entity.Text = data.Text;
                entity.Order = data.Order;
                entity.Color = data.Color;
                entity.Type = data.Type;
                entity.ModifiedDate = DateTime.UtcNow;
                entity.ModifiedBy = GetExtensions.GetUserId(_httpContextAccessor);

                await _context.SaveChangesAsync();

                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

    }
}
