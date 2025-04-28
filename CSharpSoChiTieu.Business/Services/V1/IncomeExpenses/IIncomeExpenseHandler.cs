using API_HotelManagement.common;
using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IIncomeExpenseHandler
    {
        Task<OperationResult> Gets(IncomeExpenseType type = 0, string search = "", string range = "month");
        Task<OperationResult> GetIncomeExpenseById(Guid id);
        Task<OperationResult> Create(IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Update(Guid id, IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Delete(Guid id);

        Task<OperationResult> GetSummary(string? range = "month");
    }
}
