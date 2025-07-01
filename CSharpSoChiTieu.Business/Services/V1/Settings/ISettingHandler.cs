using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CSharpSoChiTieu.Business.Services
{
    public interface ISettingHandler
    {
        /// <summary>
        /// Lấy thông tin cài đặt của người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Thông tin cài đặt của người dùng</returns>
        UserProfileModel GetUserProfile(string userId);

        /// <summary>
        /// Cập nhật cài đặt của người dùng
        /// </summary>
        /// <param name="model">Model chứa thông tin cài đặt cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        bool UpdateSettings(SettingViewModel model);

        /// <summary>
        /// Lấy cài đặt của người dùng
        /// </summary>
        /// <returns>Cài đặt của người dùng</returns>
        SettingViewModel GetUserSettings();

        /// <summary>
        /// Tạo cài đặt mặc định cho người dùng mới
        /// </summary>
        /// <returns>True nếu tạo thành công, False nếu thất bại</returns>
        bool CreateDefaultSettings();

        /// <summary>
        /// Cập nhật đơn vị tiền tệ
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        bool UpdateCurrency(string currencyCode);

        /// <summary>
        /// Chuyển đổi chế độ tối
        /// </summary>
        /// <returns></returns>
        Task<bool> ToggleDarkModeAsync();
    }
}
