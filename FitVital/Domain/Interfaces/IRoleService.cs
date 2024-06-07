using FitVital.DAL.Entities;

namespace FitVital.Domain.Interfaces
{
    public interface IRoleService
    {
        Task<Role> CreateRoleAsync(Role newRole);
        Task<Role> GetRoleByIdAsync(int id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
