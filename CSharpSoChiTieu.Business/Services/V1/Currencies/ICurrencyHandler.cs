using API_HotelManagement.common;
using System.Threading.Tasks;

namespace CSharpSoChiTieu.Business.Services
{
    public interface ICurrencyHandler
    {
        Task<OperationResult> GetAll();
    }
}