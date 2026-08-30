using API_HotelManagement.common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IFeedbackHandler
    {
        Task<OperationResult> CreateFeedbackAsync(FeedbackCreateModel model);
        Task<OperationResult> GetUserFeedbacksAsync();
        Task<OperationResult> DeleteFeedbackAsync(Guid id);
    }
}
