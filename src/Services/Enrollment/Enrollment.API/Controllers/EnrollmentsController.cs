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

    [HttpPost]
    public async Task<IActionResult> EnrollInCourse([FromBody] EnrollInCourseCommand command, CancellationToken cancellationToken)
    {
        Guid enrollmentId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(EnrollInCourse), new { id = enrollmentId }, new { id = enrollmentId });
    }
}
