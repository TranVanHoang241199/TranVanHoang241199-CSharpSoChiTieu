using System.ComponentModel.DataAnnotations.Schema;
using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_IncomeExpense")]
    public class ct_IncomeExpense : EntityBase
    {
        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Đơn vị tiền tệ
        /// </summary>
        public string? Currency {  get; set; }

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
        public IncomeExpenseType Type { get; set; }

        /// <summary>
        /// chứa đối tượng category
        /// </summary>
        public Guid? CategoryId { get; set; }  // Thay đổi từ Guid sang Guid?
    }
}