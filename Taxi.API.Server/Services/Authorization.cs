using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Taxi.API.Server.Models;
using Taxi.Core.Models.Auth;

namespace Taxi.API.Server.Services
{
    public class Authorization
    {
        private PasswordHasher<User> hasher = new PasswordHasher<User>();

        private readonly JwtOptions _jwt;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly TaxiBaseContext _context;


        public Authorization(IOptions<JwtOptions> options, TaxiBaseContext context) {
            _jwt = options.Value;
            _context = context;
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
                Email = request.email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHashe = hasher.HashPassword(user, request.password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }


        public async Task<string?> LoginUserAsync(LoginRequest request)
        {

            var account = await _context.Users.FirstOrDefaultAsync(a => a.Login == request.login);

            if (account == null ||
                string.IsNullOrWhiteSpace(request.password))
                return null;

            var result = hasher.VerifyHashedPassword(
                account, account.PasswordHashe, request.password);

            if (result == PasswordVerificationResult.Failed)
                return null;


            return CreateToken(account);
        }



        private string CreateToken(User account)
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
                //expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: new SigningCredentials(
                    _signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
