using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{

    public class UserViewModel : EntityBase
    {
        /// <summary>
        /// Email tài khoản dùng đăng nhập
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        public string? FullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Image { get; set; }
        /// <summary>
        /// Thời gian cập nhật mật khẩu
        /// </summary>
        public DateTime? PasswordUpdatedDate { get; set; }
        /// <summary>
        /// kiểm tra vô hiệu hoá
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Thời gian vô hiệu hoá
        /// </summary>
        public DateTime? DeletedDate { get; set; }
        /// <summary>
        /// quyền (use - admin)
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Token dùng để reset mật khẩu
        /// </summary>
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// Thời gian hết hạn của token reset mật khẩu
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
    

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is enter required")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Password is enter required")]
        public string Password { get; set; } = "";

    }

    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public UserViewModel User { get; set; }
        public List<Claim> Claims { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class RegistrationResult
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public UserViewModel User { get; set; }
        public List<Claim> Claims { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Role { get; set; } // Admin hoặc User
    }
    //public class RegisterViewModel
    //{
    //    [Required(ErrorMessage = "Name is required.")]
    //    public string? FullName { get; set; }

    //    [Required(ErrorMessage = "UserName is required.")]
    //    public string? UserName { get; set; }

    //    [Required(ErrorMessage = "Password is required.")]
    //    [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
    //    [DataType(DataType.Password)]
    //    [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
    //    public string? Password { get; set; }

    //    [Required(ErrorMessage = "Confirm Password is required.")]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm Password")]
    //    public string? ConfirmPassword { get; set; }
    //}

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserAutoMapper : Profile
    {
        public UserAutoMapper()
        {
            CreateMap<ct_User, UserViewModel>(); // Auto map ht_User to UserViewModel
            CreateMap<RegisterViewModel, ct_User>(); // Auto map ht_User to UserViewModel
        }
    }
}
