using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Catalog;
using Catalog.Application.Commands.AddLesson;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace Catalog.Tests.Commands;

public class AddLessonCommandHandlerTests
{
    private readonly ILessonRepository _lessonRepository = Substitute.For<ILessonRepository>();
    private readonly ICourseRepository _courseRepository = Substitute.For<ICourseRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly AddLessonCommandHandler _handler;

    public AddLessonCommandHandlerTests()
    {
        _handler = new AddLessonCommandHandler(_lessonRepository, _courseRepository, _currentUserService, _eventPublisher);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        AddLessonCommand command = new("Lesson 1", "https://video.url", 1, Guid.NewGuid());
        _courseRepository.GetByIdAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns((Course?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotCourseOwner_ThrowsBusinessException()
    {
        // Arrange
        Guid instructorId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();
        Course course = Course.Create("Clean Architecture", instructorId, Guid.NewGuid());
        AddLessonCommand command = new("Lesson 1", "https://video.url", 1, course.Id);
        _courseRepository.GetByIdAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(course);
        _currentUserService.UserId.Returns(otherUserId);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("You are not the owner of this course.");
    }

    [Fact]
    public async Task Handle_WhenValidData_ReturnsLessonDtoAndPublishesEvent()
    {
        // Arrange
        Guid instructorId = Guid.NewGuid();
        Course course = Course.Create("Clean Architecture", instructorId, Guid.NewGuid());
        AddLessonCommand command = new("Lesson 1", "https://video.url", 1, course.Id);
        _courseRepository.GetByIdAsync(command.CourseId, Arg.Any<CancellationToken>()).Returns(course);
        _currentUserService.UserId.Returns(instructorId);

        // Act
        LessonDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Title.Should().Be(command.Title);
        await _lessonRepository.Received(1).AddAsync(Arg.Any<Lesson>(), Arg.Any<CancellationToken>());
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<LessonAddedEvent>(), Arg.Any<CancellationToken>());
    }
}
