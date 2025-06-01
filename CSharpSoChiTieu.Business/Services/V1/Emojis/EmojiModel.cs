using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public class EmojiViewModel : EntityBase
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
        public IncomeExpenseType Type { get; set; }
    }
}
