using System.Threading.Tasks;
using SGA.Domain.Entities;

namespace SGA.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
    }
}
