using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_IncomeExpenseCategory")]
    public class ct_IncomeExpenseCategory : EntityBase
    {

        /// <summary>
        /// Tên danh mục
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Biểu tượng
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Màu sắc
        /// </summary>
        public string Color { get; set; } = "#000000";

        public ICollection<ct_IncomeExpense>? ct_IncomeExpenses { get; set; }

        // Các trường từ EntityBase đã bao gồm:
        // - Id (Guid) key
        // - CreatedDate (DateTime?)
        // - CreatedBy (Guid)
        // - ModifiedDate (DateTime?)
        // - ModifiedBy (Guid?)
    }
}
