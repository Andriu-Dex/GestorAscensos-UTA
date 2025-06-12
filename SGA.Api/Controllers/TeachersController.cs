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
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IPromotionService _promotionService;

        public TeachersController(ITeacherRepository teacherRepository, IPromotionService promotionService)
        {
            _teacherRepository = teacherRepository;
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAll()
        {            var teachers = await _teacherRepository.GetAllAsync();
            var teacherDtos = teachers.Select(t => new TeacherDto
            {
                Id = t.Id,
                IdentificationNumber = t.IdentificationNumber,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email,
                CurrentRank = t.CurrentRank,
                StartDateInCurrentRank = t.StartDateInCurrentRank,
                DaysInCurrentRank = t.DaysInCurrentRank,
                UserTypeId = t.UserTypeId,
                UserTypeName = t.UserType?.Name ?? string.Empty,
                Works = t.Works,
                EvaluationScore = t.EvaluationScore,
                TrainingHours = t.TrainingHours,
                ResearchMonths = t.ResearchMonths,
                YearsInCurrentRank = t.YearsInCurrentRank
            });

            return Ok(teacherDtos);
        }        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetById(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            var teacherDto = new TeacherDto
            {
                Id = teacher.Id,
                IdentificationNumber = teacher.IdentificationNumber,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                CurrentRank = teacher.CurrentRank,
                StartDateInCurrentRank = teacher.StartDateInCurrentRank,
                DaysInCurrentRank = teacher.DaysInCurrentRank,
                UserTypeId = teacher.UserTypeId,
                UserTypeName = teacher.UserType?.Name ?? string.Empty,
                Works = teacher.Works,
                EvaluationScore = teacher.EvaluationScore,
                TrainingHours = teacher.TrainingHours,
                ResearchMonths = teacher.ResearchMonths,
                YearsInCurrentRank = teacher.YearsInCurrentRank
            };

            return Ok(teacherDto);
        }

        [HttpGet("{id}/eligibility")]
        public async Task<ActionResult<PromotionEligibilityResultDto>> CheckEligibility(int id)
        {
            var eligibilityResult = await _promotionService.CheckEligibilityAsync(id);
            if (eligibilityResult == null)
            {
                return NotFound();
            }

            var resultDto = new PromotionEligibilityResultDto
            {
                IsEligible = eligibilityResult.IsEligible,
                Message = eligibilityResult.Message,
                CurrentRank = eligibilityResult.CurrentRank,
                TargetRank = eligibilityResult.TargetRank,
                RequirementsMet = eligibilityResult.RequirementsMet
            };

            return Ok(resultDto);
        }

        [HttpPost("{id}/promotion-requests")]
        public async Task<ActionResult> CreatePromotionRequest(int id)
        {
            var result = await _promotionService.CreatePromotionRequestAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }            return CreatedAtAction(
                nameof(PromotionRequestsController.GetById), 
                "PromotionRequests", 
                new { id = result.PromotionRequestId }, 
                result);
        }
        
        [HttpGet("{id}/academic-degrees")]
        public async Task<ActionResult<IEnumerable<AcademicDegreeDto>>> GetAcademicDegrees(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            
            var academicDegrees = teacher.AcademicDegrees;
            var academicDegreeDtos = academicDegrees.Select(ad => new AcademicDegreeDto
            {
                Id = ad.Id,
                DegreeType = ad.DegreeType,
                Title = ad.Title,
                IssuingInstitution = ad.IssuingInstitution,
                TeacherId = ad.TeacherId
            });
            
            return Ok(academicDegreeDtos);
        }
    }
}
