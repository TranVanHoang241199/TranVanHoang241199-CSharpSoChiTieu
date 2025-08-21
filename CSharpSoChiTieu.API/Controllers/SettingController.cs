using API_HotelManagement.common;
using CSharpSoChiTieu.API.Models;
using CSharpSoChiTieu.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SettingController : ControllerBase
    {
        private readonly ISettingHandler _settingHandler;
        private readonly ICurrencyHandler _currencyHandler;
        private readonly ILogger<SettingController> _logger;

        public SettingController(
            ISettingHandler settingHandler,
            ICurrencyHandler currencyHandler,
            ILogger<SettingController> logger)
        {
            _settingHandler = settingHandler;
            _currencyHandler = currencyHandler;
            _logger = logger;
        }

        /// <summary>
        /// Lấy cài đặt người dùng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserSettings()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var settings = _settingHandler.GetUserSettings();
                if (settings == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt người dùng"
                    });
                }

                // Lấy danh sách tiền tệ
                var currencyResult = await _currencyHandler.GetAll();
                var currencies = (currencyResult as OperationResultList<CurrencyViewModel>)?.Data ?? new List<CurrencyViewModel>();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy cài đặt thành công",
                    Data = new
                    {
                        Settings = settings,
                        Currencies = currencies
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy cài đặt người dùng");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy cài đặt"
                });
            }
        }

        /// <summary>
        /// Cập nhật cài đặt người dùng
        /// </summary>
        [HttpPut]
        public IActionResult UpdateSettings([FromBody] UpdateSettingsRequest request)
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

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var currentSettings = _settingHandler.GetUserSettings();
                if (currentSettings == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt người dùng"
                    });
                }

                // Cập nhật các giá trị được cung cấp
                var updatedSettings = new SettingViewModel
                {
                    UserId = currentSettings.UserId,
                    Currency = request.Currency ?? currentSettings.Currency,
                    DarkMode = request.DarkMode ?? currentSettings.DarkMode,
                    Language = request.Language ?? currentSettings.Language,
                    TimeZone = request.TimeZone ?? currentSettings.TimeZone,
                };

                var result = _settingHandler.UpdateSettings(updatedSettings);

                if (!result)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Cập nhật cài đặt thất bại"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật cài đặt thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật cài đặt");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật cài đặt"
                });
            }
        }

        /// <summary>
        /// Cập nhật tiền tệ
        /// </summary>
        [HttpPut("currency")]
        public IActionResult UpdateCurrency([FromBody] UpdateSettingsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Currency))
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Mã tiền tệ không hợp lệ"
                    });
                }

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var result = _settingHandler.UpdateCurrency(request.Currency);

                if (!result)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Cập nhật tiền tệ thất bại"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật tiền tệ thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tiền tệ");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật tiền tệ"
                });
            }
        }

        /// <summary>
        /// Chuyển đổi chế độ tối
        /// </summary>
        [HttpPost("toggle-dark-mode")]
        public async Task<IActionResult> ToggleDarkMode()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var result = await _settingHandler.ToggleDarkModeAsync();

                if (!result)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Cập nhật chế độ tối thất bại"
                    });
                }

                // Lấy trạng thái dark mode mới nhất
                var settings = _settingHandler.GetUserSettings();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Cập nhật chế độ tối thành công",
                    Data = new { DarkMode = settings?.DarkMode }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật chế độ tối");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật chế độ tối"
                });
            }
        }

        /// <summary>
        /// Cập nhật ngôn ngữ
        /// </summary>
        [HttpPut("language")]
        public IActionResult UpdateLanguage([FromBody] UpdateSettingsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Language))
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Ngôn ngữ không hợp lệ"
                    });
                }

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var currentSettings = _settingHandler.GetUserSettings();
                if (currentSettings == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt người dùng"
                    });
                }

                var updatedSettings = new SettingViewModel
                {
                    UserId = currentSettings.UserId,
                    Currency = currentSettings.Currency,
                    DarkMode = currentSettings.DarkMode,
                    Language = request.Language,
                    TimeZone = currentSettings.TimeZone
                };

                var result = _settingHandler.UpdateSettings(updatedSettings);

                if (!result)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Cập nhật ngôn ngữ thất bại"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật ngôn ngữ thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ngôn ngữ");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật ngôn ngữ"
                });
            }
        }

        /// <summary>
        /// Cập nhật múi giờ
        /// </summary>
        [HttpPut("timezone")]
        public IActionResult UpdateTimeZone([FromBody] UpdateSettingsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TimeZone))
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Múi giờ không hợp lệ"
                    });
                }

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var currentSettings = _settingHandler.GetUserSettings();
                if (currentSettings == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt người dùng"
                    });
                }

                var updatedSettings = new SettingViewModel
                {
                    UserId = currentSettings.UserId,
                    Currency = currentSettings.Currency,
                    DarkMode = currentSettings.DarkMode,
                    Language = currentSettings.Language,
                    TimeZone = request.TimeZone
                };

                var result = _settingHandler.UpdateSettings(updatedSettings);

                if (!result)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Cập nhật múi giờ thất bại"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật múi giờ thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật múi giờ");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật múi giờ"
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

        /// <summary>
        /// Lấy danh sách ngôn ngữ hỗ trợ
        /// </summary>
        [HttpGet("languages")]
        public IActionResult GetLanguages()
        {
            try
            {
                var languages = new List<object>
                {
                    new { Code = "vi", Name = "Tiếng Việt", NativeName = "Tiếng Việt" },
                    new { Code = "en", Name = "English", NativeName = "English" },
                    new { Code = "zh", Name = "中文", NativeName = "中文" },
                    new { Code = "ja", Name = "日本語", NativeName = "日本語" },
                    new { Code = "ko", Name = "한국어", NativeName = "한국어" }
                };

                return Ok(new ApiResponse<List<object>>
                {
                    Success = true,
                    Message = "Lấy danh sách ngôn ngữ thành công",
                    Data = languages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ngôn ngữ");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách ngôn ngữ"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách múi giờ
        /// </summary>
        [HttpGet("timezones")]
        public IActionResult GetTimeZones()
        {
            try
            {
                var timeZones = System.TimeZoneInfo.GetSystemTimeZones()
                    .Select(tz => new Models.TimeZoneInfo
                    {
                        Id = tz.Id,
                        Name = tz.DisplayName,
                        Offset = tz.BaseUtcOffset.TotalHours
                    })
                    .OrderBy(tz => tz.Offset)
                    .ToList();

                return Ok(new ApiResponse<List<Models.TimeZoneInfo>>
                {
                    Success = true,
                    Message = "Lấy danh sách múi giờ thành công",
                    Data = timeZones
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách múi giờ");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách múi giờ"
                });
            }
        }

        /// <summary>
        /// Reset cài đặt về mặc định
        /// </summary>
        [HttpPost("reset")]
        public IActionResult ResetSettings()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var currentSettings = _settingHandler.GetUserSettings();
                if (currentSettings == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy cài đặt người dùng"
                    });
                }

                var defaultSettings = new SettingViewModel
                {
                    UserId = currentSettings.UserId,
                    Currency = "VND",
                    DarkMode = false,
                    Language = "vi",
                    TimeZone = "Asia/Ho_Chi_Minh"
                };

                var result = _settingHandler.UpdateSettings(defaultSettings);

                if (!result)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Reset cài đặt thất bại"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Reset cài đặt thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi reset cài đặt");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi reset cài đặt"
                });
            }
        }
    }
}