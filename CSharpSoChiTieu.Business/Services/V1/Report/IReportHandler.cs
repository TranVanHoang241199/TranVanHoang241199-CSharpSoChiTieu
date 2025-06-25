
namespace CSharpSoChiTieu.Business.Services
{
    public interface IReportHandler
    {
        Task<ReportViewModel> GetReportData(ReportFilterModel filter);
    }
}