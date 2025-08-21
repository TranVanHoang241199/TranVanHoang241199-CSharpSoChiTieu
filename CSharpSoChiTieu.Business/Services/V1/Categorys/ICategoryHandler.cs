
using API_HotelManagement.common;

namespace CSharpSoChiTieu.Business.Services
{
    public interface ICategoryHandler
    {
        Task<OperationResult> Gets(int page, int pageSize, string searchValue);
        Task<OperationResult> Count(string searchValue = "");
        Task<OperationResult> Get(Guid? id = null);
        Task<OperationResult> Adddefault(Guid userId);
        Task<OperationResult> Add(CategoryInputModel data);
        Task<OperationResult> Update(CategoryInputModel data);
        Task<OperationResult> Delete(Guid? id = null);
        Task<OperationResult> InUsed(Guid? id = null);
    }
}