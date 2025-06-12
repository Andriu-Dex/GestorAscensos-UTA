using SGA.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public interface IAcademicDegreeService
    {
        Task<IEnumerable<AcademicDegreeDto>> GetAllAcademicDegreesAsync();
        Task<AcademicDegreeDto> GetAcademicDegreeByIdAsync(int id);
        Task<IEnumerable<AcademicDegreeDto>> GetAcademicDegreesByTeacherIdAsync(int teacherId);
    }
}
