using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _usersService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _usersService.DeleteUserAsync(id);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPatch("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO user)
        {
            var result = await _usersService.UpdateUserAsync(user);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("getAllRoles")]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _usersService.GetAllRolesAsync();
            return Ok(roles);
        }
    }
}