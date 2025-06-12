using System;
using System.Threading.Tasks;
using Moq;
using SGA.Application.Services;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;
using Xunit;

namespace SGA.Application.Tests.Services
{
    public class ReporteServiceTests
    {
        private readonly Mock<ISolicitudAscensoRepository> _mockSolicitudRepository;
        private readonly Mock<IDocenteRepository> _mockDocenteRepository;
        private readonly Mock<IValidacionAscensoService> _mockValidacionService;
        private readonly ReporteService _reporteService;

        public ReporteServiceTests()
        {
            _mockSolicitudRepository = new Mock<ISolicitudAscensoRepository>();
            _mockDocenteRepository = new Mock<IDocenteRepository>();
            _mockValidacionService = new Mock<IValidacionAscensoService>();
            _reporteService = new ReporteService(
                _mockDocenteRepository.Object,
                _mockSolicitudRepository.Object,
                _mockValidacionService.Object
            );
        }

        [Fact]
        public async Task GenerarReporteSolicitudAsync_ValidSolicitud_ReturnsPdfBytes()
        {
            // Arrange
            var solicitudId = 1;
            var solicitud = new SolicitudAscenso
            {
                Id = solicitudId,
                DocenteId = 1,
                NivelActual = 1,
                NivelSolicitado = 2,
                FechaSolicitud = DateTime.Now.AddDays(-10),
                EstadoSolicitudId = 3, // Aprobada
                EstadoSolicitud = new EstadoSolicitud
                {
                    Id = 3,
                    Nombre = "Aprobada",
                    Codigo = "APROBADA"
                },                Docente = new Docente
                {
                    Id = 1,
                    Nombres = "Juan",
                    Apellidos = "Pérez",
                    Cedula = "1234567890",
                    Email = "juan.perez@uta.edu.ec",
                    TelefonoContacto = "0987654321",
                    NombreUsuario = "juan.perez",
                    PasswordHash = "hashedpassword",
                    FacultadId = 1,
                    Facultad = new Facultad
                    {
                        Id = 1,
                        Nombre = "Facultad de Ingeniería en Sistemas",
                        Codigo = "FIS"
                    }
                }
            };

            // Crear indicadores después para evitar referencia circular
            var indicadores = new IndicadorDocente
            {
                DocenteId = 1,
                TiempoEnRolActual = 36,
                NumeroObras = 2,
                PuntajeEvaluacion = 85,
                HorasCapacitacion = 100,
                TiempoInvestigacion = 24,
                Docente = solicitud.Docente
            };

            solicitud.Docente.Indicadores = indicadores;

            _mockSolicitudRepository.Setup(r => r.GetByIdAsync(solicitudId))
                .ReturnsAsync(solicitud);

            // Act
            var result = await _reporteService.GenerarReporteSolicitudAsync(solicitudId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }        [Fact]
        public async Task GenerarReporteSolicitudAsync_SolicitudNoExiste_ThrowsException()
        {
            // Arrange
            var solicitudId = 999;
            _mockSolicitudRepository.Setup(r => r.GetByIdAsync(solicitudId))
                .ReturnsAsync((SolicitudAscenso)null!);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _reporteService.GenerarReporteSolicitudAsync(solicitudId));
        }

        [Fact]
        public async Task GenerarReporteDocenteAsync_ValidDocente_ReturnsPdfBytes()
        {
            // Arrange
            var docenteId = 1;            var docente = new Docente
            {
                Id = docenteId,
                Nombres = "Juan",
                Apellidos = "Pérez",
                Cedula = "1234567890",
                Email = "juan.perez@uta.edu.ec",
                TelefonoContacto = "0987654321",
                NombreUsuario = "juan.perez",
                PasswordHash = "hashedpassword",
                NivelActual = 1,
                FacultadId = 1,
                Facultad = new Facultad
                {
                    Id = 1,
                    Nombre = "Facultad de Ingeniería en Sistemas",
                    Codigo = "FIS"
                }
            };

            var indicadores = new IndicadorDocente
            {
                DocenteId = docenteId,
                TiempoEnRolActual = 36,
                NumeroObras = 2,
                PuntajeEvaluacion = 85,
                HorasCapacitacion = 100,
                TiempoInvestigacion = 24,
                Docente = docente
            };

            docente.Indicadores = indicadores;

            _mockDocenteRepository.Setup(r => r.GetByIdAsync(docenteId))
                .ReturnsAsync(docente);

            // Act
            var result = await _reporteService.GenerarReporteDocenteAsync(docenteId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }
    }
}
