using System.Net;
using API_HotelManagement.common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CSharpSoChiTieu.Business.Services
{
    public class EmojiHandler : IEmojiHandler
    {
        private readonly CTDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmojiHandler(CTDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OperationResult> CreateDefault(IncomeExpenseType type)
        {
            try
            {


                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<OperationResult> Gets(IncomeExpenseType type, string searchValue)
        {
            try
            {
                // Nhưng ngay sau đó lại ghi đè:
                var query = _context.ct_IncomeExpenseCategories.AsQueryable();

                // Đầu tiên bạn tạo query:
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                query = query.Where(o => o.CreatedBy.Equals(currentUserId));

                var data = await query
                    .OrderByDescending(x => x.Order)
                    .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new OperationResultList<CategoryViewModel>(data);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}
