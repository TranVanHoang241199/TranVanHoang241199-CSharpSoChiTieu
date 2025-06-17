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

        [Display(Name = "Chế độ tối")]
        public bool DarkMode { get; set; }

        [Display(Name = "Thông báo")]
        public bool Notifications { get; set; }

        [Display(Name = "Ngôn ngữ")]
        [Required(ErrorMessage = "Vui lòng chọn ngôn ngữ")]
        public string Language { get; set; }
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
        public string Avatar { get; set; }
        public bool DarkMode { get; set; }
        public bool Notifications { get; set; }
        public string Language { get; set; }
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
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));
                //.ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                //.ForMember(dest => dest.DarkMode, opt => opt.MapFrom(src => src.DarkMode))
                //.ForMember(dest => dest.Notifications, opt => opt.MapFrom(src => src.Notifications))
                //.ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
        }
    }
}
