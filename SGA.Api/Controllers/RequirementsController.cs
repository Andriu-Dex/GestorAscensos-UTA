using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequirementsController : ControllerBase
    {
        private readonly IRequirementRepository _requirementRepository;

        public RequirementsController(IRequirementRepository requirementRepository)
        {
            _requirementRepository = requirementRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementDto>>> GetAll()
        {
            var requirements = await _requirementRepository.GetAllAsync();
            var requirementDtos = requirements.Select(r => new RequirementDto
            {
                Id = r.Id,
                Name = r.Name,
                YearsInCurrentRank = r.YearsInCurrentRank,
                RequiredWorks = r.RequiredWorks,
                MinimumEvaluationScore = r.MinimumEvaluationScore,
                RequiredTrainingHours = r.RequiredTrainingHours,
                RequiredResearchMonths = r.RequiredResearchMonths
            });

            return Ok(requirementDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementDto>> GetById(int id)
        {
            var requirement = await _requirementRepository.GetByIdAsync(id);
            if (requirement == null)
            {
                return NotFound();
            }

            var requirementDto = new RequirementDto
            {
                Id = requirement.Id,
                Name = requirement.Name,
                YearsInCurrentRank = requirement.YearsInCurrentRank,
                RequiredWorks = requirement.RequiredWorks,
                MinimumEvaluationScore = requirement.MinimumEvaluationScore,
                RequiredTrainingHours = requirement.RequiredTrainingHours,
                RequiredResearchMonths = requirement.RequiredResearchMonths
            };

            return Ok(requirementDto);
        }
    }
}
