using Catalog.Application.Commands.CreateCourse;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Catalog.Tests.Commands;

public class CreateCourseCommandHandlerTests
{
    private readonly ICourseRepository _courseRepository = Substitute.For<ICourseRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly CreateCourseCommandHandler _handler;

    public CreateCourseCommandHandlerTests()
    {
        _handler = new CreateCourseCommandHandler(_courseRepository, _currentUserService);
    }

    [Fact]
    public async Task Handle_WhenValidData_ReturnsCourseDto()
    {
        // Arrange
        Guid instructorId = Guid.NewGuid();
        Guid categoryId = Guid.NewGuid();
        CreateCourseCommand command = new("Clean Architecture", "Course description", categoryId);
        _currentUserService.UserId.Returns(instructorId);

        // Act
        CourseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be(command.Name);
        result.InstructorId.Should().Be(instructorId);
        await _courseRepository.Received(1).AddAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }
}
