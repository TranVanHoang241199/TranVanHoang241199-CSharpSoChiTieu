using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Data
{
    public class ct_Emoji : EntityBase
    {
        /// <summary>
        /// Tên icon
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Tên máy học
        /// </summary>
        public string? Value { get; set; }
        /// <summary>
        /// Icon
        /// </summary>
        public string? Icon { get; set; }
        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Danh mục loại thu hay là chi
        /// 0: chi
        /// 1: thu
        /// </summary>
        public IncomeExpenseType Type { get; set; }
    }
}
