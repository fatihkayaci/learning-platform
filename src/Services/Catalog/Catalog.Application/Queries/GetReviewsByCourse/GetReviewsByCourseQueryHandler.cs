using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Queries.GetReviewsByCourse;

public class GetReviewsByCourseQueryHandler : IRequestHandler<GetReviewsByCourseQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICourseRepository _courseRepository;

    public GetReviewsByCourseQueryHandler(IReviewRepository reviewRepository, ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsByCourseQuery request, CancellationToken cancellationToken)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
            throw new Catalog.Domain.Exceptions.NotFoundException($"Course with id '{request.CourseId}' not found.");

        IReadOnlyList<Review> reviews = await _reviewRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);

        return reviews.Select(r => new ReviewDto(r.Id, r.CourseId, r.StudentId, r.Rating, r.Comment, r.CreatedAt)).ToList();
    }
}
