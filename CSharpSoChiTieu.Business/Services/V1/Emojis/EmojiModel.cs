using AutoMapper;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using CSharpSoChiTieu.Data.Data.Entitys;

namespace CSharpSoChiTieu.Business.Services
{
    public class EmojiViewModel : EntityBase
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
        public IncomeExpenseType Type { get; set; }
    }


    public class EmojiAutoMapper : Profile
    {
        public EmojiAutoMapper()
        {
            CreateMap<ct_Emoji, EmojiViewModel>(); // Auto map ht_User to UserViewModel
        }
    }
}
