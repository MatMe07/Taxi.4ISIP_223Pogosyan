using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Taxi.API.Server.Models;
using Taxi.Core.Models.Auth;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class Authorization : BaseService
    {
        private PasswordHasher<User> hasher = new PasswordHasher<User>();

        private readonly JwtOptions _jwt;
        private readonly SymmetricSecurityKey _signingKey;


        public Authorization(IOptions<JwtOptions> options, TaxiBaseContext context):base(context) {
            _jwt = options.Value;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        }

        public async Task<User?> RegisterUserAsync (RegisterRequest request)
        {

            var exists = await _context.Users.AnyAsync(u => u.Login == request.login);

            if (exists) return null;


            var user = new User
            {
                Login = request.login,
                RoleId = 1,
                LastName = request.lastName,
                FirstName = request.firstName,
                MiddleName = request.middleName,
                Phone = request.phone,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHashe = hasher.HashPassword(user, request.password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var clientProfile = new ClientProfile
            {   
                User = user,
                Rating = 5,
                CreatedAt = DateTime.Now
            };

            await _context.ClientProfiles.AddAsync(clientProfile);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string> ChangePasswordAsync(string loginUser,ChangePasswordRequst request)
        {

            var account = await _context.Users.FirstOrDefaultAsync(a => a.Login == loginUser);
            if (account == null)
                return "Нет такого логина!";

            var result = hasher.VerifyHashedPassword(
                account, account.PasswordHashe, request.PasswordCurrent);

            if (result == PasswordVerificationResult.Failed) return "Введенный текущий пароль неправильный";

            account.PasswordHashe = hasher.HashPassword(account, request.PasswordNew);

            await _context.SaveChangesAsync();
            return "Пароль успешно изменен!";
        }

        public async Task<(string, UserDTO)?> LoginUserAsync(LoginRequest request)
        {
            var account = await _context.Users.Include(u=>u.Role).FirstOrDefaultAsync(a => a.Login == request.login);

            if (account == null ||
                string.IsNullOrWhiteSpace(request.password))
                return null;

            var result = hasher.VerifyHashedPassword(
                account, account.PasswordHashe, request.password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            string token = CreateToken(account);
            var userDto = new UserDTO
            {
                Id = account.Id,
                Login = account.Login,
                LastName = account.LastName,
                FirstName = account.FirstName,
                MiddleName = account.MiddleName,
                Phone = account.Phone,
                isActive = account.IsActive,
                RoleId = account.RoleId,
                RoleName = account.Role?.Name ?? "",
                CreatedAt = account.CreatedAt
            };
            if (userDto.RoleName == "Клиент")
            {
                var clientProf = await _context.ClientProfiles.FirstOrDefaultAsync(a => a.UserId == account.Id);
                if (clientProf == null) return null;
                userDto.ClientProfile = new ClientProfileDTO
                {
                    Id= clientProf.Id,
                    DefaultPaymentMethodId = clientProf.DefaultPaymentMethodId ?? -1,
                    DefaultPaymentMethodName = clientProf?.DefaultPaymentMethod?.Name ?? "",
                    Rating = clientProf.Rating,
                    CreatedAt = clientProf.CreatedAt
                };
            }
            else if (userDto.RoleName == "Водитель")
            {
                var driverProf = await _context.DriverProfiles.FirstOrDefaultAsync(d=>d.UserId == account.Id);  
                if (driverProf == null) return null;
                userDto.DriverProfile = new DriverProfileDTO
                {
                    PassportSeries = driverProf.PassportSeries,
                    PassportNumber = driverProf.PassportNumber,
                    LicenseCategories = driverProf.LicenseCategories,
                    LicenseNumber = driverProf.LicenseNumber,
                    ExperienceYears = driverProf.ExperienceYears,
                    DateNaima = driverProf.DateNaima,
                    IsActive = driverProf.IsActive,
                    SalaryRate = driverProf.SalaryRate,
                    Rating = driverProf.Rating,
                };
            }
            return (token, userDto);
        }


        public async Task<UserDTO> GetUserAccAsync(int userId)
        {
            var account = await _context.Users.Include(u=>u.Role).FirstOrDefaultAsync(u=>u.Id == userId);
            var userDto = new UserDTO
            {
                Id = account.Id,
                Login = account.Login,
                LastName = account.LastName,
                FirstName = account.FirstName,
                MiddleName = account.MiddleName,
                Phone = account.Phone,
                isActive = account.IsActive,
                RoleId = account.RoleId,
                RoleName = account.Role?.Name ?? "",
                CreatedAt = account.CreatedAt
            };
            if (userDto.RoleName == "Клиент")
            {
                var clientProf = await _context.ClientProfiles.Include(c=>c.DefaultPaymentMethod).FirstOrDefaultAsync(a => a.UserId == userId);
                if (clientProf == null) return null;
                userDto.ClientProfile = new ClientProfileDTO
                {
                    Id = clientProf.Id,
                    DefaultPaymentMethodId = clientProf.DefaultPaymentMethodId ?? -1,
                    DefaultPaymentMethodName = clientProf?.DefaultPaymentMethod?.Name ?? "",
                    Rating = clientProf.Rating,
                    CreatedAt = clientProf.CreatedAt
                };
            }
            else if (userDto.RoleName == "Водитель")
            {
                var driverProf = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
                if (driverProf == null) return null;
                userDto.DriverProfile = new DriverProfileDTO
                {
                    PassportSeries = driverProf.PassportSeries,
                    PassportNumber = driverProf.PassportNumber,
                    LicenseCategories = driverProf.LicenseCategories,
                    LicenseNumber = driverProf.LicenseNumber,
                    ExperienceYears = driverProf.ExperienceYears,
                    DateNaima = driverProf.DateNaima,
                    IsActive = driverProf.IsActive,
                    SalaryRate = driverProf.SalaryRate,
                    Rating = driverProf.Rating,
                };
            }
            return userDto;
        }


        private string  CreateToken(User account)
        {
            var claims = new[]
            {
                new Claim("sub", account.Id.ToString()),
                new Claim("name", account.Login),
                new Claim("role", account.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                
                //expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(
                    _signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
