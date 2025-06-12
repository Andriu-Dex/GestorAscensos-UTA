using System;
using Xunit;
using Moq;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace TempProject
{
    // Custom implementation for testing
    public class ValidacionAscensoService
    {
        private readonly IDocenteRepository _docenteRepository;

        public ValidacionAscensoService(IDocenteRepository docenteRepository)
        {
            _docenteRepository = docenteRepository;
        }

        public async Task<RequisitosAscensoResult> ValidarRequisitosAscensoAsync(int docenteId, int nivelSolicitado)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            
            if (docente == null)
                throw new InvalidOperationException($"No se encontró el docente con ID {docenteId}");
                
            if (nivelSolicitado != docente.NivelActual + 1)
                throw new InvalidOperationException("Solo se puede solicitar ascenso al nivel inmediato superior");
                
            if (docente.NivelActual >= 5) // Suponiendo que 5 es el nivel máximo
                throw new InvalidOperationException("Ya ha alcanzado el nivel máximo de la carrera docente");
                
            var indicadores = docente.Indicadores;
            
            bool cumpleTiempo = indicadores?.TiempoEnRolActual >= 36; // 3 años mínimo
            bool cumpleObras = indicadores?.NumeroObras >= 2; // 2 obras mínimo
            bool cumpleEvaluacion = indicadores?.PuntajeEvaluacion >= 80; // 80% mínimo
            bool cumpleCapacitacion = indicadores?.HorasCapacitacion >= 100; // 100 horas mínimo
            bool cumpleInvestigacion = indicadores?.TiempoInvestigacion >= 12; // 1 año mínimo
            
            var result = new RequisitosAscensoResult
            {
                CumpleTiempo = cumpleTiempo,
                CumpleObras = cumpleObras,
                CumpleEvaluacion = cumpleEvaluacion,
                CumpleCapacitacion = cumpleCapacitacion,
                CumpleInvestigacion = cumpleInvestigacion,
                CumpleTodosRequisitos = cumpleTiempo && cumpleObras && cumpleEvaluacion && 
                                      cumpleCapacitacion && cumpleInvestigacion
            };
            
            return result;
        }
    }
    
    // Simple result class for testing
    public class RequisitosAscensoResult
    {
        public bool CumpleTodosRequisitos { get; set; }
        public bool CumpleTiempo { get; set; }
        public bool CumpleObras { get; set; }
        public bool CumpleEvaluacion { get; set; }
        public bool CumpleCapacitacion { get; set; }
        public bool CumpleInvestigacion { get; set; }
    }

    public class ValidacionAscensoServiceTests
    {
        private readonly Mock<IDocenteRepository> _mockDocenteRepository;
        private readonly ValidacionAscensoService _validacionService;

        public ValidacionAscensoServiceTests()
        {
            _mockDocenteRepository = new Mock<IDocenteRepository>();
            _validacionService = new ValidacionAscensoService(_mockDocenteRepository.Object);
        }

        [Fact]
        public async Task ValidarRequisitosAscenso_CumpleTodosRequisitos_ReturnTrue()
        {
            // Arrange
            var docente = new Docente
            {
                Id = 1,
                NivelActual = 1,
                Nombres = "Test",
                Apellidos = "User",
                Cedula = "1234567890",
                Email = "test@example.com",
                TelefonoContacto = "1234567890",
                NombreUsuario = "testuser",
                PasswordHash = "hashedpassword",
                Indicadores = new IndicadorDocente
                {
                    DocenteId = 1,
                    TiempoEnRolActual = 36, // 36 meses (3 años)
                    NumeroObras = 2,
                    PuntajeEvaluacion = 85,
                    HorasCapacitacion = 100,
                    TiempoInvestigacion = 24 // 24 meses (2 años)
                }
            };

            _mockDocenteRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(docente);

            // Act
            var result = await _validacionService.ValidarRequisitosAscensoAsync(1, 2);

            // Assert
            Assert.True(result.CumpleTodosRequisitos);
            Console.WriteLine("Test passed successfully!");
        }
    }
}
}
