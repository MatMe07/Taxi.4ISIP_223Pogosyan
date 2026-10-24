using Microsoft.EntityFrameworkCore;
using Taxi.API.Server.Models;
using Taxi.Core.Models.DB;

namespace Taxi.API.Server.Services
{
    public class UsersService
    {
        private readonly TaxiBaseContext _context;

        public UsersService(TaxiBaseContext context)
        {
            _context = context;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking().Select(u => new UserDTO
                {
                    Id = u.Id,
                    Login = u.Login,
                    LastName = u.LastName,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    Phone = u.Phone,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.Name : null,
                    CreatedAt = u.CreatedAt,
                    isActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return false;

            user.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(UserDTO user)
        {
            var entity = await _context.Users.FindAsync(user.Id);
            if (entity == null) return false;

            entity.LastName = user.LastName;
            entity.FirstName = user.FirstName;
            entity.MiddleName = user.MiddleName;
            entity.Phone = user.Phone;
            entity.RoleId = user.RoleId;
            entity.IsActive = user.isActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            return await _context.Roles.AsNoTracking().Select(r => new RoleDTO { Id = r.Id, RoleName = r.Name }).ToListAsync();
        }
    }
}
