using API_HotelManagement.common;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace CSharpSoChiTieu.Business.Services
{
    public class FeedbackHandler : IFeedbackHandler
    {
        private readonly CTDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostEnvironment _env;

        public FeedbackHandler(CTDbContext context, IHttpContextAccessor httpContextAccessor, IHostEnvironment env)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        public async Task<OperationResult> CreateFeedbackAsync(FeedbackCreateModel model)
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                // Xử lý lưu các file ảnh đính kèm vào thư mục wwwroot/uploads/feedback
                var savedImagePaths = new List<string>();
                if (model.Attachments != null && model.Attachments.Count > 0)
                {
                    string uploadFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "feedback");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    foreach (var file in model.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            savedImagePaths.Add($"/uploads/feedback/{fileName}");
                        }
                    }
                }

                var entity = new ct_Feedback
                {
                    Id = Guid.NewGuid(),
                    Type = model.Type,
                    Priority = model.Priority,
                    Title = model.Title,
                    Description = model.Description,
                    ImgUrl = savedImagePaths.Count > 0 ? string.Join(",", savedImagePaths) : null,
                    IsResolved = false,
                    CreatedBy = currentUserId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Set<ct_Feedback>().Add(entity);
                await _context.SaveChangesAsync();

                // Trả về OperationResult với Status OK
                return new OperationResult(HttpStatusCode.OK, "Gửi ý kiến đóng góp thành công!");
            }
            catch (Exception ex)
            {
                // Trả về OperationResultError khi gặp ngoại lệ
                return new OperationResultError(HttpStatusCode.InternalServerError, $"Lỗi lưu dữ liệu: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetUserFeedbacksAsync()
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);

                // Lấy danh sách entity từ DB trước
                var feedbacks = await _context.Set<ct_Feedback>()
                    .Where(f => f.CreatedBy == currentUserId)
                    .OrderByDescending(f => f.CreatedDate)
                    .ToListAsync();

                // Mapping sang ViewModel
                var list = feedbacks.Select(f => new FeedbackViewModel
                {
                    Id = f.Id,
                    Type = f.Type,
                    Priority = f.Priority,
                    Title = f.Title,
                    Description = f.Description,
                    ImgUrl = f.ImgUrl,
                    IsResolved = f.IsResolved,
                    CreatedDate = f.CreatedDate,
                    CreatedBy = f.CreatedBy
                }).ToList();

                return new OperationResultList<FeedbackViewModel>(list);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<OperationResult> DeleteFeedbackAsync(Guid id)
        {
            try
            {
                var currentUserId = GetExtensions.GetUserId(_httpContextAccessor);
                var feedback = await _context.Set<ct_Feedback>()
                    .FirstOrDefaultAsync(f => f.Id == id && f.CreatedBy == currentUserId);

                if (feedback == null)
                {
                    return new OperationResultError(HttpStatusCode.NotFound, "Không tìm thấy phản hồi hoặc bạn không có quyền xóa!");
                }

                // Xóa các file ảnh đính kèm thực tế trong ổ đĩa
                if (!string.IsNullOrEmpty(feedback.ImgUrl))
                {
                    var imagePaths = feedback.ImgUrl.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var imgPath in imagePaths)
                    {
                        string fullPath = Path.Combine(_env.ContentRootPath, "wwwroot", imgPath.TrimStart('/'));
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                    }
                }

                _context.Set<ct_Feedback>().Remove(feedback);
                await _context.SaveChangesAsync();

                return new OperationResultDelete(id, feedback.Title);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}