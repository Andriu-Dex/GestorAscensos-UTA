using System;
using System.Threading.Tasks;
using SGA.Domain.Entities;
using SGA.Domain.Utilities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
    public interface IEmailNormalizationService
    {
        Task<int> NormalizarCorreosExistentesAsync();
        Task NormalizarCorreoDocente(DatosTTHH datosTTHH);
    }

    public class EmailNormalizationService : IEmailNormalizationService
    {
        private readonly IDatosTTHHRepository _datosTTHHRepository;

        public EmailNormalizationService(IDatosTTHHRepository datosTTHHRepository)
        {
            _datosTTHHRepository = datosTTHHRepository;
        }

        public async Task<int> NormalizarCorreosExistentesAsync()
        {
            // Obtener todos los registros de TTHH que no tienen correo institucional o lo tienen mal formateado
            var datosTTHH = await _datosTTHHRepository.GetAllAsync();
            int actualizados = 0;

            foreach (var dato in datosTTHH)
            {
                // Generar el correo normalizado usando la utilidad
                string correoNormalizado = EmailNormalizer.GenerarCorreoInstitucional(dato.Nombres, dato.Apellidos);
                
                // Solo actualizar si el correo institucional es diferente
                if (dato.EmailInstitucional != correoNormalizado)
                {
                    dato.EmailInstitucional = correoNormalizado;
                    dato.FechaActualizacion = DateTime.Now;
                    
                    await _datosTTHHRepository.UpdateAsync(dato);
                    actualizados++;
                }
            }

            return actualizados;
        }

        public async Task NormalizarCorreoDocente(DatosTTHH datosTTHH)
        {
            // Generar el correo normalizado para un docente específico
            datosTTHH.EmailInstitucional = EmailNormalizer.GenerarCorreoInstitucional(datosTTHH.Nombres, datosTTHH.Apellidos);
            datosTTHH.FechaActualizacion = DateTime.Now;
            
            await _datosTTHHRepository.UpdateAsync(datosTTHH);
        }
    }
}