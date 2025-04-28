using System.ComponentModel.DataAnnotations.Schema;
using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_IncomeExpenseCategory")]
    public class ct_IncomeExpenseCategory : EntityBase
    {
        public string? Name { get; set; }         // tên hệ thống (food, transport, ...)
        public string? Text { get; set; }         // tên hiển thị (Ăn uống, Di chuyển, ...)
        public string? Icon { get; set; }         // icon material
        public string? Color { get; set; }        // success, primary, warning, ...
        public int Order { get; set; }

        public IncomeExpenseType Type { get; set; }

        public ICollection<ct_IncomeExpense>? ct_IncomeExpense { get; set; }

        // Các trường từ EntityBase đã bao gồm:
        // - Id (Guid) key
        // - CreatedDate (DateTime?)
        // - CreatedBy (Guid)
        // - ModifiedDate (DateTime?)
        // - ModifiedBy (Guid?)
    }
}
