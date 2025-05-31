using AutoMapper;
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
        public Guid Id { get; set; }
        public string? Name { get; set; }         // tên hệ thống (food, transport, ...)
        public string? Text { get; set; }         // tên hiển thị (Ăn uống, Di chuyển, ...)
        public string? Icon { get; set; }         // icon material
        public string? Color { get; set; }        // success, primary, warning, ...
        public int Order { get; set; }

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
