using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.API.Server.Models;
using Taxi.API.Server.Services;
using Taxi.Core.Models.Auth;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly Authorization authorization;

        public AuthController(Authorization authorization)
        {
            this.authorization = authorization;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUserRegister(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.login))
                return BadRequest(new { message = "Логин обязателен" });

            if (string.IsNullOrWhiteSpace(request.password))
                return BadRequest(new { message = "Пароль обязателен" });

            if (string.IsNullOrWhiteSpace(request.email))
                return BadRequest(new { message = "Email обязателен" });

            if (string.IsNullOrWhiteSpace(request.lastName) || string.IsNullOrWhiteSpace(request.middleName) || string.IsNullOrWhiteSpace(request.firstName))
                return BadRequest(new { message = "ФИО обязателен" });

            var user = await authorization.RegisterUserAsync(request);
            if (user == null)
            {
                return Conflict(new { message = "Логин уже занят" });
            }
            string fio = $"{user.FirstName}{user.MiddleName}{user.LastName}";

            return Created($"/api/users/{user.Id}", new
            {
                user.Id,
                user.Login,
                fio,
                user.RoleId
            });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(LoginRequest request)
        {
           
            var token = await authorization.LoginUserAsync(request);

            if (token == null)
                return Challenge();

            return Ok(new
            {
                access_token = token,
                token_type = "Bearer"
            });
        }

    }
}
