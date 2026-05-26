using MediatR;

namespace Catalog.Application.Commands.DeleteCourse;

public record DeleteCourseCommand(Guid id) : IRequest;