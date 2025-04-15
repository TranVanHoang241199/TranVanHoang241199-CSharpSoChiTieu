using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_IncomeExpense")]
    public class ct_IncomeExpense : EntityBase
    {

        /// <summary>
        /// Tên khoản chi
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Ngày chi
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Danh mục loại thu hay là chi
        /// 0: chi
        /// 1: thu
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid? CategoryId { get; set; }  // Thay đổi từ Guid sang Guid?
        public ct_IncomeExpenseCategory? Category { get; set; }

        // Các trường từ EntityBase đã bao gồm:
        // - Id (Guid) key
        // - CreatedDate (DateTime?)
        // - CreatedBy (Guid)
        // - ModifiedDate (DateTime?)
        // - ModifiedBy (Guid?)

    }

    
}