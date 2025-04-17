using Microsoft.AspNetCore.Http;

namespace API_HotelManagement.common
{
    public class GetExtensions
    {
        public static Guid GetUserId(IHttpContextAccessor httpContextAccessor)
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("UserId");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                // Trả về Guid.Empty hoặc giá trị mặc định khác
                return Guid.Empty;
            }

            return userId;
        }
    }
}
