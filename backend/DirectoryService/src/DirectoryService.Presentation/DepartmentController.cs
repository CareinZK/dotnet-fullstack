using DirectoryService.Application.Departments;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("departments")]
[ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;
    private readonly UpdateDepartmentNameHandler _updateDepartmentNameHandler;

    public DepartmentsController(
        IDepartmentsService departmentsService,
        UpdateDepartmentNameHandler updateDepartmentNameHandler)
    {
        _departmentsService = departmentsService;
        _updateDepartmentNameHandler = updateDepartmentNameHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope<DepartmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    public async Task<IResult> CreateDepartment(
        [FromBody] CreateDepartmentDto departmentDto,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.CreateAsync(departmentDto, cancellationToken);
        return result.ToCreatedEnvelopeResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(Envelope<IReadOnlyList<DepartmentDto>>), StatusCodes.Status200OK)]
    public async Task<IResult> GetDepartments(
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.GetAllAsync(cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> GetDepartmentById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.GetByIdAsync(id, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpPut("{id:guid}")]
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateDepartment(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentDto departmentDto,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.UpdateAsync(id, departmentDto, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteDepartment(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.DeleteAsync(id, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpPatch("{id:guid}/name")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateDepartmentName(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentNameRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            ErrorList error = Errors.General.ValueIsInvalid(nameof(request.Id), "The route id and request id do not match.");
            return error.ToEnvelopeResult();
        }

        var result = await _updateDepartmentNameHandler.Handle(request, cancellationToken);
        return result.ToEnvelopeResult();
    }
}