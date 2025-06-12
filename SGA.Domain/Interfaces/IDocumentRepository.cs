using SGA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Interfaz de repositorio para la entidad Document
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Obtiene un documento por ID
        /// </summary>
        Task<Document> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene documentos por ID del profesor
        /// </summary>
        Task<IEnumerable<Document>> GetByTeacherIdAsync(int teacherId);
        
        /// <summary>
        /// Obtiene documentos por ID del requisito
        /// </summary>
        Task<IEnumerable<Document>> GetByRequirementIdAsync(int requirementId);
        
        /// <summary>
        /// Obtiene documentos por tipo de documento
        /// </summary>
        Task<IEnumerable<Document>> GetByDocumentTypeAsync(string documentType);
        
        /// <summary>
        /// Obtiene todos los documentos
        /// </summary>
        Task<IEnumerable<Document>> GetAllAsync();
        
        /// <summary>
        /// Añade un nuevo documento
        /// </summary>
        Task AddAsync(Document document);
        
        /// <summary>
        /// Actualiza un documento existente
        /// </summary>
        Task UpdateAsync(Document document);
        
        /// <summary>
        /// Elimina un documento
        /// </summary>
        Task DeleteAsync(Document document);
        
        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task SaveChangesAsync();
    }
}
