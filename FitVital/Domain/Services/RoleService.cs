using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebAPI.DAL;

namespace FitVital.Domain.Services
{
    public class RoleService : IRoleService
    {
        private readonly DataBaseContext _context;

        public RoleService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<Role> CreateRoleAsync(Role newRole)
        {
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();
            return newRole;
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
