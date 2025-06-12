using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Security.Cryptography;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{    public interface IDocumentoService
    {
        Task<Documento> GetDocumentoByIdAsync(int id);
        Task<IEnumerable<Documento>> GetDocumentosByDocenteIdAsync(int docenteId);
        Task<IEnumerable<Documento>> GetDocumentosByTipoAsync(int docenteId, int tipoDocumentoId);
        Task<Documento> SubirDocumentoAsync(int docenteId, string nombre, string descripcion, int tipoDocumentoId, Stream contenido, string contentType);
        Task EliminarDocumentoAsync(int documentoId);
        Task ActualizarDocumentoAsync(int documentoId, int docenteId, string nombre, string descripcion);
        Task ValidarDocumentoAsync(int documentoId, int validadorId, bool validado, string? observaciones);
        Task<byte[]> ObtenerContenidoDocumentoAsync(int documentoId);
    }    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly ICryptoService _cryptoService;
        private const int MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB

        public DocumentoService(
            IDocumentoRepository documentoRepository, 
            IDocenteRepository docenteRepository,
            ICryptoService cryptoService)
        {
            _documentoRepository = documentoRepository;
            _docenteRepository = docenteRepository;
            _cryptoService = cryptoService;
        }

        public async Task<Documento> GetDocumentoByIdAsync(int id)
        {
            return await _documentoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Documento>> GetDocumentosByDocenteIdAsync(int docenteId)
        {
            return await _documentoRepository.GetByDocenteIdAsync(docenteId);
        }        public async Task<IEnumerable<Documento>> GetDocumentosByTipoAsync(int docenteId, int tipoDocumentoId)
        {
            return await _documentoRepository.GetByTipoAsync(docenteId, tipoDocumentoId);
        }        public async Task<Documento> SubirDocumentoAsync(int docenteId, string nombre, string descripcion, int tipoDocumentoId, Stream contenido, string contentType)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            try 
            {
                // Validación del tipo de archivo
                if (contentType != "application/pdf")
                    throw new Exception("Solo se permiten archivos PDF");

                // Validación del tamaño
                if (contenido.Length > MAX_FILE_SIZE)
                    throw new Exception($"El tamaño del archivo excede el límite permitido de {MAX_FILE_SIZE / 1024 / 1024} MB");

                // Validación adicional de estructura PDF
                if (!ValidarEstructuraPDF(contenido))
                    throw new Exception("El archivo no es un PDF válido o está corrupto");

                var docente = await _docenteRepository.GetByIdAsync(docenteId);
                if (docente == null)
                    throw new Exception("Docente no encontrado");

                // Calcular hash del contenido para verificar integridad
                string hashDocumento = _cryptoService.GenerateHash(contenido);

                using var memoryStream = new MemoryStream();
                contenido.Position = 0; // Asegurar que leemos desde el inicio
                await contenido.CopyToAsync(memoryStream);
                
                // Cifrar el contenido del documento
                byte[] contenidoCifrado = _cryptoService.EncryptData(memoryStream.ToArray(), "documento-pdf");                var documento = new Documento
                {
                    DocenteId = docenteId,
                    Docente = docente,
                    TipoDocumentoId = tipoDocumentoId,
                    TipoDocumento = null!, // Se asignará por Entity Framework al cargar
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Contenido = contenidoCifrado, // Contenido cifrado
                    ContentType = contentType,
                    TamanioBytes = memoryStream.Length,
                    FechaSubida = DateTime.Now,
                    HashSHA256 = hashDocumento // Guardar hash para verificación de integridad
                };

                await _documentoRepository.AddAsync(documento);
                scope.Complete();
                return documento;
            }
            catch (Exception ex)
            {
                // El scope se deshace automáticamente si ocurre una excepción
                throw new Exception($"Error al subir documento: {ex.Message}", ex);
            }
        }public async Task EliminarDocumentoAsync(int documentoId)
        {
            await _documentoRepository.DeleteAsync(documentoId);
        }

        public async Task ActualizarDocumentoAsync(int documentoId, int docenteId, string nombre, string descripcion)
        {
            var documento = await _documentoRepository.GetByIdAsync(documentoId);
            if (documento == null)
                throw new Exception("Documento no encontrado");

            if (documento.DocenteId != docenteId)
                throw new Exception("No tiene permisos para actualizar este documento");

            documento.Nombre = nombre;
            documento.Descripcion = descripcion;
            documento.FechaModificacion = DateTime.Now;

            await _documentoRepository.UpdateAsync(documento);
        }

        public async Task ValidarDocumentoAsync(int documentoId, int validadorId, bool validado, string? observaciones)
        {
            var documento = await _documentoRepository.GetByIdAsync(documentoId);
            if (documento == null)
                throw new Exception("Documento no encontrado");

            documento.Validado = validado;
            documento.FechaValidacion = DateTime.Now;
            documento.ValidadoPorId = validadorId;
            documento.ObservacionesValidacion = observaciones;

            await _documentoRepository.UpdateAsync(documento);
        }

        public async Task<byte[]> ObtenerContenidoDocumentoAsync(int documentoId)
        {
            try
            {
                var documento = await _documentoRepository.GetByIdAsync(documentoId);
                if (documento == null)
                    throw new Exception("Documento no encontrado");

                // Verificar que el documento está activo
                if (!documento.Activo)
                    throw new Exception("El documento no está disponible");

                // Descifrar el contenido del documento
                return _cryptoService.DecryptData(documento.Contenido, "documento-pdf");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener contenido del documento: {ex.Message}", ex);
            }
        }

        // Método para validar la estructura básica de un PDF
        private bool ValidarEstructuraPDF(Stream contenido)
        {
            try
            {
                // Restaurar posición después de la validación
                long posicionOriginal = contenido.Position;
                contenido.Position = 0;

                // Validar la firma del encabezado PDF
                using (var reader = new BinaryReader(contenido, Encoding.ASCII, true))
                {
                    // Comprobar que comienza con la firma PDF "%PDF-"
                    byte[] signature = reader.ReadBytes(5);
                    string signatureText = System.Text.Encoding.ASCII.GetString(signature);
                    
                    // Restaurar posición
                    contenido.Position = posicionOriginal;
                    
                    return signatureText.StartsWith("%PDF-");
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
