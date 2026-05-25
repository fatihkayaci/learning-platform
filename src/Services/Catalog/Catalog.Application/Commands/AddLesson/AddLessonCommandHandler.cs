using Catalog.Application.Common.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Commands.AddLesson;

public class AddLessonCommandHandler : IRequestHandler<AddLessonCommand, LessonDto>
{
    private readonly ILessonRepository _lessonRepository;

    public AddLessonCommandHandler(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonDto> Handle(AddLessonCommand request, CancellationToken cancellationToken)
    {
        Lesson lesson = Lesson.Create(request.Title, request.VideoUrl, request.Order, request.CourseId);
        await _lessonRepository.AddAsync(lesson, cancellationToken);
        await _lessonRepository.SaveChangesAsync(cancellationToken);
        return new LessonDto(lesson.Id, lesson.Title, lesson.VideoUrl, lesson.Order);
    }
}