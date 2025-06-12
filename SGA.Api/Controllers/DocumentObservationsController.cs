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
    public class DocumentObservationsController : ControllerBase
    {
        private readonly IDocumentObservationRepository _observationRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ITeacherRepository _teacherRepository;

        public DocumentObservationsController(
            IDocumentObservationRepository observationRepository,
            IDocumentRepository documentRepository,
            ITeacherRepository teacherRepository)
        {
            _observationRepository = observationRepository;
            _documentRepository = documentRepository;
            _teacherRepository = teacherRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentObservationDto>>> GetAll()
        {
            var observations = await _observationRepository.GetAllAsync();
            var observationDtos = observations.Select(MapToDto);

            return Ok(observationDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentObservationDto>> GetById(int id)
        {
            var observation = await _observationRepository.GetByIdAsync(id);
            if (observation == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(observation));
        }

        [HttpGet("document/{documentId}")]
        public async Task<ActionResult<IEnumerable<DocumentObservationDto>>> GetByDocumentId(int documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
            {
                return NotFound("Documento no encontrado");
            }

            var observations = await _observationRepository.GetByDocumentIdAsync(documentId);
            var observationDtos = observations.Select(MapToDto);

            return Ok(observationDtos);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentObservationDto>> Create([FromBody] AddDocumentObservationDto dto, int documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
            {
                return NotFound("Documento no encontrado");
            }

            var reviewer = await _teacherRepository.GetByIdAsync(dto.ReviewerId);
            if (reviewer == null)
            {
                return NotFound("Revisor no encontrado");
            }

            document.AddObservation(dto.Description, reviewer);
            await _documentRepository.UpdateAsync(document);
            await _documentRepository.SaveChangesAsync();

            // Obtenemos la observación recién creada
            var observations = (await _observationRepository.GetByDocumentIdAsync(documentId))
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            return CreatedAtAction(nameof(GetById), new { id = observations.Id }, MapToDto(observations));
        }

        private DocumentObservationDto MapToDto(DocumentObservation observation)
        {
            return new DocumentObservationDto
            {
                Id = observation.Id,
                Description = observation.Description,
                CreatedAt = observation.CreatedAt,
                DocumentId = observation.DocumentId,
                ReviewerId = observation.ReviewerId,
                ReviewerFullName = observation.Reviewer != null ? $"{observation.Reviewer.FirstName} {observation.Reviewer.LastName}" : string.Empty
            };
        }
    }
}
