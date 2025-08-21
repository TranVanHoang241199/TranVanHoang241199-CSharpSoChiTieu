using System.ComponentModel.DataAnnotations;

namespace CSharpSoChiTieu.API.Models
{
    // Account Models
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdateProfileRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }

    // Income Expense Models
    public class CreateIncomeExpenseRequest
    {
        [Required]
        public int Type { get; set; } // 1: Income, 2: Expense

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public string? Note { get; set; }

        public string? Currency { get; set; } = "VND";
    }

    public class UpdateIncomeExpenseRequest : CreateIncomeExpenseRequest
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class IncomeExpenseFilterRequest
    {
        public int? Type { get; set; } // 1: Income, 2: Expense
        public string? Search { get; set; }
        public string? Range { get; set; } = "month"; // day, week, month, year
        public string? Currency { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? QuickDate { get; set; }
    }

    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchValue { get; set; }
    }

    // Category Models
    public class CreateCategoryRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Text { get; set; }

        [Required]
        public string Icon { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = string.Empty;

        public int Order { get; set; } = 0;

        [Required]
        public int Type { get; set; } // 1: Income, 2: Expense
    }

    public class UpdateCategoryRequest : CreateCategoryRequest
    {
        [Required]
        public Guid Id { get; set; }
    }

    // Report Models
    public class ReportFilterRequest
    {
        public string? Type { get; set; }
        public string? Currency { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Period { get; set; }

        //public string? Range { get; set; } = "month";
        //public int? Year { get; set; }
        //public int? Month { get; set; }
        //public int? Day { get; set; }
        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }
    }

    // Settings Models
    public class UpdateSettingsRequest
    {
        public string? Currency { get; set; }
        public bool? DarkMode { get; set; }
        public string? Language { get; set; }
        public string? TimeZone { get; set; }
    }

    // Common Response Models
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int? TotalCount { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class ApiResponse : ApiResponse<object>
    {
    }

    public class PaginatedResponse<T> : ApiResponse<List<T>>
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class TimeZoneInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Offset { get; set; }
    }
}