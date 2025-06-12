using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Domain.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IRequirementRepository _requirementRepository;

        public DocumentsController(
            IDocumentRepository documentRepository,
            ITeacherRepository teacherRepository,
            IRequirementRepository requirementRepository)
        {
            _documentRepository = documentRepository;
            _teacherRepository = teacherRepository;
            _requirementRepository = requirementRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll()
        {
            var documents = await _documentRepository.GetAllAsync();
            var documentDtos = documents.Select(MapToDto);

            return Ok(documentDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentDto>> GetById(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(document));
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByTeacherId(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                return NotFound("Docente no encontrado");
            }

            var documents = await _documentRepository.GetByTeacherIdAsync(teacherId);
            var documentDtos = documents.Select(MapToDto);

            return Ok(documentDtos);
        }

        [HttpGet("requirement/{requirementId}")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByRequirementId(int requirementId)
        {
            var requirement = await _requirementRepository.GetByIdAsync(requirementId);
            if (requirement == null)
            {
                return NotFound("Requisito no encontrado");
            }

            var documents = await _documentRepository.GetByRequirementIdAsync(requirementId);
            var documentDtos = documents.Select(MapToDto);

            return Ok(documentDtos);
        }

        [HttpGet("type/{documentType}")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByDocumentType(string documentType)
        {
            var documents = await _documentRepository.GetByDocumentTypeAsync(documentType);
            var documentDtos = documents.Select(MapToDto);

            return Ok(documentDtos);
        }        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            // Determinar tipo MIME para el Content-Type
            string contentType = "application/octet-stream"; // Por defecto
            if (document.Name.EndsWith(".pdf"))
                contentType = "application/pdf";
            else if (document.Name.EndsWith(".doc") || document.Name.EndsWith(".docx"))
                contentType = "application/msword";
            else if (document.Name.EndsWith(".xls") || document.Name.EndsWith(".xlsx"))
                contentType = "application/vnd.ms-excel";
            
            // Devolver el archivo como un FileStreamResult
            return File(document.FileContent, contentType, document.Name);
        }
        
        [HttpPost]
        public async Task<ActionResult<DocumentDto>> UploadDocument([FromForm] UploadDocumentDto uploadDto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(uploadDto.TeacherId);
            if (teacher == null)
            {
                return NotFound("Profesor no encontrado");
            }
            
            // Verificar si se especificó un requisito
            Domain.Entities.Requirement requirement = null;
            if (uploadDto.RequirementId.HasValue)
            {
                requirement = await _requirementRepository.GetByIdAsync(uploadDto.RequirementId.Value);
                if (requirement == null)
                {
                    return NotFound("Requisito no encontrado");
                }
            }
            
            // Leer el archivo cargado
            using var memoryStream = new MemoryStream();
            await uploadDto.File.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();
            
            // Crear el documento
            var document = new Domain.Entities.Document(
                uploadDto.Name,
                uploadDto.Description,
                uploadDto.DocumentType,
                uploadDto.StartDate,
                uploadDto.EndDate,
                uploadDto.Department,
                uploadDto.IssuingInstitution,
                uploadDto.DurationHours,
                fileContent,
                teacher,
                requirement
            );
            
            // Guardar en la base de datos
            await _documentRepository.AddAsync(document);
            await _documentRepository.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, MapToDto(document));
        }
        
        [HttpPost("{id}/observation")]
        public async Task<ActionResult> AddObservation(int id, [FromBody] AddDocumentObservationDto observationDto)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound("Documento no encontrado");
            }
            
            var reviewer = await _teacherRepository.GetByIdAsync(observationDto.ReviewerId);
            if (reviewer == null)
            {
                return NotFound("Revisor no encontrado");
            }
            
            document.AddObservation(observationDto.Description, reviewer);
            await _documentRepository.UpdateAsync(document);
            await _documentRepository.SaveChangesAsync();
            
            return Ok("Observación añadida correctamente");
        }

        private DocumentDto MapToDto(Domain.Entities.Document document)
        {
            return new DocumentDto
            {
                Id = document.Id,
                Name = document.Name,
                Description = document.Description,
                DocumentType = document.DocumentType,
                StartDate = document.StartDate,
                EndDate = document.EndDate,
                UploadDate = document.UploadDate,
                IsEditable = document.IsEditable,
                Department = document.Department,
                IssuingInstitution = document.IssuingInstitution,
                DurationHours = document.DurationHours,
                
                TeacherId = document.TeacherId,
                TeacherFullName = document.Teacher != null ? $"{document.Teacher.FirstName} {document.Teacher.LastName}" : string.Empty,
                
                ReviewerId = document.ReviewerId,
                ReviewerFullName = document.Reviewer != null ? $"{document.Reviewer.FirstName} {document.Reviewer.LastName}" : string.Empty,
                
                RequirementId = document.RequirementId,
                RequirementName = document.Requirement?.Name ?? string.Empty
            };
        }
    }
}
