using System.Threading.Tasks;
using SGA.Application.DTOs;

namespace SGA.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<bool> RegisterAsync(RegisterUserDto registerRequest);
    }
}
