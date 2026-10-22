using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
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
                return BadRequest(new RegisterResponse { message = "Логин обязателен" });
            if (request.login.Length <= 3)
            {
                return BadRequest(new RegisterResponse { message = "Размер логина должно быть не менее 3!" });
            }

            if (string.IsNullOrWhiteSpace(request.password))
                return BadRequest(new RegisterResponse { message = "Пароль обязателен" });

            if (string.IsNullOrWhiteSpace(request.phone))
                return BadRequest(new RegisterResponse { message = "Phone обязателен" });

            if (string.IsNullOrWhiteSpace(request.lastName) || string.IsNullOrWhiteSpace(request.middleName) || string.IsNullOrWhiteSpace(request.firstName))
                return BadRequest(new RegisterResponse { message = "ФИО обязателен" });

            var (chekPas, message) =  CheckPassword(request.password);
            if (!chekPas)
            {
                return BadRequest(new RegisterResponse { message = message });
            }
            var user = await authorization.RegisterUserAsync(request);
            if (user == null)
            {
                return Conflict(new RegisterResponse { message = "Логин уже занят" });
            }
            string fio = $"{user.FirstName} {user.MiddleName} {user.LastName}";

            return Ok(new RegisterResponse {login = user.Login, fio= fio, message="Успешная регистрация!" });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(LoginRequest request)
        {
            string loginKey = $"LoginKey_{request.login}";

            Debug.WriteLine(JsonSerializer.Serialize(request));
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
            if (res != null)
            {

                var (token, UserDto) = res.Value;
            
                _cache.Remove(loginKey);
                return Ok(new LoginResponse {
                    IsSuccess = true,
                    access_token = token,
                    IsCaptchaRequired = false,
                    token_type = "Bearer",
                    User = UserDto
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

        [HttpGet("getProst")]
        [AllowAnonymous]
        public IActionResult GetProst()
        {
            //var isAuth = User.Identity?.IsAuthenticated ?? false;
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            //             ?? User.FindFirst("sub")?.Value;

            //Console.WriteLine($"Auth: {isAuth}, UserId: {userId ?? "<null>"}");

            //if (!isAuth)
            //    return Unauthorized();

            return Ok(new { hel = "helo" });
        }


        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequst response)
        {
            if (!string.Equals(response.PasswordNewConfirm, response.PasswordNew))
            {
                return BadRequest(
                    new ChangePasswordResponse
                    {
                        message = "Новый пароль и подтверждение пароля не совпадают.",
                        IsSuccess = false
                    }
                    );
            }
            if (string.Equals(response.PasswordCurrent, response.PasswordNew))
            {
                return BadRequest(
                    new ChangePasswordResponse
                    {
                        message = "Пароли одинаковые!",
                        IsSuccess = false
                    }
                    );
            }
            var (chekPas, message) = CheckPassword(response.PasswordNew);
            if (!chekPas)
            {
                return BadRequest(new RegisterResponse { message = message });
            }
            var userLogin  = User.Identity?.Name;
            message = await authorization.ChangePasswordAsync( userLogin,response);

            return Ok(
                new ChangePasswordResponse
                {
                    message = message,
                    IsSuccess= true
                }
                );
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



        private (bool, string) CheckPassword(string password)
        {
            string message = "";
            bool en = true;
            bool number = false;
            bool passLen = password.Length >= 6;

            if (passLen)
            {
                for (int i = 0; i < password.Length; i++)
                {
                    if (password[i] >= '0' && password[i] <= '9') number = true;
                    else if (!(
                        (password[i] >= 'A' && password[i] <= 'Z')
                        ||
                        (password[i] >= 'a' && password[i] <= 'z')
                        ))
                        en = false;
                }
            }

            if (!passLen)
                message = "Пароль слишком короткий, должно быть минимум 6 символов!";
            else if (!en)
            {
                message = "Используйте только английскую расскладку!";
            }
            else if (!number)
                message = "Добавьте хотябы одну цифру!";

            return (passLen && en && number, message);
        }
    }
}
