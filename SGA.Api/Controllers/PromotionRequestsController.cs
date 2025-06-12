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
    public class PromotionRequestsController : ControllerBase
    {
        private readonly IPromotionRequestRepository _promotionRequestRepository;
        private readonly IPromotionService _promotionService;

        public PromotionRequestsController(
            IPromotionRequestRepository promotionRequestRepository,
            IPromotionService promotionService)
        {
            _promotionRequestRepository = promotionRequestRepository;
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromotionRequestDto>>> GetAll()
        {
            var requests = await _promotionRequestRepository.GetAllAsync();
            var requestDtos = requests.Select(MapToDto);

            return Ok(requestDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionRequestDto>> GetById(int id)
        {
            var request = await _promotionRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(request));
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<PromotionRequestDto>>> GetByTeacherId(int teacherId)
        {
            var requests = await _promotionRequestRepository.GetByTeacherIdAsync(teacherId);
            var requestDtos = requests.Select(MapToDto);

            return Ok(requestDtos);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<PromotionRequestDto>>> GetByStatus(SGA.Domain.Enums.PromotionRequestStatus status)
        {
            var requests = await _promotionRequestRepository.GetByStatusAsync(status);
            var requestDtos = requests.Select(MapToDto);

            return Ok(requestDtos);
        }        [HttpPut("{id}/process")]
        public async Task<ActionResult> ProcessRequest(int id, [FromBody] ProcessPromotionRequestDto processRequest)
        {
            var result = await _promotionService.ProcessPromotionRequestAsync(
                id, processRequest.NewStatus, processRequest.Comments);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
        
        [HttpPost("teacher/{teacherId}")]
        public async Task<ActionResult> CreateRequest(int teacherId)
        {
            var result = await _promotionService.CreatePromotionRequestAsync(teacherId);
            
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            
            return CreatedAtAction(nameof(GetById), new { id = result.PromotionRequestId }, result);
        }
        
        [HttpPost("teacher/{teacherId}/withdocument/{documentId}")]
        public async Task<ActionResult> CreateRequestWithDocument(int teacherId, int documentId)
        {
            var result = await _promotionService.CreatePromotionRequestWithDocumentAsync(teacherId, documentId);
            
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            
            return CreatedAtAction(nameof(GetById), new { id = result.PromotionRequestId }, result);
        }
        
        private PromotionRequestDto MapToDto(PromotionRequest request)
        {
            return new PromotionRequestDto
            {
                Id = request.Id,
                TeacherId = request.TeacherId,
                TeacherFullName = request.Teacher?.FirstName + " " + request.Teacher?.LastName,
                CurrentRank = request.CurrentRank,
                TargetRank = request.TargetRank,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                ProcessedDate = request.ProcessedDate,
                Comments = request.Comments,
                DocumentId = request.DocumentId,
                DocumentName = request.Document?.Name,
                ReviewerId = request.ReviewerId,
                ReviewerFullName = request.Reviewer?.FirstName + " " + request.Reviewer?.LastName,
                Observations = request.Observations?.Select(o => new PromotionObservationDto
                {
                    Id = o.Id,
                    Description = o.Description,
                    CreatedAt = o.CreatedAt,
                    PromotionRequestId = o.PromotionRequestId
                }).ToList() ?? new List<PromotionObservationDto>()
            };
        }
    }
}
