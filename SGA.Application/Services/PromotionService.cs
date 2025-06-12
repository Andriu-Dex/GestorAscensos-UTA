using System;
using System.Threading.Tasks;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Interfaces;
using SGA.Domain.Models;

namespace SGA.Application.Services
{    public class PromotionService : IPromotionService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IPromotionRequestRepository _promotionRequestRepository;
        private readonly IDocumentRepository _documentRepository;

        public PromotionService(
            ITeacherRepository teacherRepository,
            IPromotionRequestRepository promotionRequestRepository,
            IDocumentRepository documentRepository)
        {
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _promotionRequestRepository = promotionRequestRepository ?? throw new ArgumentNullException(nameof(promotionRequestRepository));
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        }

        public async Task<PromotionEligibilityResult> CheckEligibilityAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                return new PromotionEligibilityResult
                {
                    IsEligible = false,
                    Message = "Teacher not found"
                };
            }

            var eligibilityResult = teacher.CheckPromotionEligibility();
            return new PromotionEligibilityResult
            {
                IsEligible = eligibilityResult.IsEligible,
                Message = eligibilityResult.Message,
                CurrentRank = teacher.CurrentRank,
                TargetRank = teacher.GetNextRank(),
                RequirementsMet = eligibilityResult.RequirementsMet
            };
        }        public async Task<PromotionRequestResult> CreatePromotionRequestAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = "Teacher not found"
                };
            }

            var eligibilityResult = teacher.CheckPromotionEligibility();
            if (!eligibilityResult.IsEligible)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = $"Teacher is not eligible for promotion: {eligibilityResult.Message}"
                };
            }

            // Check if there's already an active request
            var existingRequest = await _promotionRequestRepository.GetActiveRequestByTeacherIdAsync(teacherId);
            if (existingRequest != null)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = $"Teacher already has an active promotion request with status: {existingRequest.Status}"
                };
            }

            // Create a new promotion request
            var promotionRequest = teacher.CreatePromotionRequest();
            await _promotionRequestRepository.AddAsync(promotionRequest);

            return new PromotionRequestResult
            {
                Success = true,
                Message = "Promotion request created successfully",
                PromotionRequestId = promotionRequest.Id
            };
        }

        public async Task<PromotionRequestResult> CreatePromotionRequestWithDocumentAsync(int teacherId, int documentId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = "Profesor no encontrado"
                };
            }

            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = "Documento no encontrado"
                };
            }

            // Verificar que el documento pertenezca al profesor
            if (document.TeacherId != teacherId)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = "El documento no pertenece al profesor"
                };
            }

            var eligibilityResult = teacher.CheckPromotionEligibility();
            if (!eligibilityResult.IsEligible)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = $"El profesor no es elegible para promoción: {eligibilityResult.Message}"
                };
            }

            // Verificar si ya existe una solicitud activa
            var existingRequest = await _promotionRequestRepository.GetActiveRequestByTeacherIdAsync(teacherId);
            if (existingRequest != null)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = $"El profesor ya tiene una solicitud de promoción activa con estado: {existingRequest.Status}"
                };
            }

            // Crear una nueva solicitud de promoción con el documento asociado
            var promotionRequest = PromotionRequest.Create(teacher, teacher.GetNextRank(), document);
            await _promotionRequestRepository.AddAsync(promotionRequest);

            return new PromotionRequestResult
            {
                Success = true,
                Message = "Solicitud de promoción creada exitosamente con documento adjunto",
                PromotionRequestId = promotionRequest.Id
            };
        }

        public async Task<PromotionRequestResult> ProcessPromotionRequestAsync(int requestId, PromotionRequestStatus newStatus, string? comments = null)
        {
            var request = await _promotionRequestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = "Promotion request not found"
                };
            }

            var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);
            if (teacher == null)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = "Teacher not found"
                };
            }

            try
            {
                switch (newStatus)
                {
                    case PromotionRequestStatus.Approved:
                        request.Approve(comments);
                        teacher.PromoteToNextRank();
                        break;
                    case PromotionRequestStatus.Rejected:
                        if (string.IsNullOrWhiteSpace(comments))
                        {
                            return new PromotionRequestResult
                            {
                                Success = false,
                                Message = "Comments are required when rejecting a promotion request"
                            };
                        }
                        request.Reject(comments);
                        break;
                    case PromotionRequestStatus.InProgress:
                        request.MarkInProgress(comments);
                        break;
                    default:
                        return new PromotionRequestResult
                        {
                            Success = false,
                            Message = $"Invalid status change: {newStatus}"
                        };
                }

                await _promotionRequestRepository.UpdateAsync(request);
                
                // If approved, update the teacher as well
                if (newStatus == PromotionRequestStatus.Approved)
                {
                    await _teacherRepository.UpdateAsync(teacher);
                }

                return new PromotionRequestResult
                {
                    Success = true,
                    Message = $"Promotion request {newStatus.ToString().ToLower()} successfully",
                    PromotionRequestId = request.Id
                };
            }
            catch (Exception ex)
            {
                return new PromotionRequestResult
                {
                    Success = false,
                    Message = $"Error processing promotion request: {ex.Message}"
                };
            }
        }    }
}
