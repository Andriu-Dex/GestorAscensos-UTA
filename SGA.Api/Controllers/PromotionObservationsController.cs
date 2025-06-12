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
    public class PromotionObservationsController : ControllerBase
    {
        private readonly IPromotionObservationRepository _observationRepository;
        private readonly IPromotionRequestRepository _requestRepository;

        public PromotionObservationsController(
            IPromotionObservationRepository observationRepository,
            IPromotionRequestRepository requestRepository)
        {
            _observationRepository = observationRepository;
            _requestRepository = requestRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromotionObservationDto>>> GetAll()
        {
            var observations = await _observationRepository.GetAllAsync();
            var observationDtos = observations.Select(MapToDto);

            return Ok(observationDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionObservationDto>> GetById(int id)
        {
            var observation = await _observationRepository.GetByIdAsync(id);
            if (observation == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(observation));
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<IEnumerable<PromotionObservationDto>>> GetByRequestId(int requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound("Solicitud de promoción no encontrada");
            }

            var observations = await _observationRepository.GetByPromotionRequestIdAsync(requestId);
            var observationDtos = observations.Select(MapToDto);

            return Ok(observationDtos);
        }

        [HttpPost]
        public async Task<ActionResult<PromotionObservationDto>> Create([FromBody] CreatePromotionObservationDto dto)
        {
            var request = await _requestRepository.GetByIdAsync(dto.PromotionRequestId);
            if (request == null)
            {
                return NotFound("Solicitud de promoción no encontrada");
            }

            request.AddObservation(dto.Description);
            await _requestRepository.UpdateAsync(request);

            // Obtenemos la observación recién creada
            var observations = (await _observationRepository.GetByPromotionRequestIdAsync(dto.PromotionRequestId))
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            return CreatedAtAction(nameof(GetById), new { id = observations.Id }, MapToDto(observations));
        }

        private PromotionObservationDto MapToDto(PromotionObservation observation)
        {
            return new PromotionObservationDto
            {
                Id = observation.Id,
                Description = observation.Description,
                CreatedAt = observation.CreatedAt,
                PromotionRequestId = observation.PromotionRequestId
            };
        }
    }
}
