using Microsoft.AspNetCore.Http;

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
    }
}
