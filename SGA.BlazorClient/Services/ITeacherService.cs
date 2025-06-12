using SGA.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public interface ITeacherService
    {
        Task<IEnumerable<TeacherDto>?> GetAllTeachersAsync();
        Task<TeacherDto?> GetTeacherByIdAsync(int id);
        Task<PromotionEligibilityResultDto?> CheckEligibilityAsync(int teacherId);
        Task<PromotionRequestDto?> CreatePromotionRequestAsync(int teacherId);
    }
}
