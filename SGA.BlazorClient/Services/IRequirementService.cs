using SGA.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public interface IRequirementService
    {
        Task<IEnumerable<RequirementDto>?> GetAllRequirementsAsync();
        Task<RequirementDto?> GetRequirementByIdAsync(int id);
    }
}
