using System.Net;
using API_HotelManagement.common;
using AutoMapper;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using CSharpSoChiTieu.common;

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

        /// <summary>
        /// Thêm danh mục mới
        /// </summary>
        /// <param name="data">Thông tin của danh mục</param>
        /// <returns></returns>
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

        /// <summary>
        /// Giữ liệu mặt đinh khi tạo tài khoản
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult> Adddefault(Guid userId)
        {
            try
            {
                var defaultCategories = new List<CategoryInputModel>
                {
                    // Income (Thu nhập)
                    new CategoryInputModel { Name = "Lương", Text = "Thu nhập từ lương", Type = IncomeExpenseType.Income, Icon = "💵", Color = "#4CAF50", Order = 1 },
                    new CategoryInputModel { Name = "Thưởng", Text = "Tiền thưởng", Type = IncomeExpenseType.Income, Icon = "🏆", Color = "#8BC34A", Order = 2 },
                    new CategoryInputModel { Name = "Thu nhập phụ", Text = "Thu nhập phụ", Type = IncomeExpenseType.Income, Icon = "💸", Color = "#CDDC39", Order = 3 },
                    new CategoryInputModel { Name = "Đầu tư", Text = "Lãi đầu tư", Type = IncomeExpenseType.Income, Icon = "📈", Color = "#009688", Order = 4 },
                    new CategoryInputModel { Name = "Bán hàng", Text = "Thu nhập bán hàng", Type = IncomeExpenseType.Income, Icon = "🛍️", Color = "#2196F3", Order = 5 },

                    // Expense (Chi tiêu)
                    new CategoryInputModel { Name = "Ăn uống", Text = "Chi cho ăn uống", Type = IncomeExpenseType.Expense, Icon = "🍽️", Color = "#FF5722", Order = 1 },
                    new CategoryInputModel { Name = "Đi lại", Text = "Chi phí di chuyển", Type = IncomeExpenseType.Expense, Icon = "🚌", Color = "#795548", Order = 2 },
                    new CategoryInputModel { Name = "Mua sắm", Text = "Chi phí mua đồ", Type = IncomeExpenseType.Expense, Icon = "🛒", Color = "#9C27B0", Order = 3 },
                    new CategoryInputModel { Name = "Hóa đơn", Text = "Điện nước, Internet...", Type = IncomeExpenseType.Expense, Icon = "🧾", Color = "#FFC107", Order = 4 },
                    new CategoryInputModel { Name = "Giải trí", Text = "Xem phim, du lịch...", Type = IncomeExpenseType.Expense, Icon = "🎬", Color = "#E91E63", Order = 5 },
                };

                var entities = _mapper.Map<List<ct_IncomeExpenseCategory>>(defaultCategories);

                foreach (var item in entities)
                {
                    item.Id = Guid.NewGuid();
                    item.CreatedDate = DateTime.UtcNow;
                    item.CreatedBy = userId;
                }

                _context.ct_IncomeExpenseCategories.AddRange(entities);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<List<CategoryViewModel>>(entities);
                return new OperationResult<List<CategoryViewModel>>(result);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Đếm số lượng để hiển thị category search
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Xoá danh mục 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// lấy đối tượng danh mục dựa vào id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Lấy danh sách 
        /// </summary>
        /// <param name="page">số trang hiện tại</param>
        /// <param name="pageSize">Tổng số trang hiển thị</param>
        /// <param name="searchValue">tìm kiếm theo tên và Id</param>
        /// <returns></returns>
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

        /// <summary>
        /// Kiểm tra danh mục có tồn tại các lớp con không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Cập nhật và chỉnh sửa danh mục 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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