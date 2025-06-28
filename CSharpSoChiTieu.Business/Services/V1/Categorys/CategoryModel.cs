using AutoMapper;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using System.ComponentModel.DataAnnotations;

namespace CSharpSoChiTieu.Business.Services
{

    public class CategoryViewModel : EntityBase
    {
        public string? Name { get; set; }         // tên hệ thống (food, transport, ...)
        public string? Text { get; set; }         // tên hiển thị (Ăn uống, Di chuyển, ...)
        public string? Icon { get; set; }         // icon material
        public string? Color { get; set; }        // success, primary, warning, ...
        public int Order { get; set; }

        public IncomeExpenseType Type { get; set; }
    }

    public class CategoryInputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string? Name { get; set; }         // tên hệ thống (food, transport, ...)

        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        public string? Text { get; set; }         // tên hiển thị (Ăn uống, Di chuyển, ...)

        [Required(ErrorMessage = "Icon không được để trống")]
        public string? Icon { get; set; }         // icon material

        [Required(ErrorMessage = "Màu sắc không được để trống")]
        public string? Color { get; set; }        // success, primary, warning, ...

        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải là số dương")]
        public int Order { get; set; }

        [Required(ErrorMessage = "Loại khoản không được để trống")]
        public IncomeExpenseType Type { get; set; }
    }

    public class CategoryListViewModel
    {
        public List<CategoryViewModel>? IncomeCategories { get; set; }
        public List<CategoryViewModel>? ExpenseCategories { get; set; }
    }

    public class IncomeExpenseCategoryAutoMapper : Profile
    {
        public IncomeExpenseCategoryAutoMapper()
        {
            CreateMap<ct_IncomeExpenseCategory, CategoryViewModel>(); // Auto map ht_User to UserViewModel
            CreateMap<CategoryInputModel, ct_IncomeExpenseCategory>(); // Auto map ht_User to UserViewModel
        }
    }
}
