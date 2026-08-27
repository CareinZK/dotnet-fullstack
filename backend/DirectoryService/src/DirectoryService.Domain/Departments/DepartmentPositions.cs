using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Domain.Departments;

public class DepartmentPositions
{
    private DepartmentPositions()
    {
        PositionIds = [];
    }

    private DepartmentPositions(Guid id, Guid departmentId, IReadOnlyList<Guid> positionIds)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionIds = positionIds.ToList();
    }

    public static Result<DepartmentPositions, Error> Create(Guid id, Guid departmentId, IReadOnlyList<Guid> positionIds)
    {
        if (id == Guid.Empty)
        {
            return Error.Validation("department_positions.id.invalid", "Id cannot be empty.", nameof(id));
        }

        if (departmentId == Guid.Empty)
        {
            return Error.Validation("department_positions.department_id.invalid", "DepartmentId cannot be empty.", nameof(departmentId));
        }

        if (positionIds.Any(p => p == Guid.Empty))
        {
            return Error.Validation("department_positions.position_ids.invalid", "PositionIds cannot contain empty Guid.", nameof(positionIds));
        }

        return new DepartmentPositions(id, departmentId, positionIds);
    }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public IReadOnlyList<Guid> PositionIds { get; private set; }
}
