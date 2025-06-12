using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicDegreesController : ControllerBase
    {
        private readonly IAcademicDegreeRepository _academicDegreeRepository;
        private readonly ITeacherRepository _teacherRepository;

        public AcademicDegreesController(
            IAcademicDegreeRepository academicDegreeRepository,
            ITeacherRepository teacherRepository)
        {
            _academicDegreeRepository = academicDegreeRepository;
            _teacherRepository = teacherRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicDegreeDto>>> GetAll()
        {
            var degrees = await _academicDegreeRepository.GetAllAsync();
            var degreeDtos = degrees.Select(d => new AcademicDegreeDto
            {
                Id = d.Id,
                DegreeType = d.DegreeType,
                Title = d.Title,
                IssuingInstitution = d.IssuingInstitution,
                TeacherId = d.TeacherId,
                TeacherFullName = d.Teacher != null ? $"{d.Teacher.FirstName} {d.Teacher.LastName}" : string.Empty
            });

            return Ok(degreeDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicDegreeDto>> GetById(int id)
        {
            var degree = await _academicDegreeRepository.GetByIdAsync(id);
            if (degree == null)
            {
                return NotFound();
            }

            var degreeDto = new AcademicDegreeDto
            {
                Id = degree.Id,
                DegreeType = degree.DegreeType,
                Title = degree.Title,
                IssuingInstitution = degree.IssuingInstitution,
                TeacherId = degree.TeacherId,
                TeacherFullName = degree.Teacher != null ? $"{degree.Teacher.FirstName} {degree.Teacher.LastName}" : string.Empty
            };

            return Ok(degreeDto);
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<AcademicDegreeDto>>> GetByTeacherId(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                return NotFound("Docente no encontrado");
            }

            var degrees = await _academicDegreeRepository.GetByTeacherIdAsync(teacherId);
            var degreeDtos = degrees.Select(d => new AcademicDegreeDto
            {
                Id = d.Id,
                DegreeType = d.DegreeType,
                Title = d.Title,
                IssuingInstitution = d.IssuingInstitution,
                TeacherId = d.TeacherId,
                TeacherFullName = $"{teacher.FirstName} {teacher.LastName}"
            });

            return Ok(degreeDtos);
        }
    }
}
