using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Queries.GetReviewsByCourse;

public record GetReviewsByCourseQuery(Guid CourseId) : IRequest<IReadOnlyList<ReviewDto>>;
