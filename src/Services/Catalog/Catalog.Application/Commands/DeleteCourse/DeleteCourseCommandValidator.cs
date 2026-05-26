using FluentValidation;

namespace Catalog.Application.Commands.DeleteCourse;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.id).NotEmpty();
    }
}