using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportHandler _reportHandler;

        public ReportController(IReportHandler reportHandler)
        {
            _reportHandler = reportHandler;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _reportHandler.GetReportData();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData(string period)
        {
            var result = await _reportHandler.GetChartData(period);
            return Json(result);
        }
    }
}
