using DirectoryService.Application.Departments;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("departments")]
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
    public async Task<IActionResult> CreateDepartment(
        [FromBody] CreateDepartmentDto departmentDto,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.CreateAsync(departmentDto, cancellationToken);

        return result.ToCreatedAtActionResult(
            nameof(GetDepartmentById),
            new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartments(
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.GetAllAsync(cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDepartmentById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateDepartment(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentDto departmentDto,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.UpdateAsync(id, departmentDto, cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDepartment(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.DeleteAsync(id, cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpPatch("{id:guid}/name")]
    public async Task<IActionResult> UpdateDepartmentName(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentNameRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            ErrorList error = Errors.General.ValueIsInvalid(nameof(request.Id), "The route id and request id do not match.");
            return error.ToActionResult();
        }

        var result = await _updateDepartmentNameHandler.Handle(request, cancellationToken);
        return result.ToActionResult();
    }
}