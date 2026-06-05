using Enrollment.Application.Common.Interfaces;
using Enrollment.Domain.Entities;
using Enrollment.Domain.Exceptions;
using MediatR;

namespace Enrollment.Application.Commands.CompleteLesson;

public class CompleteLessonCommandHandler : IRequestHandler<CompleteLessonCommand, Guid>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly ICurrentUserService _currentUserService;

    public CompleteLessonCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ILessonProgressRepository lessonProgressRepository,
        ICurrentUserService currentUserService)
    {
        _enrollmentRepository = enrollmentRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CompleteLessonCommand request, CancellationToken cancellationToken)
    {
        Guid studentId = _currentUserService.UserId;

        Domain.Entities.Enrollment enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, request.CourseId, cancellationToken)
            ?? throw new NotFoundException("Enrollment not found for this course.");

        bool alreadyCompleted = await _lessonProgressRepository.ExistsAsync(enrollment.Id, request.LessonId, cancellationToken);
        if (alreadyCompleted)
            throw new BusinessException("This lesson is already completed.");

        LessonProgress lessonProgress = LessonProgress.Create(enrollment.Id, request.LessonId);

        await _lessonProgressRepository.AddAsync(lessonProgress, cancellationToken);
        await _lessonProgressRepository.SaveChangesAsync(cancellationToken);

        return lessonProgress.Id;
    }
}
