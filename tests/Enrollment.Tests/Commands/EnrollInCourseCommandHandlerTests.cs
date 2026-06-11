using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Enrollment;
using Enrollment.Application.Commands.EnrollInCourse;
using Enrollment.Application.Common.Interfaces;
using Enrollment.Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace Enrollment.Tests.Commands;

public class EnrollInCourseCommandHandlerTests
{
    private readonly IEnrollmentRepository _enrollmentRepository = Substitute.For<IEnrollmentRepository>();
    private readonly ICourseService _courseService = Substitute.For<ICourseService>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly EnrollInCourseCommandHandler _handler;

    public EnrollInCourseCommandHandlerTests()
    {
        _handler = new EnrollInCourseCommandHandler(_enrollmentRepository, _courseService, _currentUserService, _eventPublisher);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        EnrollInCourseCommand command = new(Guid.NewGuid());
        _courseService.ExistsAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenAlreadyEnrolled_ThrowsBusinessException()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        EnrollInCourseCommand command = new(Guid.NewGuid());
        _courseService.ExistsAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(true);
        _currentUserService.UserId.Returns(studentId);
        _enrollmentRepository.ExistsAsync(studentId, command.CourseId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("You are already enrolled in this course.");
    }

    [Fact]
    public async Task Handle_WhenValidData_ReturnsEnrollmentIdAndPublishesEvent()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        EnrollInCourseCommand command = new(Guid.NewGuid());
        _courseService.ExistsAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(true);
        _currentUserService.UserId.Returns(studentId);
        _enrollmentRepository.ExistsAsync(studentId, command.CourseId, Arg.Any<CancellationToken>()).Returns(false);
        _courseService.GetLessonCountAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(5);

        // Act
        Guid result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        await _enrollmentRepository.Received(1).AddAsync(Arg.Any<Enrollment.Domain.Entities.Enrollment>(), Arg.Any<CancellationToken>());
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<StudentEnrolledEvent>(), Arg.Any<CancellationToken>());
    }
}
