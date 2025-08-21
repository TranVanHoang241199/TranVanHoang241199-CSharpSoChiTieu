using API_HotelManagement.common;
using CSharpSoChiTieu.API.Models;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CSharpSoChiTieu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncomeExpenseController : ControllerBase
    {
        private readonly IIncomeExpenseHandler _incomeExpenseHandler;
        private readonly ICurrencyHandler _currencyHandler;
        private readonly ISettingHandler _settingHandler;
        private readonly ILogger<IncomeExpenseController> _logger;

        public IncomeExpenseController(
            IIncomeExpenseHandler incomeExpenseHandler,
            ICurrencyHandler currencyHandler,
            ISettingHandler settingHandler,
            ILogger<IncomeExpenseController> logger)
        {
            _incomeExpenseHandler = incomeExpenseHandler;
            _currencyHandler = currencyHandler;
            _settingHandler = settingHandler;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách thu chi với phân trang
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetIncomeExpenses([FromQuery] PaginationRequest request)
        {
            try
            {
                var result = await _incomeExpenseHandler.Gets(
                    request.Page,
                    request.PageSize,
                    request.SearchValue ?? "");

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Lỗi khi lấy danh sách thu chi"
                    });
                }

                var data = (result as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();
                var countResult = await _incomeExpenseHandler.Count(request.SearchValue ?? "");
                var totalCount = (countResult as OperationResult<int>)?.Data ?? 0;

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                return Ok(new PaginatedResponse<IEGroupViewModel>
                {
                    Success = true,
                    Message = "Lấy danh sách thu chi thành công",
                    Data = data,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    CurrentPage = request.Page,
                    HasPreviousPage = request.Page > 1,
                    HasNextPage = request.Page < totalPages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thu chi");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách thu chi"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách thu chi theo filter
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetIncomeExpensesByFilter([FromBody] IncomeExpenseFilterRequest request)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var currency = request.Currency ?? userSetting?.Currency ?? "VND";

                var result = await _incomeExpenseHandler.Gets(
                    0, 0, "",
                    request.Year,
                    request.Month,
                    request.Day,
                    currency);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Lỗi khi lấy danh sách thu chi"
                    });
                }

                var data = (result as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

                return Ok(new ApiResponse<List<IEGroupViewModel>>
                {
                    Success = true,
                    Message = "Lấy danh sách thu chi thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thu chi theo filter");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách thu chi"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách thu chi theo loại
        /// </summary>
        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetIncomeExpensesByType(int type, [FromQuery] string? search = "")
        {
            try
            {
                var result = await _incomeExpenseHandler.Gets((IncomeExpenseType)type, search ?? "");

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Lỗi khi lấy danh sách thu chi"
                    });
                }

                var data = (result as OperationResultList<IEGroupViewModel>)?.Data ?? new List<IEGroupViewModel>();

                return Ok(new ApiResponse<List<IEGroupViewModel>>
                {
                    Success = true,
                    Message = "Lấy danh sách thu chi thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thu chi theo loại");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách thu chi"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin thu chi theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncomeExpenseById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "ID không hợp lệ"
                    });
                }

                var result = await _incomeExpenseHandler.GetIncomeExpenseById(id);

                if (result.Status != HttpStatusCode.OK)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không tìm thấy khoản thu chi"
                    });
                }

                var data = (result as OperationResult<IncomeExpenseViewModel>)?.Data;

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy thông tin thu chi thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin thu chi");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin thu chi"
                });
            }
        }

        /// <summary>
        /// Tạo mới khoản thu chi
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateIncomeExpense([FromBody] CreateIncomeExpenseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ",
                        Data = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var model = new IncomeExpenseCreateUpdateModel
                {
                    Id = Guid.Empty,
                    Type = (IncomeExpenseType)request.Type,
                    Amount = request.Amount,
                    Date = request.Date,
                    CategoryId = request.CategoryId,
                    Description = request.Note,
                    Currency = request.Currency ?? "VND"
                };

                var result = await _incomeExpenseHandler.Create(model);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không thể tạo mới khoản thu chi"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Tạo khoản thu chi thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo khoản thu chi");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi tạo khoản thu chi"
                });
            }
        }

        /// <summary>
        /// Cập nhật khoản thu chi
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncomeExpense(Guid id, [FromBody] UpdateIncomeExpenseRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "ID không hợp lệ"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ",
                        Data = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var model = new IncomeExpenseCreateUpdateModel
                {
                    Id = id,
                    Type = (IncomeExpenseType)request.Type,
                    Amount = request.Amount,
                    Date = request.Date,
                    CategoryId = request.CategoryId,
                    Description = request.Note,
                    Currency = request.Currency ?? "VND"
                };

                var result = await _incomeExpenseHandler.Update(id, model);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không thể cập nhật khoản thu chi"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật khoản thu chi thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật khoản thu chi");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật khoản thu chi"
                });
            }
        }

        /// <summary>
        /// Xóa khoản thu chi
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncomeExpense(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "ID không hợp lệ"
                    });
                }

                var result = await _incomeExpenseHandler.Delete(id);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không thể xóa khoản thu chi"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Xóa khoản thu chi thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khoản thu chi");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xóa khoản thu chi"
                });
            }
        }

        /// <summary>
        /// Lấy tổng kết thu chi
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] string? range = "month", [FromQuery] string? currency = null)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var selectedCurrency = currency ?? userSetting?.Currency ?? "VND";

                var result = await _incomeExpenseHandler.GetSummary(range, selectedCurrency);

                if (result is OperationResult<IncomeExpenseummaryViewModel> summaryResult)
                {
                    var model = summaryResult.Data;
                    var symbol = await _currencyHandler.GetSymbolByCodeAsync(selectedCurrency);

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Lấy tổng kết thành công",
                        Data = new
                        {
                            TotalIncome = model.TotalIncome,
                            TotalExpense = model.TotalExpense,
                            Balance = model.Balance,
                            TotalIncomeFormatted = $"{model.TotalIncome:N0} {symbol}",
                            TotalExpenseFormatted = $"{model.TotalExpense:N0} {symbol}",
                            BalanceFormatted = $"{model.Balance:N0} {symbol}",
                            Currency = selectedCurrency,
                            Symbol = symbol
                        }
                    });
                }

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi khi lấy tổng kết"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng kết");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy tổng kết"
                });
            }
        }

        /// <summary>
        /// Lấy tổng kết thu chi theo filter
        /// </summary>
        [HttpPost("summary/filter")]
        public async Task<IActionResult> GetSummaryByFilter([FromBody] IncomeExpenseFilterRequest request)
        {
            try
            {
                var userSetting = _settingHandler.GetUserSettings();
                var currency = request.Currency ?? userSetting?.Currency ?? "VND";

                var result = await _incomeExpenseHandler.GetSummary(
                    request.Year,
                    request.Month,
                    request.Day,
                    request.Search ?? "",
                    currency);

                if (result is OperationResult<IncomeExpenseummaryViewModel> summaryResult)
                {
                    var model = summaryResult.Data;
                    var symbol = await _currencyHandler.GetSymbolByCodeAsync(currency);

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Lấy tổng kết thành công",
                        Data = new
                        {
                            TotalIncome = model.TotalIncome,
                            TotalExpense = model.TotalExpense,
                            Balance = model.Balance,
                            TotalIncomeFormatted = $"{model.TotalIncome:N0} {symbol}",
                            TotalExpenseFormatted = $"{model.TotalExpense:N0} {symbol}",
                            BalanceFormatted = $"{model.Balance:N0} {symbol}",
                            Currency = currency,
                            Symbol = symbol
                        }
                    });
                }

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi khi lấy tổng kết"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng kết theo filter");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy tổng kết"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách danh mục theo loại
        /// </summary>
        [HttpGet("categories/{type}")]
        public async Task<IActionResult> GetCategories(int type)
        {
            try
            {
                var result = await _incomeExpenseHandler.GetCategories((IncomeExpenseType)type);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Lỗi khi lấy danh sách danh mục"
                    });
                }

                var data = (result as OperationResultList<CategoryViewModel>)?.Data ?? new List<CategoryViewModel>();

                return Ok(new ApiResponse<List<CategoryViewModel>>
                {
                    Success = true,
                    Message = "Lấy danh sách danh mục thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách danh mục"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách tiền tệ
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
    }
}
