using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
    public interface IServicioExternoService
    {
        Task<T?> ConsumirServicioDITIC<T>(string endpoint, string? requestBody = null, HttpMethod? method = null);
        Task<T?> ConsumirServicioDAC<T>(string endpoint, string? requestBody = null, HttpMethod? method = null);
        Task<T?> ConsumirServicioTTHH<T>(string endpoint, string? requestBody = null, HttpMethod? method = null);
        Task<bool> ProbarConexionServicio(string codigo);
        Task<IEnumerable<ServicioExterno>> ObtenerServiciosActivosAsync();
    }

    public class ServicioExternoService : IServicioExternoService
    {
        private readonly IServicioExternoRepository _servicioExternoRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ServicioExternoService> _logger;
        private readonly ILogService _logService;

        private const string CODIGO_DITIC = "DITIC";
        private const string CODIGO_DAC = "DAC";
        private const string CODIGO_TTHH = "TTHH";

        public ServicioExternoService(
            IServicioExternoRepository servicioExternoRepository,
            IHttpClientFactory httpClientFactory,
            ILogger<ServicioExternoService> logger,
            ILogService logService)
        {
            _servicioExternoRepository = servicioExternoRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _logService = logService;
        }

        public async Task<T?> ConsumirServicioDITIC<T>(string endpoint, string? requestBody = null, HttpMethod? method = null)
        {
            return await ConsumirServicioGenerico<T>(CODIGO_DITIC, endpoint, requestBody, method);
        }

        public async Task<T?> ConsumirServicioDAC<T>(string endpoint, string? requestBody = null, HttpMethod? method = null)
        {
            return await ConsumirServicioGenerico<T>(CODIGO_DAC, endpoint, requestBody, method);
        }

        public async Task<T?> ConsumirServicioTTHH<T>(string endpoint, string? requestBody = null, HttpMethod? method = null)
        {
            return await ConsumirServicioGenerico<T>(CODIGO_TTHH, endpoint, requestBody, method);
        }

        private async Task<T?> ConsumirServicioGenerico<T>(string codigoServicio, string endpoint, string? requestBody = null, HttpMethod? method = null)
        {
            try
            {
                var servicioExterno = await _servicioExternoRepository.GetByCodigoAsync(codigoServicio);
                if (servicioExterno == null || !servicioExterno.Activo)
                {
                    await _logService.LogAdvertenciaAsync(
                        "ConsumirServicio", 
                        "ServicioExterno", 
                        $"El servicio {codigoServicio} no está disponible o no está activo.");
                    return default;
                }

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(servicioExterno.TimeoutSegundos);
                client.BaseAddress = new Uri(servicioExterno.UrlBase);

                if (!string.IsNullOrEmpty(servicioExterno.ApiKey))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", servicioExterno.ApiKey);
                }

                HttpResponseMessage response;
                HttpMethod httpMethod = method ?? HttpMethod.Get;

                if (httpMethod == HttpMethod.Get)
                {
                    response = await client.GetAsync(endpoint);
                }
                else if (httpMethod == HttpMethod.Post)
                {
                    HttpContent content = new StringContent(requestBody ?? "");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(endpoint, content);
                }
                else if (httpMethod == HttpMethod.Put)
                {
                    HttpContent content = new StringContent(requestBody ?? "");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PutAsync(endpoint, content);
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    response = await client.DeleteAsync(endpoint);
                }
                else
                {
                    throw new NotSupportedException($"Método HTTP no soportado: {httpMethod}");
                }

                await ActualizarEstadoConexion(servicioExterno.Id, response.IsSuccessStatusCode, 
                    !response.IsSuccessStatusCode ? $"Error HTTP: {response.StatusCode}" : null);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await _logService.LogErrorAsync(
                        "ConsumirServicio",
                        "ServicioExterno",
                        new Exception($"Error al consumir servicio {codigoServicio}: {response.StatusCode}, {errorContent}"));
                    return default;
                }
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "ConsumirServicio",
                    "ServicioExterno",
                    ex);
                return default;
            }
        }

        public async Task<bool> ProbarConexionServicio(string codigo)
        {
            try
            {
                var servicioExterno = await _servicioExternoRepository.GetByCodigoAsync(codigo);
                if (servicioExterno == null)
                    return false;

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(servicioExterno.TimeoutSegundos);
                
                var response = await client.GetAsync(servicioExterno.UrlBase);
                
                await ActualizarEstadoConexion(servicioExterno.Id, response.IsSuccessStatusCode,
                    !response.IsSuccessStatusCode ? $"Error HTTP: {response.StatusCode}" : null);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar conexión con servicio {Codigo}", codigo);
                return false;
            }
        }

        private async Task<bool> ActualizarEstadoConexion(int servicioId, bool exito, string? error = null)
        {
            try
            {
                return await _servicioExternoRepository.ActualizarEstadoConexionAsync(servicioId, exito, error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de conexión del servicio {ServicioId}", servicioId);
                return false;
            }
        }

        public async Task<IEnumerable<ServicioExterno>> ObtenerServiciosActivosAsync()
        {
            return await _servicioExternoRepository.GetAllActivosAsync();
        }
    }
}
