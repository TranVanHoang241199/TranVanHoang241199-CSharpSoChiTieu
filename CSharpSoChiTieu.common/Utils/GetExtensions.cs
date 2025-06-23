using Microsoft.AspNetCore.Http;

namespace API_HotelManagement.common
{
    public static class GetExtensions
    {
        public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
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
