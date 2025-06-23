using API_HotelManagement.common;
using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IIncomeExpenseHandler
    {
        Task<OperationResult> Gets(IncomeExpenseType type = 0, string search = "", string range = "month", string currency = null);
        Task<OperationResult> Gets(int page, int pageSize, string searchValue,/* IncomeExpenseType type = 0, string range = "month",*/ int? year = null, int? month = null, int? day = null, string currency = null);
        Task<OperationResult> GetCategories(IncomeExpenseType type = 0);
        Task<OperationResult> Count(string searchValue = "");
        Task<OperationResult> Count(string searchValue = "", int? year = null, int? month = null, int? day = null);
        Task<OperationResult> GetIncomeExpenseById(Guid id);
        Task<OperationResult> Create(IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Update(Guid id, IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Delete(Guid id);
        Task<OperationResult> InUsed(Guid? id = null);
        Task<OperationResult> GetSummary(string? range = "month", string currency = null);
        Task<OperationResult> GetSummary(int? year, int? month, int? day, string searchValue, string currency);
    }
}
