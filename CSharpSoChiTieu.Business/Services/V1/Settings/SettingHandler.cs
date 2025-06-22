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
                    .Include(u => u.ct_UserSettings)
                    .FirstOrDefault(u => u.Id == Guid.Parse(userId));

                if (user == null)
                {
                    _logger.LogWarning($"Không tìm thấy người dùng với ID: {userId}");
                    return null;
                }

                var userProfile = _mapper.Map<UserProfileModel>(user);

                // Nếu có settings, map settings vào profile
                if (user.ct_UserSettings != null)
                {
                    _mapper.Map(user.ct_UserSettings, userProfile);
                }

                return userProfile;
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
                var userSetting = _context.ct_UserSettings
                    .FirstOrDefault(s => s.UserId == model.UserId);

                if (userSetting == null)
                {
                    // Tạo mới settings nếu chưa có
                    userSetting = _mapper.Map<ct_UserSetting>(model);
                    userSetting.Id = Guid.NewGuid();
                    userSetting.CreatedDate = DateTime.UtcNow;
                    userSetting.CreatedBy = model.UserId;
                    _context.ct_UserSettings.Add(userSetting);
                }
                else
                {
                    // Cập nhật settings hiện có
                    _mapper.Map(model, userSetting);
                    userSetting.ModifiedDate = DateTime.UtcNow;
                    userSetting.ModifiedBy = model.UserId;
                    _context.ct_UserSettings.Update(userSetting);
                }

                var result = _context.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật cài đặt cho người dùng {model.UserId}");
                throw;
            }
        }

        public SettingViewModel GetUserSettings(Guid userId)
        {
            try
            {
                var userSetting = _context.ct_UserSettings
                    .FirstOrDefault(s => s.UserId == userId);

                if (userSetting == null)
                {
                    // Trả về settings mặc định nếu chưa có
                    return new SettingViewModel
                    {
                        UserId = userId,
                        Currency = "VND",
                        Language = "vi",
                        Theme = "light",
                        ItemsPerPage = 10,
                        FontSize = 14,
                        ReceiveEmailNotifications = true,
                        ReceivePushNotifications = true,
                        DarkMode = false,
                        CurrencyFormat = "N0",
                        TimeZone = "Asia/Ho_Chi_Minh"
                    };
                }

                return _mapper.Map<SettingViewModel>(userSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy cài đặt của người dùng {userId}");
                throw;
            }
        }

        public bool CreateDefaultSettings(Guid userId)
        {
            try
            {
                var existingSettings = _context.ct_UserSettings
                    .FirstOrDefault(s => s.UserId == userId);

                if (existingSettings != null)
                {
                    return true; // Đã có settings
                }

                var defaultSettings = new ct_UserSetting
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Currency = "VND",
                    Language = "vi",
                    Theme = "light",
                    ItemsPerPage = 10,
                    FontSize = 14,
                    ReceiveEmailNotifications = true,
                    ReceivePushNotifications = true,
                    DarkMode = false,
                    CurrencyFormat = "N0",
                    TimeZone = "Asia/Ho_Chi_Minh",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.ct_UserSettings.Add(defaultSettings);
                var result = _context.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tạo cài đặt mặc định cho người dùng {userId}");
                throw;
            }
        }
    }
}
