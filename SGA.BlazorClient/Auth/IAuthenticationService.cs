using System;
using System.Threading.Tasks;
using SGA.Application.DTOs;

namespace SGA.BlazorApp.Client.Auth
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto?> Login(LoginRequestDto request);
        Task Logout();
        Task<bool> IsUserAuthenticated();
        Task<string?> GetUserRole();
        Task<int> GetUserId();
        Task<int?> GetTeacherId();
    }
}
