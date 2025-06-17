using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

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
            if (period == "week")
            {
                // Lấy năm và tuần hiện tại
                var now = DateTime.Now;
                var weekNumber = GetIso8601WeekOfYear(now);
                period = $"{now.Year}-W{weekNumber}";
            }
            var result = await _reportHandler.GetChartData(period);
            return Json(result);
        }

        private int GetIso8601WeekOfYear(DateTime time)
        {
            // Lấy tuần theo chuẩn ISO 8601
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
