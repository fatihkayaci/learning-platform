using Enrollment.Application.Commands.CompleteLesson;
using Enrollment.Application.Queries.CheckEnrollment;
using Enrollment.Application.Commands.EnrollInCourse;
using Enrollment.Application.DTOs;
using Enrollment.Application.Queries.GetEnrolledCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enrollment.API.Controllers;

[ApiController]
[Route("enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetEnrolledCourses(CancellationToken cancellationToken)
    {
        List<EnrolledCourseDto> result = await _mediator.Send(new GetEnrolledCoursesQuery(), cancellationToken);
        return Ok(result);
    }
    [HttpGet("check")]
    [AllowAnonymous]
    public async Task<IActionResult> IsEnrolled([FromQuery] Guid studentId, [FromQuery] Guid courseId, CancellationToken cancellationToken)
    {
        bool enrolled = await _mediator.Send(new CheckEnrollmentQuery(studentId, courseId), cancellationToken);
        return Ok(enrolled);
    }

    [HttpPost]
    public async Task<IActionResult> EnrollInCourse([FromBody] EnrollInCourseCommand command, CancellationToken cancellationToken)
    {
        Guid enrollmentId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(EnrollInCourse), new { id = enrollmentId }, new { id = enrollmentId });
    }

    [HttpPost("lessons/{lessonId}/complete")]
    public async Task<IActionResult> CompleteLesson([FromRoute] Guid lessonId, [FromBody] CompleteLessonRequest request, CancellationToken cancellationToken)
    {
        Guid lessonProgressId = await _mediator.Send(new CompleteLessonCommand(request.CourseId, lessonId), cancellationToken);
        return CreatedAtAction(nameof(CompleteLesson), new { id = lessonProgressId }, new { id = lessonProgressId });
    }
}
