using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Queries.GetAllCourses;

public record GetAllCoursesQuery : IRequest<IReadOnlyList<CourseDto>>;
