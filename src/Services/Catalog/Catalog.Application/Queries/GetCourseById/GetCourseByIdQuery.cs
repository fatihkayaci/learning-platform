using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Queries.GetCourseById;

public record GetCourseByIdQuery(Guid Id) : IRequest<CourseDetailDto?>;
