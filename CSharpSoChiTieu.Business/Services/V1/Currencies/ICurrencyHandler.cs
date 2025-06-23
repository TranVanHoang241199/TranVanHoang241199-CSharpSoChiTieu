using API_HotelManagement.common;

namespace CSharpSoChiTieu.Business.Services
{
    public interface ICurrencyHandler
    {
        Task<OperationResult> GetAll();
        Task<SettingViewModel> GetSetting(Guid userId);
        Task<string> GetSymbolByCodeAsync(string code);
    }
}