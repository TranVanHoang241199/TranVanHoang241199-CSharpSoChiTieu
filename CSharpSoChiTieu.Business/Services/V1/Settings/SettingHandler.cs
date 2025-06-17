using AutoMapper;
using CSharpSoChiTieu.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CSharpSoChiTieu.Business.Services
{
    public class SettingHandler : ISettingHandler
    {
        private readonly CTDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SettingHandler> _logger;

        public SettingHandler(CTDbContext context, IMapper mapper, ILogger<SettingHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public UserProfileModel GetUserProfile(string userId)
        {
            try
            {
                var user = _context.ct_Users
                    .FirstOrDefault(u => u.Id == Guid.Parse(userId));

                if (user == null)
                {
                    _logger.LogWarning($"Không tìm thấy người dùng với ID: {userId}");
                    return null;
                }

                return _mapper.Map<UserProfileModel>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy thông tin cài đặt của người dùng {userId}");
                throw;
            }
        }

        public bool UpdateSettings(SettingViewModel model)
        {
            try
            {
                var user = _context.ct_Users
                    .FirstOrDefault(u => u.Id == model.UserId);

                if (user == null)
                {
                    _logger.LogWarning($"Không tìm thấy người dùng với ID: {model.UserId}");
                    return false;
                }

                // Cập nhật các cài đặt
                //user.DarkMode = model.DarkMode;
                //user.Notifications = model.Notifications;
                //user.Language = model.Language;

                _context.ct_Users.Update(user);
                var result = _context.SaveChanges();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật cài đặt cho người dùng {model.UserId}");
                throw;
            }
        }
    }
}
