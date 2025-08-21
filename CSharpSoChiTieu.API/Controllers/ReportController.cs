/*using API_HotelManagement.common;
using CSharpSoChiTieu.API.Models;
using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportHandler _reportHandler;
        private readonly ICurrencyHandler _currencyHandler;
        private readonly ISettingHandler _settingHandler;
        private readonly ILogger<ReportController> _logger;

        public ReportController(
            IReportHandler reportHandler,
            ICurrencyHandler currencyHandler,
            ISettingHandler settingHandler,
            ILogger<ReportController> logger)
        {
            _reportHandler = reportHandler;
            _currencyHandler = currencyHandler;
            _settingHandler = settingHandler;
            _logger = logger;
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo mặc định
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetReportData()
        {
            try
            {
                var result = await _reportHandler.GetReportData(new ReportFilterModel());

                if (result == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi lấy dữ liệu báo cáo"
                    });
                }

                // Lấy danh sách tiền tệ
                var currencyResult = await _currencyHandler.GetAll();
                var currencies = (currencyResult as OperationResultList<CurrencyViewModel>)?.Data ?? new List<CurrencyViewModel>();

                // Lấy đơn vị tiền tệ đang lưu trong user setting
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = userSetting?.Currency ?? "VND";

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy dữ liệu báo cáo thành công",
                    Data = new
                    {
                        ReportData = result,
                        Currencies = currencies,
                        SelectedCurrency = selectedCurrency
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu báo cáo");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy dữ liệu báo cáo"
                });
            }
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo theo filter
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterReport([FromBody] ReportFilterRequest request)
        {
            try
            {
                var filter = new ReportFilterModel
                {
                    Type = request.Type,
                    Currency = request.Currency,
                    DateFrom = request.DateFrom,
                    DateTo = request.DateTo,
                    Period = request.Period,
                    //Range = request.Range ?? "month",
                    //Currency = request.Currency,
                    //Year = request.Year,
                    //Month = request.Month,
                    //Day = request.Day,
                    //FromDate = request.FromDate,
                    //ToDate = request.ToDate
                };

                var result = await _reportHandler.GetReportData(filter);

                if (result == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi lấy dữ liệu báo cáo"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy dữ liệu báo cáo thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu báo cáo theo filter");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy dữ liệu báo cáo"
                });
            }
        }

        /// <summary>
        /// Lấy báo cáo theo tháng
        /// </summary>
        [HttpGet("monthly/{year}/{month}")]
        public async Task<IActionResult> GetMonthlyReport(int year, int month, [FromQuery] string? currency = null)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = currency ?? userSetting?.Currency ?? "VND";

                var filter = new ReportFilterModel
                {
                    Year = year,
                    Month = month,
                    Currency = selectedCurrency
                };

                var result = await _reportHandler.GetReportData(filter);

                if (result == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi lấy báo cáo tháng"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy báo cáo tháng thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy báo cáo tháng");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy báo cáo tháng"
                });
            }
        }

        /// <summary>
        /// Lấy báo cáo theo năm
        /// </summary>
        [HttpGet("yearly/{year}")]
        public async Task<IActionResult> GetYearlyReport(int year, [FromQuery] string? currency = null)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = currency ?? userSetting?.Currency ?? "VND";

                var filter = new ReportFilterModel
                {
                    Year = year,
                    Currency = selectedCurrency
                };

                var result = await _reportHandler.GetReportData(filter);

                if (result == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi lấy báo cáo năm"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy báo cáo năm thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy báo cáo năm");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy báo cáo năm"
                });
            }
        }

        /// <summary>
        /// Lấy báo cáo theo khoảng thời gian
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetDateRangeReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string? currency = null)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = currency ?? userSetting?.Currency ?? "VND";

                var filter = new ReportFilterModel
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Currency = selectedCurrency
                };

                var result = await _reportHandler.GetReportData(filter);

                if (result == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi lấy báo cáo theo khoảng thời gian"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy báo cáo theo khoảng thời gian thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy báo cáo theo khoảng thời gian");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy báo cáo theo khoảng thời gian"
                });
            }
        }

        /// <summary>
        /// Lấy báo cáo nhanh theo range
        /// </summary>
        [HttpGet("quick/{range}")]
        public async Task<IActionResult> GetQuickReport(string range, [FromQuery] string? currency = null)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = currency ?? userSetting?.Currency ?? "VND";

                var filter = new ReportFilterModel
                {
                    Range = range,
                    Currency = selectedCurrency
                };

                var result = await _reportHandler.GetReportData(filter);

                if (result == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi lấy báo cáo nhanh"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy báo cáo nhanh thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy báo cáo nhanh");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy báo cáo nhanh"
                });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan
        /// </summary>
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromQuery] string? currency = null)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = currency ?? userSetting?.Currency ?? "VND";

                // Lấy báo cáo tháng hiện tại
                var currentMonth = DateTime.UtcNow;
                var monthFilter = new ReportFilterModel
                {
                    Year = currentMonth.Year,
                    Month = currentMonth.Month,
                    Currency = selectedCurrency
                };

                var monthResult = await _reportHandler.GetReportData(monthFilter);

                // Lấy báo cáo năm hiện tại
                var yearFilter = new ReportFilterModel
                {
                    Year = currentMonth.Year,
                    Currency = selectedCurrency
                };

                var yearResult = await _reportHandler.GetReportData(yearFilter);

                // Lấy báo cáo tháng trước
                var lastMonth = currentMonth.AddMonths(-1);
                var lastMonthFilter = new ReportFilterModel
                {
                    Year = lastMonth.Year,
                    Month = lastMonth.Month,
                    Currency = selectedCurrency
                };

                var lastMonthResult = await _reportHandler.GetReportData(lastMonthFilter);

                var overview = new
                {
                    CurrentMonth = monthResult,
                    CurrentYear = yearResult,
                    LastMonth = lastMonthResult,
                    Currency = selectedCurrency
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy thống kê tổng quan thành công",
                    Data = overview
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê tổng quan");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy thống kê tổng quan"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách tiền tệ cho báo cáo
        /// </summary>
        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            try
            {
                var result = await _currencyHandler.GetAll();
                var currencies = (result as OperationResultList<CurrencyViewModel>)?.Data ?? new List<CurrencyViewModel>();

                return Ok(new ApiResponse<List<CurrencyViewModel>>
                {
                    Success = true,
                    Message = "Lấy danh sách tiền tệ thành công",
                    Data = currencies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tiền tệ");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách tiền tệ"
                });
            }
        }

        /// <summary>
        /// Lấy cài đặt tiền tệ hiện tại
        /// </summary>
        [HttpGet("current-currency")]
        public IActionResult GetCurrentCurrency()
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = userSetting?.Currency ?? "VND";

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy tiền tệ hiện tại thành công",
                    Data = new { Currency = selectedCurrency }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tiền tệ hiện tại");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy tiền tệ hiện tại"
                });
            }
        }
    }
}

*/