using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IAccountHandler
    {
        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        /// <param name="model">Bao gồm user và password</param>
        /// <returns></returns>
        AuthenticationResult Authenticate(LoginViewModel model);
        /// <summary>
        /// Tạo tài khoản mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        RegistrationResult Register(RegisterViewModel model);
        /// <summary>
        /// Lấy thông tin user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ProfileViewModel GetProfile(Guid userId);
    }
}
