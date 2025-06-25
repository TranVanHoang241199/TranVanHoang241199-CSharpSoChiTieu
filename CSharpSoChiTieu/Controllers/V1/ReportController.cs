using API_HotelManagement.common;
using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportHandler _reportHandler;
        private readonly ICurrencyHandler _currencyHandler;
        private readonly ISettingHandler _settingHandler;

        public ReportController(IReportHandler reportHandler, ICurrencyHandler currencyHandler, ISettingHandler settingHandler)
        {
            _reportHandler = reportHandler;
            _currencyHandler = currencyHandler;
            _settingHandler = settingHandler;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _reportHandler.GetReportData(new ReportFilterModel());

            // Lấy danh sách tiền tệ
            var currencyResult = await _currencyHandler.GetAll();
            var currencies = (currencyResult as OperationResultList<CurrencyViewModel>)?.Data ?? new List<CurrencyViewModel>();
            ViewBag.Currencies = currencies;

            // Lấy đơn vị tiền tệ đang lưu trong user setting
            var userSetting = _settingHandler.GetUserSettings();
            ViewBag.SelectedCurrency = userSetting?.Currency ?? "VND";

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> FilterReport([FromBody] ReportFilterModel filter)
        {
            var result = await _reportHandler.GetReportData(filter);
            return Json(result);
        }
    }
}
