using API_HotelManagement.common;
using CSharpSoChiTieu.API.Models;
using CSharpSoChiTieu.Business.Services;
using CSharpSoChiTieu.common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CSharpSoChiTieu.API.Controllers
{
    [ApiController]
    [Route("api/category")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryHandler _categoryHandler;
        private readonly IEmojiHandler _emojiHandler;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryHandler categoryHandler,
            IEmojiHandler emojiHandler,
            ILogger<CategoryController> logger)
        {
            _categoryHandler = categoryHandler;
            _emojiHandler = emojiHandler;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách danh mục với phân trang
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] PaginationRequest request)
        {
            try
            {
                // Đếm số lượng
                var countResult = await _categoryHandler.Count(request.SearchValue ?? "");
                var totalCount = (countResult as OperationResult<int>)?.Data ?? 0;

                // Lấy danh sách
                var dataResult = await _categoryHandler.Gets(request.Page, request.PageSize, request.SearchValue ?? "");
                var data = (dataResult as OperationResultList<CategoryViewModel>)?.Data ?? new List<CategoryViewModel>();

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                return Ok(new PaginatedResponse<CategoryViewModel>
                {
                    Success = true,
                    Message = "Lấy danh sách danh mục thành công",
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách danh mục"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách danh mục theo loại
        /// </summary>
        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetCategoriesByType(int type)
        {
            try
            {
                var result = await _categoryHandler.Gets(0, 0, "");
                //var result = await _categoryHandler.Gets(0, 0, "", (IncomeExpenseType)type);

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
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục theo loại");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách danh mục"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin danh mục theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
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

                var result = await _categoryHandler.Get(id);

                if (result.Status != HttpStatusCode.OK)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không tìm thấy danh mục"
                    });
                }

                var data = (result as OperationResult<CategoryViewModel>)?.Data;

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Lấy thông tin danh mục thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin danh mục");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin danh mục"
                });
            }
        }

        /// <summary>
        /// Tạo mới danh mục
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
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

                var model = new CategoryInputModel
                {
                    Id = Guid.Empty,
                    Name = request.Name,
                    Text = request.Text,
                    Icon = request.Icon,
                    Color = request.Color,
                    Order = request.Order,
                    Type = (IncomeExpenseType)request.Type
                };

                var result = await _categoryHandler.Add(model);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không thể tạo mới danh mục"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Tạo danh mục thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi tạo danh mục"
                });
            }
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
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

                var model = new CategoryInputModel
                {
                    Id = id,
                    Name = request.Name,
                    Text = request.Text,
                    Icon = request.Icon,
                    Color = request.Color,
                    Order = request.Order,
                    Type = (IncomeExpenseType)request.Type
                };

                var result = await _categoryHandler.Update(model);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không thể cập nhật danh mục"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật danh mục thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật danh mục");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật danh mục"
                });
            }
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
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

                var result = await _categoryHandler.Delete(id);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Không thể xóa danh mục"
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Xóa danh mục thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa danh mục");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xóa danh mục"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách emoji theo loại
        /// </summary>
        [HttpGet("emojis/{type}")]
        public async Task<IActionResult> GetEmojisByType(int type)
        {
            try
            {
                var result = await _emojiHandler.Gets((IncomeExpenseType)type, "");

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Lỗi khi lấy danh sách emoji"
                    });
                }

                var data = (result as OperationResultList<EmojiViewModel>)?.Data ?? new List<EmojiViewModel>();

                return Ok(new ApiResponse<List<EmojiViewModel>>
                {
                    Success = true,
                    Message = "Lấy danh sách emoji thành công",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách emoji");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách emoji"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách màu sắc
        /// </summary>
        [HttpGet("colors")]
        public IActionResult GetColors()
        {
            try
            {
                var colors = new List<object>
                {
                    new { Text = "🔴 Đỏ", Value = "#FF3B30" },
                    new { Text = "🟢 Xanh lá", Value = "#34C759" },
                    new { Text = "🔵 Xanh dương", Value = "#007AFF" },
                    new { Text = "🟡 Vàng", Value = "#FFCC00" },
                    new { Text = "🟠 Cam", Value = "#FF9500" },
                    new { Text = "🟣 Tím", Value = "#AF52DE" },
                    new { Text = "⚫ Đen", Value = "#1C1C1E" },
                    new { Text = "⚪ Trắng", Value = "#FFFFFF" },
                    new { Text = "🟤 Nâu", Value = "#A0522D" },
                    new { Text = "🌸 Hồng pastel", Value = "#FFB6C1" },
                    new { Text = "🌿 Xanh bạc hà", Value = "#98FF98" },
                    new { Text = "🌊 Xanh biển", Value = "#00CED1" },
                    new { Text = "🧁 Hồng kem", Value = "#FADADD" },
                    new { Text = "🧊 Xanh băng", Value = "#D0F0FD" },
                    new { Text = "🌫️ Xám nhẹ", Value = "#E0E0E0" },
                    new { Text = "⛰️ Nâu đá", Value = "#8B8680" },
                    new { Text = "🌅 Cam hoàng hôn", Value = "#FFA07A" }
                };

                return Ok(new ApiResponse<List<object>>
                {
                    Success = true,
                    Message = "Lấy danh sách màu sắc thành công",
                    Data = colors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách màu sắc");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách màu sắc"
                });
            }
        }

        /// <summary>
        /// Kiểm tra danh mục có đang được sử dụng không
        /// </summary>
        [HttpGet("{id}/in-use")]
        public async Task<IActionResult> CheckCategoryInUse(Guid id)
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

                var result = await _categoryHandler.InUsed(id);

                if (result.Status != HttpStatusCode.OK)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message ?? "Lỗi khi kiểm tra danh mục"
                    });
                }

                var isInUse = (result as OperationResult<bool>)?.Data ?? false;

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Kiểm tra danh mục thành công",
                    Data = new { IsInUse = isInUse }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra danh mục");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi kiểm tra danh mục"
                });
            }
        }
    }
}