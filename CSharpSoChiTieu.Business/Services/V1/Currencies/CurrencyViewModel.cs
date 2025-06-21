using AutoMapper;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public class CurrencyViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }

    public class CurrencyAutoMapper : Profile
    {
        public CurrencyAutoMapper()
        {
            CreateMap<ct_Currency, CurrencyViewModel>();
        }
    }
}