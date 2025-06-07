using System.Security.Claims;
using AutoMapper;
using CSharpSoChiTieu.common;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.Business.Services
{
    public class AccountHandler : IAccountHandler
    {
        private readonly CTDbContext _context;
        private readonly IMapper _mapper;

        public AccountHandler(CTDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public AuthenticationResult Authenticate(LoginViewModel model)
        {
            try
            {
                // Kiểm tra null hoặc empty
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        ErrorMessage = "Tên đăng nhập và mật khẩu không được để trống"
                    };
                }

                var hashedPassword = PasswordHasher.Hash(model.Password);

                var entity = _context.ct_Users
                               .FirstOrDefault(u => u.UserName == model.UserName
                                                 && u.Password == hashedPassword
                                                 && u.IsDeleted == false);

                if (entity == null)
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng"
                    };
                }

                var user = _mapper.Map<UserViewModel>(entity);

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

                return new AuthenticationResult
                {
                    Success = true,
                    User = user,
                    Claims = claims
                };
            }
            catch (Exception ex)
            {
                // Log exception here
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Có lỗi xảy ra trong quá trình đăng nhập"
                };
            }
        }

        public RegistrationResult Register(RegisterViewModel model)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        ErrorMessage = "Tên đăng nhập và mật khẩu không được để trống"
                    };
                }

                // Kiểm tra tài khoản đã tồn tại
                if (_context.ct_Users.Any(u => u.UserName == model.UserName))
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        ErrorMessage = "Tên đăng nhập đã tồn tại"
                    };
                }

                var userId = Guid.NewGuid();

                var user = new ct_User
                {
                    Id = userId,
                    UserName = model.UserName,
                    Password = PasswordHasher.Hash(model.Password),
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Email = model.Email,
                    Role = string.IsNullOrEmpty(model.Role) ? "user" : model.Role.ToLower(),
                    IsDeleted = false,
                    PasswordUpdatedDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId // Tự nhận mình là người tạo
                };

                _context.ct_Users.Add(user);
                _context.SaveChanges();

                // Map to ViewModel
                var userViewModel = _mapper.Map<UserViewModel>(user);

                // Tạo claims cho user mới
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("IdUser", user.Id.ToString())
            };

                return new RegistrationResult
                {
                    Id = user.Id,
                    Success = true,
                    User = userViewModel,
                    Claims = claims
                };
            }
            catch (Exception ex)
            {
                // Log exception here
                return new RegistrationResult
                {
                    Success = false,
                    ErrorMessage = "Có lỗi xảy ra trong quá trình đăng ký"
                };
            }
        }

    }

}
