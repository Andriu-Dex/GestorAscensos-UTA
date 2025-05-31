using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
    public interface IDocumentoService
    {
        Task<Documento> GetDocumentoByIdAsync(int id);
        Task<IEnumerable<Documento>> GetDocumentosByDocenteIdAsync(int docenteId);
        Task<IEnumerable<Documento>> GetDocumentosByTipoAsync(int docenteId, TipoDocumento tipo);
        Task<Documento> SubirDocumentoAsync(int docenteId, string nombre, string descripcion, TipoDocumento tipo, Stream contenido, string contentType);
        Task EliminarDocumentoAsync(int documentoId);
    }

    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IDocenteRepository _docenteRepository;
        private const int MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB

        public DocumentoService(IDocumentoRepository documentoRepository, IDocenteRepository docenteRepository)
        {
            _documentoRepository = documentoRepository;
            _docenteRepository = docenteRepository;
        }

        public async Task<Documento> GetDocumentoByIdAsync(int id)
        {
            return await _documentoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Documento>> GetDocumentosByDocenteIdAsync(int docenteId)
        {
            return await _documentoRepository.GetByDocenteIdAsync(docenteId);
        }

        public async Task<IEnumerable<Documento>> GetDocumentosByTipoAsync(int docenteId, TipoDocumento tipo)
        {
            return await _documentoRepository.GetByTipoAsync(docenteId, tipo);
        }

        public async Task<Documento> SubirDocumentoAsync(int docenteId, string nombre, string descripcion, TipoDocumento tipo, Stream contenido, string contentType)
        {
            if (contentType != "application/pdf")
                throw new Exception("Solo se permiten archivos PDF");

            if (contenido.Length > MAX_FILE_SIZE)
                throw new Exception($"El tamaño del archivo excede el límite permitido de {MAX_FILE_SIZE / 1024 / 1024} MB");

            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new Exception("Docente no encontrado");

            using var memoryStream = new MemoryStream();
            await contenido.CopyToAsync(memoryStream);

            var documento = new Documento
            {
                DocenteId = docenteId,
                Nombre = nombre,
                Descripcion = descripcion,
                Tipo = tipo,
                Contenido = memoryStream.ToArray(),
                ContentType = contentType,
                TamanioBytes = memoryStream.Length,
                FechaSubida = DateTime.Now
            };

            await _documentoRepository.AddAsync(documento);
            return documento;
        }

        public async Task EliminarDocumentoAsync(int documentoId)
        {
            await _documentoRepository.DeleteAsync(documentoId);
        }
    }
}
