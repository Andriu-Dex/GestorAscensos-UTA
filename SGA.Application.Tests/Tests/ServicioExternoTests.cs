using System;
using System.Threading.Tasks;
using SGA.Application.Services;
using Xunit;

namespace SGA.Application.Tests.Tests
{
    public class ExternalDataServiceTests
    {
        private readonly ExternalDataService _externalDataService;

        public ExternalDataServiceTests()
        {
            _externalDataService = new ExternalDataService();
        }

        [Fact]
        public async Task ImportarDesdeTTHHAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDesdeTTHHAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Exitoso);
            Assert.Contains("FechaNombramiento", resultado.DatosImportados.Keys);
        }

        [Fact]
        public async Task ImportarDesdeDADACAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDesdeDADACAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Exitoso);
            Assert.Contains("PromedioEvaluaciones", resultado.DatosImportados.Keys);
        }

        [Fact]
        public async Task ImportarDesdeDITICAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDesdeDITICAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Exitoso);
            Assert.Contains("HorasCapacitacion", resultado.DatosImportados.Keys);
        }

        [Fact]
        public async Task ImportarDesdeDIRINVAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDesdeDIRINVAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Exitoso);
            Assert.Contains("NumeroObrasAcademicas", resultado.DatosImportados.Keys);
            Assert.Contains("MesesInvestigacion", resultado.DatosImportados.Keys);
        }
    }
}
