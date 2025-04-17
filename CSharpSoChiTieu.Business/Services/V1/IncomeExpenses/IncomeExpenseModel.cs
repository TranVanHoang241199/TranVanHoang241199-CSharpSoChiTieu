using AutoMapper;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public class IncomeExpenseViewModel : EntityBase
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
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
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public TransactionType Type { get; set; } // Enum: Income/Expense
    }


    public class IncomeItemViewModel
    {
        public string Type { get; set; }       // icon type: "payments", "military_tech"
        public string Label { get; set; }      // "Lương tháng 4", "Thưởng hiệu suất"
        public decimal Amount { get; set; }    // 10000000
        public DateTime Date { get; set; }     // 16/04/2025
    }

    public class IncomeGroupViewModel
    {
        public DateTime Date { get; set; }                    // nhóm theo ngày
        public List<IncomeItemViewModel> Items { get; set; }  // danh sách các khoản thu nhập
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
