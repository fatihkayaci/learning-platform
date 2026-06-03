using Enrollment.Application.Commands.EnrollInCourse;
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

    [HttpPost]
    public async Task<IActionResult> EnrollInCourse([FromBody] EnrollInCourseCommand command, CancellationToken cancellationToken)
    {
        Guid enrollmentId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(EnrollInCourse), new { id = enrollmentId }, new { id = enrollmentId });
    }
}
