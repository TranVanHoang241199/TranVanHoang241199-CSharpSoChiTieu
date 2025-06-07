using CSharpSoChiTieu.common;

namespace CSharpSoChiTieu.Data.Data.Entitys
{
    public class ct_Emoji : EntityBase
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
        public IncomeExpenseType Type { get; set; }
    }
}
