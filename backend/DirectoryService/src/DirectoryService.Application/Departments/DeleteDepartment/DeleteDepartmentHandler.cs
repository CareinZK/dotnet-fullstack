using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Domain.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class DeleteDepartmentHandler : ICommandHandler<DeleteDepartmentCommand>
{
    private readonly IDepartmentRepository _departmentRepository;

    public DeleteDepartmentHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<UnitResult<ErrorList>> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        var deleteResult = await _departmentRepository.DeleteAsync(command.Id, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return deleteResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }
}
