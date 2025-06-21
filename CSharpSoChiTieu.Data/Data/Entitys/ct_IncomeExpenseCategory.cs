using System.ComponentModel.DataAnnotations.Schema;
using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_IncomeExpenseCategory")]
    public class ct_IncomeExpenseCategory : EntityBase
    {
        /// <summary>
        /// tên hệ thống (food, transport, ...)
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// tên hiển thị (Ăn uống, Di chuyển, ...)
        /// </summary>
        public string? Text { get; set; }
        /// <summary>
        /// icon
        /// </summary>
        public string? Icon { get; set; }
        /// <summary>
        /// backgroud của icon
        /// </summary>
        public string? Color { get; set; }
        /// <summary>
        /// Vị trí hiển thị
        /// </summary>
        public int Order { get; set; }

        public IncomeExpenseType Type { get; set; }
    }
}
