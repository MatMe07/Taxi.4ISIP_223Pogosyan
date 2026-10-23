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
            var user = await _context.Users
                .Include(u => u.ClientProfile)
                .Include(u => u.DriverProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return false;

            var driverProfileId = user.DriverProfile?.Id;
            var clientProfileId = user.ClientProfile?.Id;

            if (driverProfileId.HasValue)
            {
                var driverShiftIds = await _context.Shifts
                    .Where(s => s.DriverId == driverProfileId.Value)
                    .Select(s => s.Id)
                    .ToListAsync();

                if (driverShiftIds.Any())
                {
                    var waybills = await _context.Waybills
                        .Where(w => driverShiftIds.Contains(w.ShiftId))
                        .ToListAsync();
                    _context.Waybills.RemoveRange(waybills);
                }

                var poezdkaIds = await _context.Poezdkas
                    .Where(p => p.DriverId == driverProfileId.Value)
                    .Select(p => p.Id)
                    .ToListAsync();

                if (poezdkaIds.Any())
                {
                    var poezdkaRatings = await _context.PoezdkaRatings
                        .Where(r => poezdkaIds.Contains(r.PoezdkaId))
                        .ToListAsync();
                    _context.PoezdkaRatings.RemoveRange(poezdkaRatings);
                }

                var driverDocs = await _context.DriverDocuments
                    .Where(d => d.DriverId == driverProfileId.Value)
                    .ToListAsync();
                _context.DriverDocuments.RemoveRange(driverDocs);

                var payouts = await _context.DriverPayouts
                    .Where(p => p.DriverId == driverProfileId.Value)
                    .ToListAsync();
                _context.DriverPayouts.RemoveRange(payouts);

                var shifts = await _context.Shifts
                    .Where(s => s.DriverId == driverProfileId.Value)
                    .ToListAsync();
                _context.Shifts.RemoveRange(shifts);

                var poezdki = await _context.Poezdkas
                    .Where(p => p.DriverId == driverProfileId.Value)
                    .ToListAsync();
                _context.Poezdkas.RemoveRange(poezdki);
            }

            if (clientProfileId.HasValue)
            {
                var clientOrders = await _context.Orders
                    .Where(o => o.ClientId == clientProfileId.Value)
                    .Select(o => o.Id)
                    .ToListAsync();

                if (clientOrders.Any())
                {
                    var orderComplaints = await _context.Complaints
                        .Where(c => clientOrders.Contains(c.OrderId))
                        .ToListAsync();
                    _context.Complaints.RemoveRange(orderComplaints);

                    var orderPoezdkaIds = await _context.Poezdkas
                        .Where(p => clientOrders.Contains(p.OrderId))
                        .Select(p => p.Id)
                        .ToListAsync();

                    if (orderPoezdkaIds.Any())
                    {
                        var orderPoezdkaRatings = await _context.PoezdkaRatings
                            .Where(r => orderPoezdkaIds.Contains(r.PoezdkaId))
                            .ToListAsync();
                        _context.PoezdkaRatings.RemoveRange(orderPoezdkaRatings);

                        var orderPoezdki = await _context.Poezdkas
                            .Where(p => clientOrders.Contains(p.OrderId))
                            .ToListAsync();
                        _context.Poezdkas.RemoveRange(orderPoezdki);
                    }

                    var orders = await _context.Orders
                        .Where(o => clientOrders.Contains(o.Id))
                        .ToListAsync();
                    _context.Orders.RemoveRange(orders);
                }
            }

            var complaintsAsAuthor = await _context.Complaints
                .Where(c => c.AuthorUserId == id || c.TargetUserId == id || c.ResolvedBy == id)
                .ToListAsync();
            _context.Complaints.RemoveRange(complaintsAsAuthor);

            var ratingsBy = await _context.PoezdkaRatings
                .Where(r => r.RatedByUserId == id || r.RatedUserId == id)
                .ToListAsync();
            _context.PoezdkaRatings.RemoveRange(ratingsBy);

            var driverDocsVerified = await _context.DriverDocuments
                .Where(d => d.VerifiedByUserId == id)
                .ToListAsync();
            _context.DriverDocuments.RemoveRange(driverDocsVerified);

            var vehicleDocsVerified = await _context.VehicleDocuments
                .Where(d => d.VerifiedByUserId == id)
                .ToListAsync();
            _context.VehicleDocuments.RemoveRange(vehicleDocsVerified);

            var maintenances = await _context.Maintenances
                .Where(m => m.ManagerId == id)
                .ToListAsync();
            _context.Maintenances.RemoveRange(maintenances);

            var dispatcherOrders = await _context.Orders
                .Where(o => o.DispatcherId == id)
                .ToListAsync();

            if (dispatcherOrders.Any())
            {
                var dispatcherOrderIds = dispatcherOrders.Select(o => o.Id).ToList();

                var dComplaints = await _context.Complaints
                    .Where(c => dispatcherOrderIds.Contains(c.OrderId))
                    .ToListAsync();
                _context.Complaints.RemoveRange(dComplaints);

                var dPoezdkaIds = await _context.Poezdkas
                    .Where(p => dispatcherOrderIds.Contains(p.OrderId))
                    .Select(p => p.Id)
                    .ToListAsync();

                if (dPoezdkaIds.Any())
                {
                    var dRatings = await _context.PoezdkaRatings
                        .Where(r => dPoezdkaIds.Contains(r.PoezdkaId))
                        .ToListAsync();
                    _context.PoezdkaRatings.RemoveRange(dRatings);

                    var dPoezdki = await _context.Poezdkas
                        .Where(p => dispatcherOrderIds.Contains(p.OrderId))
                        .ToListAsync();
                    _context.Poezdkas.RemoveRange(dPoezdki);
                }

                _context.Orders.RemoveRange(dispatcherOrders);
            }

            if (user.ClientProfile != null)
                _context.ClientProfiles.Remove(user.ClientProfile);

            if (user.DriverProfile != null)
                _context.DriverProfiles.Remove(user.DriverProfile);

            _context.Users.Remove(user);

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
