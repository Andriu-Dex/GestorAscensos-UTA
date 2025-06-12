using SGA.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public interface IUserTypeService
    {
        Task<IEnumerable<UserTypeDto>?> GetAllUserTypesAsync();
        Task<UserTypeDto?> GetUserTypeByIdAsync(int id);
    }
}
