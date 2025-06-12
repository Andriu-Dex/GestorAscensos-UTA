using SGA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Interfaz de repositorio para la entidad DocumentObservation
    /// </summary>
    public interface IDocumentObservationRepository
    {
        /// <summary>
        /// Obtiene una observación por ID
        /// </summary>
        Task<DocumentObservation> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene observaciones por ID del documento
        /// </summary>
        Task<IEnumerable<DocumentObservation>> GetByDocumentIdAsync(int documentId);
        
        /// <summary>
        /// Obtiene observaciones por ID del revisor
        /// </summary>
        Task<IEnumerable<DocumentObservation>> GetByReviewerIdAsync(int reviewerId);
        
        /// <summary>
        /// Obtiene todas las observaciones
        /// </summary>
        Task<IEnumerable<DocumentObservation>> GetAllAsync();
        
        /// <summary>
        /// Añade una nueva observación
        /// </summary>
        Task AddAsync(DocumentObservation observation);
        
        /// <summary>
        /// Actualiza una observación existente
        /// </summary>
        Task UpdateAsync(DocumentObservation observation);
        
        /// <summary>
        /// Elimina una observación
        /// </summary>
        Task DeleteAsync(DocumentObservation observation);
        
        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task SaveChangesAsync();
    }
}
