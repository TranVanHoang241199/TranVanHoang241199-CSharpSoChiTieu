using System.Net;
using System;
using API_HotelManagement.common;
using Microsoft.EntityFrameworkCore;

namespace CSharpSoChiTieu.Business.Services
{
    public class CategoryHandler : ICategoryHandler
    {
        public Task<OperationResult> Add(CategoryInputModel data)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> Count(string searchValue = "")
        {
            try
            {

                return new OperationResult<int>(0);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public Task<OperationResult> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> Gets(int page, int pageSize, string searchValue)
        {
            try
            {
                var data = new List<CategoryViewModel>();


                // (Giả lập data nếu cần - ví dụ bạn muốn load dữ liệu từ DB thì code tiếp ở đây)
                // data = ...;

                return new OperationResultList<CategoryViewModel>(data);
            }
            catch (Exception ex)
            {
                return new OperationResultError(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public Task<OperationResult> InUsed(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Update(CategoryInputModel data)
        {
            throw new NotImplementedException();
        }
    }
}
