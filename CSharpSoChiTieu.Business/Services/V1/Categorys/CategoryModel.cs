using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;

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
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Icon là bắt buộc")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "Màu sắc là bắt buộc")]
        public string ColorClass { get; set; }

        public bool IsIncome { get; set; }

        public int Order { get; set; }
    }

    public class CategoryListViewModel
    {
        public List<CategoryViewModel> IncomeCategories { get; set; }
        public List<CategoryViewModel> ExpenseCategories { get; set; }
    }
}
