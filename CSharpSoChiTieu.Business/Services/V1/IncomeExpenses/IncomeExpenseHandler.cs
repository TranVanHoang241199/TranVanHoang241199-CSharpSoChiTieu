using AutoMapper;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;
using Microsoft.EntityFrameworkCore;

namespace CSharpSoChiTieu.Business.Services
{
    public class IncomeExpenseHandler : IIncomeExpenseHandler
    {
        private readonly CTDbContext _context;
        private readonly IMapper _mapper;

        public IncomeExpenseHandler(CTDbContext dbContext, IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }

        public Task<OperationResult> Create(IncomeExpenseCreateUpdateModel model)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Get(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Gets(Guid userId, int status)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalExpense(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalIncome(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Update(IncomeExpenseCreateUpdateModel model)
        {
            throw new NotImplementedException();
        }
    }
}