using API_HotelManagement.common;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IIncomeExpenseHandler
    {
        Task<OperationResult> Gets(Guid userId, int? status = null, string search = "");
        Task<OperationResult> GetIncomeExpenseById(Guid id);
        Task<OperationResult> Create(IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Update(Guid id, IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Delete(Guid id);

        Task<OperationResult> GetSummary(Guid userId, int moth);


    }
}
