using Catalog.Application.Commands.AddReview;
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace Catalog.Tests.Commands;

public class AddReviewCommandHandlerTests
{
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly ICourseRepository _courseRepository = Substitute.For<ICourseRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IEnrollmentService _enrollmentService = Substitute.For<IEnrollmentService>();
    private readonly AddReviewCommandHandler _handler;

    public AddReviewCommandHandlerTests()
    {
        _handler = new AddReviewCommandHandler(_reviewRepository, _courseRepository, _currentUserService, _enrollmentService);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        AddReviewCommand command = new(Guid.NewGuid(), 5, "Great course!");
        _courseRepository.GetByIdAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns((Course?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenReviewAlreadyExists_ThrowsBusinessException()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Course course = Course.Create("Clean Architecture", Guid.NewGuid(), Guid.NewGuid());
        AddReviewCommand command = new(course.Id, 5, "Great course!");
        _courseRepository.GetByIdAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(course);
        _currentUserService.UserId.Returns(studentId);
        _reviewRepository.ExistsAsync(studentId, command.CourseId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Student has already submitted a review for this course.");
    }

    [Fact]
    public async Task Handle_WhenStudentNotEnrolled_ThrowsBusinessException()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Course course = Course.Create("Clean Architecture", Guid.NewGuid(), Guid.NewGuid());
        AddReviewCommand command = new(course.Id, 5, "Great course!");
        _courseRepository.GetByIdAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(course);
        _currentUserService.UserId.Returns(studentId);
        _reviewRepository.ExistsAsync(studentId, command.CourseId, Arg.Any<CancellationToken>()).Returns(false);
        _enrollmentService.IsEnrolledAsync(studentId, command.CourseId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Student is not enrolled in this course.");
    }

    [Fact]
    public async Task Handle_WhenValidData_ReturnsReviewId()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Course course = Course.Create("Clean Architecture", Guid.NewGuid(), Guid.NewGuid());
        AddReviewCommand command = new(course.Id, 5, "Great course!");
        _courseRepository.GetByIdAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(course);
        _currentUserService.UserId.Returns(studentId);
        _reviewRepository.ExistsAsync(studentId, command.CourseId, Arg.Any<CancellationToken>()).Returns(false);
        _enrollmentService.IsEnrolledAsync(studentId, command.CourseId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Guid result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        await _reviewRepository.Received(1).AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
    }
}
