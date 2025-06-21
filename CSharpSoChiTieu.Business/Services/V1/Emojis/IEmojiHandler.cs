
using API_HotelManagement.common;
using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IEmojiHandler
    {
        Task<OperationResult> Gets(IncomeExpenseType type, string searchValue);
    }
}
