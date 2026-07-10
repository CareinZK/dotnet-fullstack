using Microsoft.AspNetCore.Mvc;
using TemplateService.Contracts;

namespace TemplateService.Web;

[ApiController]
[Route("departments")]
#pragma warning disable CA1515 // Consider making public types internal
public sealed class DepartmentsController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IDepartmentsService _departmentsService;

    public DepartmentsController(IDepartmentsService departmentsService)
    {
        _departmentsService = departmentsService;
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(
        [FromBody] CreateDepartmentDto departmentDto,
        CancellationToken cancellationToken)
    {
        var createdDepartment = await _departmentsService.CreateAsync(departmentDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetDepartmentById),
            new { id = createdDepartment.Id },
            createdDepartment);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartments(
        CancellationToken cancellationToken)
    {
        var departments = await _departmentsService.GetAllAsync(cancellationToken);
        return Ok(departments);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartmentById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var department = await _departmentsService.GetByIdAsync(id, cancellationToken);

        if (department is null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDepartment(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentDto departmentDto,
        CancellationToken cancellationToken)
    {
        var updated = await _departmentsService.UpdateAsync(id, departmentDto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDepartment(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _departmentsService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}