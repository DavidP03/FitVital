using FitVital.DAL.Entities;

namespace FitVital.Domain.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User newUser);
        Task<User> GetUserByIdAsync(int id);
        Task DeactivateUserAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetAllActiveUsersAsync();
    }
}
