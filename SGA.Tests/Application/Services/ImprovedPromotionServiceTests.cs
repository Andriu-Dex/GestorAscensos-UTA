using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SGA.Application.Models;
using SGA.Application.Services;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Tests.Application.Services
{
    [TestClass]
    public class ImprovedPromotionServiceTests
    {
        // Usando null! para indicar al compilador que estos campos serán inicializados en el método TestInitialize
        private Mock<ITeacherRepository> _mockTeacherRepository = null!;
        private Mock<IPromotionRequestRepository> _mockPromotionRequestRepository = null!;
        private PromotionService _promotionService = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockTeacherRepository = new Mock<ITeacherRepository>();
            _mockPromotionRequestRepository = new Mock<IPromotionRequestRepository>();
            _promotionService = new PromotionService(_mockTeacherRepository.Object, _mockPromotionRequestRepository.Object);
        }
        
        [TestMethod]
        public async Task CheckEligibilityAsync_TeacherNotFound_ReturnsNotEligible()
        {
            // Arrange
            int teacherId = 1;
            // Usando un cast explícito a Teacher? para evitar advertencias de nulabilidad
            _mockTeacherRepository.Setup(repo => repo.GetByIdAsync(teacherId))
                .ReturnsAsync((Teacher?)null);

            // Act
            var result = await _promotionService.CheckEligibilityAsync(teacherId);

            // Assert
            Assert.IsFalse(result.IsEligible);
            Assert.AreEqual("Teacher not found", result.Message);
        }

        [TestMethod]
        public async Task CheckEligibilityAsync_TeacherEligible_ReturnsEligibleResult()
        {
            // Arrange
            int teacherId = 1;
            var teacher = new Teacher(
                "1234567890", // identificationNumber
                "John",       // firstName
                "Doe",        // lastName
                "john.doe@example.com", // email
                AcademicRank.Titular1   // initialRank
            )
            {
                Id = teacherId,
                YearsInCurrentRank = 4,
                Works = 1,
                EvaluationScore = 80,
                TrainingHours = 100,
                ResearchMonths = 0
            };

            // Configurar el mock del repositorio para devolver el teacher
            _mockTeacherRepository.Setup(repo => repo.GetByIdAsync(teacherId))
                .ReturnsAsync(teacher);

            // Act
            var result = await _promotionService.CheckEligibilityAsync(teacherId);

            // Assert - asumiendo que la implementación real de CheckPromotionEligibility en Teacher funciona correctamente
            Assert.IsTrue(result.IsEligible);
            Assert.AreEqual(AcademicRank.Titular1, result.CurrentRank);
            Assert.AreEqual(AcademicRank.Titular2, result.TargetRank);
        }
        
        [TestMethod]
        public async Task CreatePromotionRequestAsync_TeacherNotFound_ReturnsFailure()
        {
            // Arrange
            int teacherId = 1;
            // Usando un cast explícito a Teacher? para evitar advertencias de nulabilidad
            _mockTeacherRepository.Setup(repo => repo.GetByIdAsync(teacherId))
                .ReturnsAsync((Teacher?)null);

            // Act
            var result = await _promotionService.CreatePromotionRequestAsync(teacherId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Teacher not found", result.Message);
        }
    }
}
