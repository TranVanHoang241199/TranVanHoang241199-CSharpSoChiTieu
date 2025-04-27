using AutoMapper;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public class IncomeExpenseViewModel : EntityBase
    {
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public TransactionType Type { get; set; } // Enum: Income/Expense
    }

    public class IncomeExpenseSummaryViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;
    }

    public enum TransactionType
    {
        Income = 1,
        Expense = 2
    }

    public class IncomeExpenseCreateUpdateModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public TransactionType Type { get; set; } // Enum: Income/Expense
    }


    public class IEGroupViewModel
    {
        public DateTime Date { get; set; }                    // nhóm theo ngày
        public List<IncomeExpenseViewModel>? Items { get; set; }  // danh sách các khoản thu nhập
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
