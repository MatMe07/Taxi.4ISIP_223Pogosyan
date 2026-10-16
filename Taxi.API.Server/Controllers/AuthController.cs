using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
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
        private readonly CaptchaService captchaService;
        private readonly IMemoryCache _cache;

        public AuthController(Authorization authorization, IMemoryCache cache, CaptchaService captcha)
        {
            this.authorization = authorization;
            this._cache = cache;
            this.captchaService = captcha;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUserRegister(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.login))
                return BadRequest(new { message = "Логин обязателен" });

            if (string.IsNullOrWhiteSpace(request.password))
                return BadRequest(new { message = "Пароль обязателен" });

            if (string.IsNullOrWhiteSpace(request.phone))
                return BadRequest(new { message = "Phone обязателен" });

            if (string.IsNullOrWhiteSpace(request.lastName) || string.IsNullOrWhiteSpace(request.middleName) || string.IsNullOrWhiteSpace(request.firstName))
                return BadRequest(new { message = "ФИО обязателен" });

            var user = await authorization.RegisterUserAsync(request);
            if (user == null)
            {
                return Conflict(new { message = "Логин уже занят" });
            }
            string fio = $"{user.FirstName} {user.MiddleName} {user.LastName}";

            return Ok(new RegisterResponse(user.Login, fio));
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(LoginRequest request)
        {
            string loginKey = $"LoginKey_{request.login}";
            _cache.TryGetValue(loginKey, out int failedAttmpts);

            bool captchaRequired = failedAttmpts >= 1;
            if (captchaRequired)
            {
                if (string.IsNullOrWhiteSpace(request.login)
                    || string.IsNullOrWhiteSpace(request.captchaText)
                    || !captchaService.CapthcaCheck(request.captchaId, request.captchaText))
                {
                    PLUSFailed(loginKey, ref  failedAttmpts);
                    
                    var (guid, img) = GenerateCaptcha(request.login);

                    return Ok(new LoginResponse
                    {
                        IsSuccess = false,
                        IsCaptchaRequired = true,
                        CaptchaId = guid,
                        message = "Неверная капча",
                        CaptchaImageBase64 = img

                    });
                }
            }

                
            var res = await authorization.LoginUserAsync(request);
            var (token, roleId) = res.Value;
            
            if (token != null)
            {
                _cache.Remove(loginKey);
                return Ok(new LoginResponse {
                    IsSuccess = true,
                    access_token = token,
                    IsCaptchaRequired = false,
                    token_type = "Bearer",
                    RoleId=roleId
                });

            }


            PLUSFailed(loginKey,ref failedAttmpts);

            bool needCaptcha = failedAttmpts >= 1;
            if (needCaptcha)
            {

                var (guid, img) = GenerateCaptcha(request.login);


                return Ok(new LoginResponse
                {
                    IsSuccess = false,
                    IsCaptchaRequired = true,
                    CaptchaId = guid,
                    message = "Много неудачных попыток. Введите капчу.",
                    CaptchaImageBase64 = img
                });

            }
            return Ok(new LoginResponse
            {
                IsSuccess = false,
                IsCaptchaRequired = false,
                message = "Неверный логин или пароль."
            });

        }

        private void PLUSFailed(string key, ref int current)
        {
            current++;
            var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            _cache.Set(key, current, cacheOptions);
        }

        private (Guid, string) GenerateCaptcha(string login)
        {
            var guid = Guid.NewGuid();
            var text = captchaService.CaptchaGen(login);
            var img = captchaService.GenImageBase64(text);
            _cache.Set(guid.ToString(), text, TimeSpan.FromMinutes(5));
            return (guid, img);
        }
    }
}
