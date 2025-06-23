using API_HotelManagement.common;
using AutoMapper;
using CSharpSoChiTieu.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CSharpSoChiTieu.Business.Services
{
    public class CurrencyHandler : ICurrencyHandler
    {
        private readonly CTDbContext _context;
        private readonly IMapper _mapper;

        public CurrencyHandler(CTDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult> GetAll()
        {
            try
            {
                var currencies = await _context.ct_Currencies.OrderBy(c => c.Code).ToListAsync();
                var result = _mapper.Map<List<CurrencyViewModel>>(currencies);
                return new OperationResultList<CurrencyViewModel>(result);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<SettingViewModel> GetSetting(Guid userId)
        {
            var setting = await _context.ct_UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);
            return _mapper.Map<SettingViewModel>(setting);
        }

        public async Task<string> GetSymbolByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            var currency = await _context.ct_Currencies.FirstOrDefaultAsync(c => c.Code == code);
            return currency?.Symbol ?? string.Empty;
        }
    }
}