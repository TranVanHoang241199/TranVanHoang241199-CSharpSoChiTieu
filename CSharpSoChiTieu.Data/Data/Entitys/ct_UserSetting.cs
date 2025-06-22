using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_UserSetting")]
    public class ct_UserSetting : EntityBase
    {
        /// <summary>
        /// ID của người dùng
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Đơn vị tiền tệ mặt định
        /// </summary>
        public string? Currency { get; set; } = "VND";

        /// <summary>
        /// Ngôn ngữ 
        /// </summary>
        public string? Language { get; set; } = "vi";

        /// <summary>
        /// Chủ đề màu sắc
        /// </summary>
        public string Theme { get; set; } = "light";

        /// <summary>
        /// Số lượng page hiển thị trên 1 trang
        /// </summary>
        public int ItemsPerPage { get; set; } = 10;

        /// <summary>
        /// Kích thước chữ
        /// </summary>
        public int FontSize { get; set; } = 14;

        /// <summary>
        /// Thông báo qua email
        /// </summary>
        public bool ReceiveEmailNotifications { get; set; } = true;

        /// <summary>
        /// Thông báo push
        /// </summary>
        public bool ReceivePushNotifications { get; set; } = true;

        /// <summary>
        /// Chế độ tối
        /// </summary>
        public bool DarkMode { get; set; } = false;

        /// <summary>
        /// Hiển thị số tiền theo định dạng
        /// </summary>
        public string CurrencyFormat { get; set; } = "N0";

        /// <summary>
        /// Múi giờ
        /// </summary>
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";

        /// <summary>
        /// Navigation property
        /// </summary>
        public virtual ct_User User { get; set; }
    }
}
