using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public interface IAccountHandler
    {
        AuthenticationResult Authenticate(LoginViewModel model);
        RegistrationResult Register(RegisterViewModel model);
    }
}
