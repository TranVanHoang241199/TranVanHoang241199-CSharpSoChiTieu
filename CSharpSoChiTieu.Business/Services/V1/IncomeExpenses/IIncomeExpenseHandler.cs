using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IIncomeExpenseHandler
    {
        Task<OperationResult> Gets(Guid userId, int status);
        Task<OperationResult> Get(Guid userId);
        Task<OperationResult> Create(IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Update(IncomeExpenseCreateUpdateModel model);
        Task<OperationResult> Delete(Guid id);


        Task<decimal> GetTotalIncome(Guid userId);
        Task<decimal> GetTotalExpense(Guid userId);
    }
}
