using AutoMapper;
using MediatR;
using SGA.Application.DTOs.Docentes;
using SGA.Application.Features.Docentes.Commands;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;

namespace SGA.Application.Features.Docentes.Handlers;

public class DocenteCommandHandlers :
    IRequestHandler<CreateDocenteCommand, DocenteDto>,
    IRequestHandler<UpdateDocenteCommand, DocenteDto>,
    IRequestHandler<ImportarDatosTTHHCommand, DocenteDto>,
    IRequestHandler<ImportarDatosDACCommand, DocenteDto>,
    IRequestHandler<ImportarDatosDITICCommand, DocenteDto>,
    IRequestHandler<ImportarDatosDirInvCommand, DocenteDto>,
    IRequestHandler<ImportarTodosLosDatosCommand, DocenteDto>
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly IExternalDataService _externalDataService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IMapper _mapper;

    public DocenteCommandHandlers(
        IDocenteRepository docenteRepository,
        IExternalDataService externalDataService,
        IAuditoriaService auditoriaService,
        IMapper mapper)
    {
        _docenteRepository = docenteRepository;
        _externalDataService = externalDataService;
        _auditoriaService = auditoriaService;
        _mapper = mapper;
    }

    public async Task<DocenteDto> Handle(CreateDocenteCommand request, CancellationToken cancellationToken)
    {
        var docente = _mapper.Map<Docente>(request);
        docente = await _docenteRepository.CreateAsync(docente);
        
        await _auditoriaService.RegistrarAccionAsync(
            "Docente Creado", 
            request.UsuarioId?.ToString(), 
            request.Email,
            "Docente", 
            null, 
            $"Nuevo docente: {docente.Nombres} {docente.Apellidos}",
            null);

        return _mapper.Map<DocenteDto>(docente);
    }

    public async Task<DocenteDto> Handle(UpdateDocenteCommand request, CancellationToken cancellationToken)
    {
        var docenteExistente = await _docenteRepository.GetByIdAsync(request.Id);
        if (docenteExistente == null)
            throw new ArgumentException("Docente no encontrado");

        var valoresAnteriores = $"{docenteExistente.Nombres} {docenteExistente.Apellidos}";
        
        _mapper.Map(request, docenteExistente);
        docenteExistente = await _docenteRepository.UpdateAsync(docenteExistente);

        await _auditoriaService.RegistrarAccionAsync(
            "Docente Actualizado",
            request.UsuarioId?.ToString(),
            docenteExistente.Email,
            "Docente",
            valoresAnteriores,
            $"{docenteExistente.Nombres} {docenteExistente.Apellidos}",
            null);

        return _mapper.Map<DocenteDto>(docenteExistente);
    }

    public async Task<DocenteDto> Handle(ImportarDatosTTHHCommand request, CancellationToken cancellationToken)
    {
        var docente = await _docenteRepository.GetByIdAsync(request.DocenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var datosTTHH = await _externalDataService.ImportarDatosTTHHAsync(docente.Cedula);
        if (datosTTHH != null)
        {
            // Actualizar datos del docente con información de TTHH
            if (datosTTHH.FechaNombramiento.HasValue)
                docente.FechaNombramiento = datosTTHH.FechaNombramiento;
            
            if (datosTTHH.FechaIngresoNivelActual.HasValue)
                docente.FechaInicioNivelActual = datosTTHH.FechaIngresoNivelActual.Value;

            docente = await _docenteRepository.UpdateAsync(docente);
        }

        await _auditoriaService.RegistrarAccionAsync(
            "Datos TTHH Importados",
            request.UsuarioId?.ToString(),
            docente.Email,
            "Docente",
            null,
            "Importación de datos desde TTHH",
            null);

        return _mapper.Map<DocenteDto>(docente);
    }

    public async Task<DocenteDto> Handle(ImportarDatosDACCommand request, CancellationToken cancellationToken)
    {
        var docente = await _docenteRepository.GetByIdAsync(request.DocenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var datosDAC = await _externalDataService.ImportarDatosDACAsync(docente.Cedula);
        if (datosDAC != null)
        {
            docente.PromedioEvaluaciones = datosDAC.PromedioEvaluaciones;
            docente = await _docenteRepository.UpdateAsync(docente);
        }

        await _auditoriaService.RegistrarAccionAsync(
            "Datos DAC Importados",
            request.UsuarioId?.ToString(),
            docente.Email,
            "Docente",
            null,
            "Importación de datos desde DAC",
            null);

        return _mapper.Map<DocenteDto>(docente);
    }

    public async Task<DocenteDto> Handle(ImportarDatosDITICCommand request, CancellationToken cancellationToken)
    {
        var docente = await _docenteRepository.GetByIdAsync(request.DocenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var datosDITIC = await _externalDataService.ImportarDatosDITICAsync(docente.Cedula);
        if (datosDITIC != null)
        {
            docente.HorasCapacitacion = datosDITIC.HorasCapacitacion;
            docente = await _docenteRepository.UpdateAsync(docente);
        }

        await _auditoriaService.RegistrarAccionAsync(
            "Datos DITIC Importados",
            request.UsuarioId?.ToString(),
            docente.Email,
            "Docente",
            null,
            "Importación de datos desde DITIC",
            null);

        return _mapper.Map<DocenteDto>(docente);
    }

    public async Task<DocenteDto> Handle(ImportarDatosDirInvCommand request, CancellationToken cancellationToken)
    {
        var docente = await _docenteRepository.GetByIdAsync(request.DocenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var datosDirInv = await _externalDataService.ImportarDatosDirInvAsync(docente.Cedula);
        if (datosDirInv != null)
        {
            docente.NumeroObrasAcademicas = datosDirInv.NumeroObras;
            docente.MesesInvestigacion = datosDirInv.MesesInvestigacion;
            docente = await _docenteRepository.UpdateAsync(docente);
        }

        await _auditoriaService.RegistrarAccionAsync(
            "Datos DIR INV Importados",
            request.UsuarioId?.ToString(),
            docente.Email,
            "Docente",
            null,
            "Importación de datos desde DIR INV",
            null);

        return _mapper.Map<DocenteDto>(docente);
    }

    public async Task<DocenteDto> Handle(ImportarTodosLosDatosCommand request, CancellationToken cancellationToken)
    {
        var docente = await _docenteRepository.GetByIdAsync(request.DocenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        // Importar de todos los sistemas
        var datosTTHH = await _externalDataService.ImportarDatosTTHHAsync(docente.Cedula);
        var datosDAC = await _externalDataService.ImportarDatosDACAsync(docente.Cedula);
        var datosDITIC = await _externalDataService.ImportarDatosDITICAsync(docente.Cedula);
        var datosDirInv = await _externalDataService.ImportarDatosDirInvAsync(docente.Cedula);

        // Actualizar con todos los datos
        if (datosTTHH != null)
        {
            if (datosTTHH.FechaNombramiento.HasValue)
                docente.FechaNombramiento = datosTTHH.FechaNombramiento;
            if (datosTTHH.FechaIngresoNivelActual.HasValue)
                docente.FechaInicioNivelActual = datosTTHH.FechaIngresoNivelActual.Value;
        }

        if (datosDAC != null)
            docente.PromedioEvaluaciones = datosDAC.PromedioEvaluaciones;

        if (datosDITIC != null)
            docente.HorasCapacitacion = datosDITIC.HorasCapacitacion;

        if (datosDirInv != null)
        {
            docente.NumeroObrasAcademicas = datosDirInv.NumeroObras;
            docente.MesesInvestigacion = datosDirInv.MesesInvestigacion;
        }

        docente = await _docenteRepository.UpdateAsync(docente);

        await _auditoriaService.RegistrarAccionAsync(
            "Datos Completos Importados",
            request.UsuarioId?.ToString(),
            docente.Email,
            "Docente",
            null,
            "Importación completa de datos desde todos los sistemas",
            null);

        return _mapper.Map<DocenteDto>(docente);
    }
}
