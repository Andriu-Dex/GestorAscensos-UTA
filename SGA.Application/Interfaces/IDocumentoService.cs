using SGA.Application.DTOs.Documentos;
using SGA.Domain.Entities;

namespace SGA.Application.Interfaces;

public interface IDocumentoService 
{
    Task<Guid> SubirDocumentoAsync(Guid solicitudId, SubirDocumentoRequestDto documento);
    Task<byte[]?> DescargarDocumentoAsync(Guid documentoId);
    Task<bool> EliminarDocumentoAsync(Guid documentoId);
    Task<bool> ValidarDocumentoAsync(SubirDocumentoRequestDto documento);
    Task<byte[]> ComprimirPDFAsync(byte[] contenidoPdf);
    Task<Documento?> ObtenerDocumentoPorIdAsync(Guid documentoId);
    Task<bool> AsociarDocumentoASolicitudAsync(Guid documentoId, Guid solicitudId);
}
