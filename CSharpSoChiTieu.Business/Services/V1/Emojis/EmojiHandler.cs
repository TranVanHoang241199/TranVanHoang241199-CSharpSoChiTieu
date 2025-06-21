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


        /// <summary>
        /// Lấy danh sách icon hiển thị trong tạo danh mục
        /// </summary>
        /// <param name="type"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<OperationResult> Gets(IncomeExpenseType type, string searchValue)
        {
            try
            {
                // Nhưng ngay sau đó lại ghi đè:
                var query = _context.ct_Emojis.AsQueryable();

                // Filter theo Type nếu có
                if (type != 0) query = query.Where(o => o.Type == type);

                var data = await query
                    .OrderByDescending(x => x.Order)
                    .ProjectTo<EmojiViewModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new OperationResultList<EmojiViewModel>(data);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}
