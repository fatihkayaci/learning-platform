using Catalog.Application.Common;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;

namespace Catalog.Application.Commands.AddReview;

public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Guid>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEnrollmentService _enrollmentService;

    public AddReviewCommandHandler(
        IReviewRepository reviewRepository,
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService,
        IEnrollmentService enrollmentService)
    {
        _courseRepository = courseRepository;
        _reviewRepository = reviewRepository;
        _currentUserService = currentUserService;
        _enrollmentService = enrollmentService;
    }

    public async Task<Guid> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new NotFoundException($"Course with id '{request.CourseId}' not found.");   
        Guid studentId = _currentUserService.UserId;
        bool reviewExist = await _reviewRepository.ExistsAsync(studentId, request.CourseId, cancellationToken);
        if (reviewExist)
            throw new BusinessException("Student has already submitted a review for this course.");

        bool isEnrolled = await _enrollmentService.IsEnrolledAsync(studentId, request.CourseId, cancellationToken);
        if (!isEnrolled)
            throw new BusinessException("Student is not enrolled in this course.");

        Review review = Review.Create(request.CourseId, studentId, request.Rating, request.Comment);
        await _reviewRepository.AddAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return review.Id;

    }
}