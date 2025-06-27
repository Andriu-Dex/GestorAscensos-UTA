using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SGA.Application.Services;
using SGA.Application.Interfaces;
using Xunit;

namespace SGA.Application.Tests.Tests
{
    public class ExternalDataServiceTests
    {
        private readonly ExternalDataService _externalDataService;

        public ExternalDataServiceTests()
        {
            // Crear configuración mock para pruebas
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:TTHHDatabase", "Server=localhost;Database=TTHH_Test;Trusted_Connection=true;"},
                    {"ConnectionStrings:DACDatabase", "Server=localhost;Database=DAC_Test;Trusted_Connection=true;"},
                    {"ConnectionStrings:DITICDatabase", "Server=localhost;Database=DITIC_Test;Trusted_Connection=true;"},
                    {"ConnectionStrings:DIRINVDatabase", "Server=localhost;Database=DIRINV_Test;Trusted_Connection=true;"}
                })
                .Build();
            
            _externalDataService = new ExternalDataService(configuration);
        }

        [Fact]
        public async Task ImportarDatosTTHHAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDatosTTHHAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEqual(DateTime.MinValue, resultado.FechaNombramiento);
            Assert.NotEmpty(resultado.CargoActual);
        }

        [Fact]
        public async Task ImportarDatosDACAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDatosDACAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.PromedioEvaluaciones > 0);
            Assert.True(resultado.PeriodosEvaluados > 0);
        }

        [Fact]
        public async Task ImportarDatosDITICAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDatosDITICAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.HorasCapacitacion > 0);
            Assert.True(resultado.CursosCompletados > 0);
        }

        [Fact]
        public async Task ImportarDatosDirInvAsync_DebeRetornarRespuesta()
        {
            // Arrange
            var cedula = "1234567890";

            // Act
            var resultado = await _externalDataService.ImportarDatosDirInvAsync(cedula);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.NumeroObrasAcademicas >= 0);
            Assert.True(resultado.MesesInvestigacion >= 0);
        }
    }
}
