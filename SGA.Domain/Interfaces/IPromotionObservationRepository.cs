using SGA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Interfaz de repositorio para la entidad PromotionObservation
    /// </summary>
    public interface IPromotionObservationRepository
    {
        /// <summary>
        /// Obtiene una observación por ID
        /// </summary>
        Task<PromotionObservation> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene observaciones por ID de la solicitud de promoción
        /// </summary>
        Task<IEnumerable<PromotionObservation>> GetByPromotionRequestIdAsync(int promotionRequestId);
        
        /// <summary>
        /// Obtiene todas las observaciones
        /// </summary>
        Task<IEnumerable<PromotionObservation>> GetAllAsync();
        
        /// <summary>
        /// Añade una nueva observación
        /// </summary>
        Task AddAsync(PromotionObservation observation);
        
        /// <summary>
        /// Actualiza una observación existente
        /// </summary>
        Task UpdateAsync(PromotionObservation observation);
        
        /// <summary>
        /// Elimina una observación
        /// </summary>
        Task DeleteAsync(PromotionObservation observation);
        
        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task SaveChangesAsync();
    }
}
