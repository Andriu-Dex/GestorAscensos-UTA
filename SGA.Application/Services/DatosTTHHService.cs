using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
    public interface IDatosTTHHService
    {
        Task<DatosTTHH> GetDatosByCedulaAsync(string cedula);
        Task<bool> ExistsByCedulaAsync(string cedula);
        Task CreateDatosTTHHAsync(DatosTTHH datosTTHH);
        Task<IEnumerable<DatosTTHH>> GetAllAsync();
    }

    public class DatosTTHHService : IDatosTTHHService
    {
        private readonly IDatosTTHHRepository _datosTTHHRepository;

        public DatosTTHHService(IDatosTTHHRepository datosTTHHRepository)
        {
            _datosTTHHRepository = datosTTHHRepository;
        }

        public async Task<DatosTTHH> GetDatosByCedulaAsync(string cedula)
        {
            return await _datosTTHHRepository.GetByCedulaAsync(cedula);
        }

        public async Task<bool> ExistsByCedulaAsync(string cedula)
        {
            return await _datosTTHHRepository.ExistsByCedulaAsync(cedula);
        }

        public async Task CreateDatosTTHHAsync(DatosTTHH datosTTHH)
        {
            var existingData = await _datosTTHHRepository.GetByCedulaAsync(datosTTHH.Cedula);
            if (existingData != null)
            {
                throw new Exception("Ya existen datos registrados para esta cédula");
            }

            datosTTHH.FechaRegistro = DateTime.Now;
            await _datosTTHHRepository.AddAsync(datosTTHH);
        }

        public async Task<IEnumerable<DatosTTHH>> GetAllAsync()
        {
            return await _datosTTHHRepository.GetAllAsync();
        }
    }
}
