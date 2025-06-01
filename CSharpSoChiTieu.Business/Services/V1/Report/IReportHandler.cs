using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IReportHandler
    {
        Task<ReportViewModel> GetReportData();
        Task<ChartDataViewModel> GetChartData(string period);
    }
}