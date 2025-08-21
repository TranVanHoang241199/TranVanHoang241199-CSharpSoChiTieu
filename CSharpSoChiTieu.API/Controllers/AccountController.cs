using CSharpSoChiTieu.API.Models;
using CSharpSoChiTieu.API.Services;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpSoChiTieu.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountHandler _accountHandler;
        private readonly ICategoryHandler _categoryHandler;
        private readonly IJwtService _jwtService;
        private readonly CTDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountHandler accountHandler,
            ICategoryHandler categoryHandler,
            IJwtService jwtService,
            CTDbContext context,
            ILogger<AccountController> logger)
        {
            _accountHandler = accountHandler;
            _categoryHandler = categoryHandler;
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
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

                var loginModel = new LoginViewModel
                {
                    UserName = request.UserName,
                    Password = request.Password
                };

                var result = _accountHandler.Authenticate(loginModel);

                if (!result.Success)
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = result.ErrorMessage ?? "Đăng nhập thất bại"
                    });
                }

                // Lấy entity ct_User từ database để tạo JWT token
                var userEntity = _context.ct_Users.FirstOrDefault(u => u.Id == result.User.Id && !u.IsDeleted);
                if (userEntity == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                string token;
                try
                {
                    token = _jwtService.GenerateToken(userEntity);
                }
                catch (Exception tokenEx)
                {
                    _logger.LogError(tokenEx, "Lỗi khi tạo JWT token cho user: {UserId}", userEntity.Id);
                    return StatusCode(500, new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi tạo token xác thực"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Đăng nhập thành công",
                    Data = new
                    {
                        Token = token,
                        User = new
                        {
                            Id = result.User.Id,
                            UserName = result.User.UserName,
                            FullName = result.User.FullName,
                            Email = result.User.Email,
                            Role = result.User.Role,
                            Image = result.User.Image
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi đăng nhập"
                });
            }
        }

        /// <summary>
        /// Đăng ký tài khoản
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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

                var registerModel = new RegisterViewModel
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    Password = request.Password,
                    FullName = request.FullName,
                    Phone = request.Phone
                };

                var result = _accountHandler.Register(registerModel);

                if (!result.Success)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.ErrorMessage ?? "Đăng ký thất bại"
                    });
                }

                // Thêm loại mặc định
                await _categoryHandler.Adddefault(result.Id);

                // Tạo token và đăng nhập tự động
                var userEntity = _context.ct_Users.FirstOrDefault(u => u.Id == result.Id && !u.IsDeleted);
                if (userEntity != null)
                {
                    string token;
                    try
                    {
                        token = _jwtService.GenerateToken(userEntity);
                    }
                    catch (Exception tokenEx)
                    {
                        _logger.LogError(tokenEx, "Lỗi khi tạo JWT token cho user: {UserId}", userEntity.Id);
                        return StatusCode(500, new ApiResponse
                        {
                            Success = false,
                            Message = "Lỗi khi tạo token xác thực"
                        });
                    }

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Đăng ký thành công",
                        Data = new
                        {
                            Token = token,
                            User = new
                            {
                                Id = userEntity.Id,
                                UserName = userEntity.UserName,
                                FullName = userEntity.FullName,
                                Email = userEntity.Email,
                                Role = userEntity.Role,
                                Image = userEntity.Image
                            }
                        }
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi đăng ký"
                });
            }
        }

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var user = _context.ct_Users.FirstOrDefault(u => u.Email == request.Email && !u.IsDeleted);
                if (user == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy tài khoản với email này"
                    });
                }

                // Tạo token reset
                user.PasswordResetToken = Guid.NewGuid().ToString();
                user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

                _context.SaveChanges();

                // TODO: Gửi email reset password
                // Có thể sử dụng GmailServiceHelper như trong Web

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Email reset password đã được gửi. Vui lòng kiểm tra hộp thư của bạn."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý quên mật khẩu");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xử lý quên mật khẩu"
                });
            }
        }

        /// <summary>
        /// Reset mật khẩu
        /// </summary>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var user = _context.ct_Users.FirstOrDefault(u =>
                    u.PasswordResetToken == request.Token &&
                    u.PasswordResetTokenExpiry > DateTime.UtcNow);

                if (user == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Token không hợp lệ hoặc đã hết hạn"
                    });
                }

                user.Password = PasswordHasher.Hash(request.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;

                _context.SaveChanges();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Đặt lại mật khẩu thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi reset mật khẩu");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi đặt lại mật khẩu"
                });
            }
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var user = _accountHandler.GetProfile(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy người dùng"
                    });
                }

                if (user.Password != PasswordHasher.Hash(request.CurrentPassword))
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Mật khẩu hiện tại không đúng"
                    });
                }

                _accountHandler.ChangePassword(userId, request.NewPassword);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Đổi mật khẩu thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi đổi mật khẩu"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin profile
        /// </summary>
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var profile = _accountHandler.GetProfile(userId);
                if (profile == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy thông tin profile thành công",
                    Data = profile
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy profile");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin profile"
                });
            }
        }

        /// <summary>
        /// Cập nhật profile
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var user = _context.ct_Users.FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy người dùng"
                    });
                }

                // Update user information
                user.FullName = request.FullName;
                user.Email = request.Email;
                user.Phone = request.Phone;

                _context.SaveChanges();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật profile thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật profile");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật profile"
                });
            }
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        [Authorize]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    });
                }

                var userEntity = _context.ct_Users.FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (userEntity == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy người dùng"
                    });
                }

                string newToken;
                try
                {
                    newToken = _jwtService.GenerateToken(userEntity);
                }
                catch (Exception tokenEx)
                {
                    _logger.LogError(tokenEx, "Lỗi khi tạo JWT token cho user: {UserId}", userEntity.Id);
                    return StatusCode(500, new ApiResponse
                    {
                        Success = false,
                        Message = "Lỗi khi tạo token xác thực"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Refresh token thành công",
                    Data = new { Token = newToken }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi refresh token");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi refresh token"
                });
            }
        }
    }
}