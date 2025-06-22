using AutoMapper;
using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CSharpSoChiTieu.Business.Services
{
    public class SettingViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Display(Name = "Đơn vị tiền tệ")]
        public string Currency { get; set; } = "VND";

        [Display(Name = "Ngôn ngữ")]
        public string Language { get; set; } = "vi";

        [Display(Name = "Chủ đề")]
        public string Theme { get; set; } = "light";

        [Display(Name = "Số lượng hiển thị trên trang")]
        [Range(5, 50, ErrorMessage = "Số lượng phải từ 5 đến 50")]
        public int ItemsPerPage { get; set; } = 10;

        [Display(Name = "Kích thước chữ")]
        [Range(12, 20, ErrorMessage = "Kích thước chữ phải từ 12 đến 20")]
        public int FontSize { get; set; } = 14;

        [Display(Name = "Thông báo email")]
        public bool ReceiveEmailNotifications { get; set; } = true;

        [Display(Name = "Thông báo push")]
        public bool ReceivePushNotifications { get; set; } = true;

        [Display(Name = "Chế độ tối")]
        public bool DarkMode { get; set; } = false;

        [Display(Name = "Định dạng tiền tệ")]
        public string CurrencyFormat { get; set; } = "N0";

        [Display(Name = "Múi giờ")]
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
    }

    public class ApplicationUser : EntityBase
    {
        public string FullName { get; set; }
        public string Image { get; set; }
        public bool DarkMode { get; set; }
        public bool Notifications { get; set; }
        public string Language { get; set; }
    }

    public class UserProfileModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public string Role { get; set; }

        // Settings
        public string Currency { get; set; } = "VND";
        public string Language { get; set; } = "vi";
        public string Theme { get; set; } = "light";
        public int ItemsPerPage { get; set; } = 10;
        public int FontSize { get; set; } = 14;
        public bool ReceiveEmailNotifications { get; set; } = true;
        public bool ReceivePushNotifications { get; set; } = true;
        public bool DarkMode { get; set; } = false;
        public string CurrencyFormat { get; set; } = "N0";
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
    }

    public class SettingAutoMapper : Profile
    {
        public SettingAutoMapper()
        {
            CreateMap<ct_User, UserProfileModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<ct_UserSetting, SettingViewModel>()
                .ReverseMap();

            CreateMap<ct_UserSetting, UserProfileModel>()
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.Theme, opt => opt.MapFrom(src => src.Theme))
                .ForMember(dest => dest.ItemsPerPage, opt => opt.MapFrom(src => src.ItemsPerPage))
                .ForMember(dest => dest.FontSize, opt => opt.MapFrom(src => src.FontSize))
                .ForMember(dest => dest.ReceiveEmailNotifications, opt => opt.MapFrom(src => src.ReceiveEmailNotifications))
                .ForMember(dest => dest.ReceivePushNotifications, opt => opt.MapFrom(src => src.ReceivePushNotifications))
                .ForMember(dest => dest.DarkMode, opt => opt.MapFrom(src => src.DarkMode))
                .ForMember(dest => dest.CurrencyFormat, opt => opt.MapFrom(src => src.CurrencyFormat))
                .ForMember(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZone));
        }
    }
}
