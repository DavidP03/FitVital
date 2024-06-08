using FitVital.Controllers;
using FitVital.DAL.Pojos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitVital.Domain.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult> RegisterAsync(userRequest request);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<IEnumerable<UserResponse>> GetActiveUsersAsync();
        Task<UserResponse> GetUserByIdAsync(int id);
        Task<IEnumerable<UserResponse>> GetUsersByRoleAsync(string roleName);
        Task<OperationResult> UpdateUserAsync(int userId, userRequest request);
        Task<OperationResult> DisableUserAsync(int userId);
        Task<OperationResult> EnableUserAsync(int userId);
        Task<OperationResult> AssignRolesAsync(int userId, List<int> roleIds);
        Task<OperationResult> RemoveRoleFromUserAsync(int userId, int roleId);
    }
}