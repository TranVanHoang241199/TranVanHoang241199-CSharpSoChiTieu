using AutoMapper;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class IncomeExpenseViewModel : EntityBase
    {
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public IncomeExpenseType Type { get; set; } // Enum: Income/Expense

        //----------- Category ---------------
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryIcon { get; set; }
        public string CategoryColor { get; set; } = "#000000";

    }

    /// <summary>
    /// Phục vụ hiển thị view tiền thu - chi - còn lại 
    /// </summary>
    public class IncomeExpenseummaryViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;
    }

    public class IncomeExpenseSessionModel
    {
        public string FormType { get; set; } = "expense"; // "income" hoặc "expense"
        public string RangeType { get; set; } = "month"; // "today", "week", "month", "year"
    }


    public class IEGroupViewModel
    {
        public DateTime Date { get; set; }  // nhóm theo ngày
        public List<IncomeExpenseViewModel>? Items { get; set; }  // danh sách các khoản thu nhập
    }


    public class IncomeExpenseCreateUpdateModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public IncomeExpenseType Type { get; set; } // Enum: Income/Expense
    }

    public class IncomeExpenseAutoMapper : Profile
    {
        public IncomeExpenseAutoMapper()
        {
            CreateMap<ct_IncomeExpense, IncomeExpenseViewModel>(); // Auto map ht_User to UserViewModel
            CreateMap<IncomeExpenseCreateUpdateModel, ct_IncomeExpense>(); // Auto map ht_User to UserViewModel
        }
    }
}
